using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EventManager
{
    private EventManager() { }

    public delegate void EventDelegate<T>(T e) where T : GameEvent;
    private delegate void EventDelegate(GameEvent e);

    private readonly Dictionary<Type, EventDelegate> _delegates = new Dictionary<Type, EventDelegate>();
    private readonly Dictionary<Delegate, EventDelegate> _delegateLookup = new Dictionary<Delegate, EventDelegate>();

    public static EventManager instance = new EventManager();
    
    public void AddHandler<T>(EventDelegate<T> del) where T: GameEvent
    {

        if (_delegateLookup.ContainsKey(del))
        {
            return;
        }

        EventDelegate internalDelegate = (e) => del((T)e);
        _delegateLookup[del] = internalDelegate;

        EventDelegate tdel;
        if(_delegates.TryGetValue(typeof(T),out tdel))
        {
            _delegates[typeof(T)] = tdel+=internalDelegate;
        }
        else
        {
            _delegates[typeof(T)] = internalDelegate;
        }
    }

    public void RemoveHandler<T>(EventDelegate<T> del) where T : GameEvent
    {

        EventDelegate internalDelegate;
        if(_delegateLookup.TryGetValue(del,out internalDelegate))
        {
            EventDelegate tdel;
            if(_delegates.TryGetValue(typeof(T),out tdel))
            {
                tdel -= internalDelegate;
                if (tdel == null)
                {
                    _delegates.Remove(typeof(T));
                }
                else
                {
                    _delegates[typeof(T)] = tdel;
                }
            }
            _delegateLookup.Remove(del);
        }
    }

    public void Fire(GameEvent e)
    {

        EventDelegate del;
        if (_delegates.TryGetValue(e.GetType(), out del))
        {

            del.Invoke(e);
        }
    }

}
