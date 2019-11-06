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
		m_SpeedManager.SelfSpeed = new Vector2(Speed.Value.x * Owner.transform.right.x, Speed.Value.y);
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
