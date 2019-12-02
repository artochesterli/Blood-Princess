using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
[RequiredComponent(typeof(GravityManager))]
public class FlyTo : Action
{
	public SharedFloat FlyTime;
	private SpeedManager m_SpeedManager;
	private GameObject m_Player;
	private GravityManager m_GravityManager;

	public override void OnAwake()
	{
		m_SpeedManager = GetComponent<SpeedManager>();
		m_GravityManager = GetComponent<GravityManager>();
		m_Player = GameObject.FindGameObjectWithTag("Player");
	}

	public override void OnStart()
	{
		float xOffset = m_Player.transform.position.x - m_SpeedManager.GetTruePos().x;
		float xVelocity = xOffset / FlyTime.Value;

		float yVelocity = m_GravityManager.Gravity * 10f * FlyTime.Value / 2f;
		m_SpeedManager.SelfSpeed.x = xVelocity;
		m_SpeedManager.SelfSpeed.y = yVelocity;
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
