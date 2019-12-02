using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class PlayerInRange : Conditional
{
	public SharedFloat Range;

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
		if (Vector2.Distance(m_PlayerSpeedManager.GetTruePos(), m_SpeedManager.GetTruePos()) < Range.Value)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
