using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(KnightSpriteData))]
public class Recovery : Action
{
	public SharedFloat Duration;

	private KnightSpriteData m_KnightSpriteData;
	private float m_Timer;

	public override void OnAwake()
	{
		base.OnAwake();
		m_KnightSpriteData = GetComponent<KnightSpriteData>();
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		GetComponent<SpriteRenderer>().sprite = m_KnightSpriteData.Idle;
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
