using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(GravityManager))]
public class SetGravity : Action
{
	public SharedFloat Gravity;
	public SharedBool Relative;
	public SharedFloat Duration;

	private float m_timer;

	public override void OnStart()
	{
		base.OnStart();
		if (Relative.Value)
		{
			GetComponent<GravityManager>().Gravity += Gravity.Value;
		}
		else
		{
			GetComponent<GravityManager>().Gravity = Gravity.Value;
		}
		m_timer = Time.timeSinceLevelLoad + Duration.Value;
	}

	public override TaskStatus OnUpdate()
	{
		if (m_timer < Time.timeSinceLevelLoad)
			return TaskStatus.Success;
		return TaskStatus.Running;
	}
}
