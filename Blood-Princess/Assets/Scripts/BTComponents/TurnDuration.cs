using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class TurnDuration : Action
{
	public SharedFloat Duration;
	public SharedFloat ZRotationEuler;

	private float m_Timer;

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		Owner.transform.DOLocalRotate(new Vector3(0f, 0f, ZRotationEuler.Value), Duration.Value).SetRelative();
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
