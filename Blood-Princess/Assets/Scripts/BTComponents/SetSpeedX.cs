using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class SetSpeedX : Action
{
	public SharedFloat SpeedX;
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
		Vector2 CurrentSpeed = new Vector2(m_SpeedManager.SelfSpeed.x, m_SpeedManager.SelfSpeed.y);
		m_SpeedManager.SelfSpeed = new Vector2(SpeedX.Value * Owner.transform.right.x, CurrentSpeed.y);
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
