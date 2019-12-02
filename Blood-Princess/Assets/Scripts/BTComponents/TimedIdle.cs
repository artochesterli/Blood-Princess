using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class TimedIdle : Action
{
	public SharedFloat MinIdleTime;
	public SharedFloat MaxIdleTime;
	private float m_Timer;

	public override void OnStart()
	{
		m_Timer = Time.timeSinceLevelLoad + Random.Range(MinIdleTime.Value, MaxIdleTime.Value);
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
