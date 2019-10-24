﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class Patrol : Action
{
    public SharedFloat Speed;
    public SharedFloat ArrivalDistance;
    public SharedFloat MinWayPointPauseDuration = 0f;
    public SharedFloat MaxWayPointPauseDuration = 2f;
    public SharedVector2List WayPoints;

    private List<Vector2> m_TargetPositions;
    private int m_WayPointIndex;
    private float m_WayPointReachTimer;
    private SpeedManager m_SpeedManager;
    private Vector2 m_TruePosition { get { return m_SpeedManager.GetTruePos(); } }

    public override void OnAwake()
    {
        m_WayPointReachTimer = -1f;
        m_SpeedManager = GetComponent<SpeedManager>();
        Debug.Assert(m_SpeedManager != null, "There is no Speed Manager on GameObject: " + gameObject.name);
        Debug.Assert(WayPoints.Value.Count > 0, "There are no way points on GameObject: " + gameObject.name);
        m_TargetPositions = new List<Vector2>();
        for (int i = 0; i < WayPoints.Value.Count; i++)
        {
            m_TargetPositions.Add(new Vector2(m_TruePosition.x + WayPoints.Value[i].x, m_TruePosition.y + WayPoints.Value[i].y));
        }
    }
    public override TaskStatus OnUpdate()
    {
        // If Arrived at point, then wait for seconds
        if (_hasArrived())
        {
            if (m_WayPointReachTimer == -1)
            {
                m_SpeedManager.SelfSpeed = Vector2.zero;
                m_WayPointReachTimer = Time.timeSinceLevelLoad + Random.Range(MinWayPointPauseDuration.Value, MaxWayPointPauseDuration.Value);
            }
            if (m_WayPointReachTimer <= Time.timeSinceLevelLoad)
            {
                m_WayPointIndex = (m_WayPointIndex + 1) % m_TargetPositions.Count;
                m_WayPointReachTimer = -1;
            }
        }
        // If not arrived at points, then move towards the next point
        else
        {
            bool MovingRight = m_TruePosition.x < m_TargetPositions[m_WayPointIndex].x;

            if (MovingRight) transform.eulerAngles = Vector3.zero;
            else transform.eulerAngles = new Vector3(0f, 180f, 0f);

            // Set Speed
            m_SpeedManager.SelfSpeed.x = (MovingRight ? 1f : -1f) * Speed.Value;

        }
        return TaskStatus.Running;
    }
    private bool _hasArrived()
    {
        return Vector2.Distance(m_TruePosition, m_TargetPositions[m_WayPointIndex]) < ArrivalDistance.Value;
    }

    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (WayPoints == null || WayPoints.Value == null)
        {
            return;
        }
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        for (int i = 0; i < WayPoints.Value.Count; i++)
        {
            if (WayPoints.Value[i] != null)
            {
                UnityEditor.Handles.SphereHandleCap(0, WayPoints.Value[i] + new Vector2(Owner.transform.position.x, Owner.transform.position.y),
                Quaternion.identity,
                0.5f,
                EventType.Repaint);
            }
        }
        UnityEditor.Handles.color = oldColor;
#endif
    }
}