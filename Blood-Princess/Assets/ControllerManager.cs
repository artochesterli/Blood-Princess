using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ControllerManager : MonoBehaviour
{
    public static Joystick CharacterJoystick;
    public static Player Character;

    // Start is called before the first frame update
    void Start()
    {
        CharacterJoystick = null;
        Character = ReInput.players.GetPlayer(0);
        ReInput.ControllerConnectedEvent += OnControllerConnect;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnect;

        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            if (CharacterJoystick == null)
            {
                CharacterJoystick = j;
                Character.controllers.AddController(CharacterJoystick, false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnControllerConnect(ControllerStatusChangedEventArgs args)
    {
        if(CharacterJoystick == null)
        {
            CharacterJoystick = ReInput.controllers.GetController<Joystick>(args.controllerId);
            Character.controllers.AddController(CharacterJoystick, false);
        }
    }

    private void OnControllerDisconnect(ControllerStatusChangedEventArgs args)
    {
        if(CharacterJoystick !=null && CharacterJoystick.id == args.controllerId)
        {
            CharacterJoystick = null;
        }
    }
}
