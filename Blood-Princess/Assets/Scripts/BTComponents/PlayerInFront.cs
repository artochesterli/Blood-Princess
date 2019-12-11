﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class PlayerInFront : Conditional
{
    public SharedFloat Distance;
    public SharedFloat HalfAngle;
    public SharedLayerMask ObstacleLayer;
    private SpeedManager m_SpeedManager;
    private SpeedManager m_PlayerSpeedManager;
    private GameObject m_Player;

    public override void OnAwake()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        Debug.Assert(m_Player != null, "Could not Find Player Object");
        m_PlayerSpeedManager = m_Player.GetComponent<SpeedManager>();
        Debug.Assert(m_PlayerSpeedManager != null, "Player Does Not Have a Speed Manager");
        m_SpeedManager = GetComponent<SpeedManager>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_isPlayerInSight())
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    private bool _isPlayerInSight()
    {
        if (Vector2.Distance(m_SpeedManager.GetTruePos(), m_PlayerSpeedManager.GetTruePos()) < Distance.Value &&
            Vector3.Angle((m_PlayerSpeedManager.GetTruePos() - m_SpeedManager.GetTruePos()).normalized, Owner.transform.right) < HalfAngle.Value &&
            !Physics2D.Linecast(m_SpeedManager.GetTruePos(), m_PlayerSpeedManager.GetTruePos(), ObstacleLayer.Value))
        {
            return true;
        }
        return false;
    }

}
