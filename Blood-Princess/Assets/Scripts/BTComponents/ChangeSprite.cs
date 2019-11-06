using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(KnightSpriteData))]
[RequiredComponent(typeof(SpriteRenderer))]
public class ChangeSprite : Action
{
	public SharedFloat Duration;
	public Sprite Sprite;
	private KnightSpriteData m_KnightSpriteData;
	private SpriteRenderer m_SpriteRenderer;
	private float m_Timer;

	public override void OnAwake()
	{
		base.OnAwake();
		m_KnightSpriteData = GetComponent<KnightSpriteData>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		m_SpriteRenderer.sprite = Sprite;
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer < Time.timeSinceLevelLoad)
		{
			m_SpriteRenderer.sprite = m_KnightSpriteData.Idle;
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}

}
