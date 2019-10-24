﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(KnightSpriteData))]
[RequiredComponent(typeof(SpriteRenderer))]
[RequiredComponent(typeof(SpeedManager))]
public class Attack : Action
{
	public SharedFloat Duration;
	public SharedInt Damage;
	public SharedVector2 HitBoxOffset;
	public SharedVector2 HitBoxSize;
	public SharedFloat ForwardStep;
	public LayerMask PlayerLayer;

	private KnightSpriteData m_KnightSpriteData;
	private float m_Timer;
	private EnemyAttackInfo AttackInfo;
	private bool m_AttackHit;

	public override void OnAwake()
	{
		m_KnightSpriteData = GetComponent<KnightSpriteData>();
	}

	public override void OnStart()
	{
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		GetComponent<SpriteRenderer>().sprite = m_KnightSpriteData.Recovery;
		m_AttackHit = false;
		bool isRight = transform.eulerAngles.y == 0f;
		AttackInfo = new EnemyAttackInfo(Owner.gameObject, isRight, Damage.Value, HitBoxOffset.Value, HitBoxSize.Value);
		GetComponent<SpeedManager>().SelfSpeed.x = transform.right.x * ForwardStep.Value;
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer <= Time.timeSinceLevelLoad)
		{
			return TaskStatus.Success;
		}
		CheckHitPlayer();
		return TaskStatus.Running;
	}

	private void CheckHitPlayer()
	{
		if (!m_AttackHit && HitPlayer(AttackInfo))
		{
			m_AttackHit = true;
			GetComponent<SpeedManager>().SelfSpeed.x = 0;
		}
	}

	private bool HitPlayer(EnemyAttackInfo Attack)
	{
		Vector2 Offset = Attack.HitBoxOffset;
		Offset.x = transform.right.x * Offset.x;

		RaycastHit2D Hit = Physics2D.BoxCast(transform.position + (Vector3)Offset, Attack.HitBoxSize, 0, transform.right, 0, PlayerLayer);

		if (Hit)
		{
			Hit.collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void OnDrawGizmos()
	{
#if UNITY_EDITOR
		Rect boxCastRect = new Rect((Vector2)Owner.transform.position + HitBoxOffset.Value, HitBoxSize.Value);
		boxCastRect.center = (Vector2)Owner.transform.position + HitBoxOffset.Value;
		UnityEditor.Handles.DrawSolidRectangleWithOutline(boxCastRect,
			Color.cyan, Color.white);
#endif
	}

}