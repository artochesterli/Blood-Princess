﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class SetSpeed : Action
{
	public SharedVector2 Speed;
	public SharedFloat Duration;
	public SharedBool Relative;

	private float m_Timer;
	private SpeedManager m_SpeedManager;

	public override void OnAwake()
	{
		base.OnAwake();
		m_SpeedManager = GetComponent<SpeedManager>();
	}

	public override void OnStart()
	{
		base.OnStart();
		Vector2 CurrentSpeed = new Vector2(m_SpeedManager.SelfSpeed.x, m_SpeedManager.SelfSpeed.y);
		if (!Relative.Value)
			m_SpeedManager.SelfSpeed = new Vector2(Speed.Value.x * Owner.transform.right.x, Speed.Value.y);
		else
		{
			m_SpeedManager.SelfSpeed.x = Speed.Value.x + CurrentSpeed.x;
			m_SpeedManager.SelfSpeed.y = Speed.Value.y + CurrentSpeed.y;
		}
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer < Time.timeSinceLevelLoad)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
