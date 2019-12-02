using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using DG.Tweening;

[RequiredComponent(typeof(SpriteRenderer))]
public class Vanquish : Action
{
	public SharedFloat Duration;
	public SharedFloat FadeToAmount;

	private SpriteRenderer m_SpriteRenderer;
	private float m_Timer;

	public override void OnAwake()
	{
		base.OnAwake();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		m_SpriteRenderer.DOFade(FadeToAmount.Value, Duration.Value);
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
