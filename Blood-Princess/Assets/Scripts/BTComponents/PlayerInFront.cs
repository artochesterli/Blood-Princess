using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class PlayerInFront : Conditional
{
	public SharedFloat Distance;
	public SharedFloat HalfAngle;
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
		float playerPosX = m_PlayerSpeedManager.GetTruePos().x;
		float playerPosY = m_PlayerSpeedManager.GetTruePos().y;
		if (_isPlayerInSight())
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}

	private bool _isPlayerInSight()
	{
		if (Vector2.Distance(Owner.transform.position, m_Player.transform.position) < Distance.Value &&
			Vector3.Angle(Owner.transform.right, m_Player.transform.right) > 180f - HalfAngle.Value)
		{
			return true;
		}
		return false;
	}

}
