using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(SpeedManager))]
public class AttackTimed : Action
{
	public SharedInt Damage;
	public SharedFloat Duration;
	public SharedVector2 RelativeStartPosition;
	public SharedVector2 RelativeFinalPosition;
	public SharedFloat HitboxHeight;
	public LayerMask PlayerLayer;

	private SpeedManager m_SpeedManager;
	private float m_Timer;
	private Vector2 m_AttackStart
	{
		get
		{
			Vector2 RelativePosition = RelativeStartPosition.Value;
			RelativePosition.x *= Owner.transform.right.x;
			return m_SpeedManager.GetTruePos() + RelativePosition;
		}
	}
	private Vector2 m_AttackEnd
	{
		get
		{
			Vector2 RelativePosition = RelativeFinalPosition.Value;
			RelativePosition.x *= Owner.transform.right.x;
			return m_SpeedManager.GetTruePos() + RelativePosition;
		}
	}
	private Vector2 m_AttackPoint;
	private bool m_AttackHit;
	private EnemyAttackInfo AttackInfo;

	public override void OnAwake()
	{
		base.OnAwake();
		m_SpeedManager = GetComponent<SpeedManager>();
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		m_AttackPoint = m_AttackStart;
		m_AttackHit = false;
		AttackInfo = new EnemyAttackInfo(Owner.gameObject, true, Damage.Value, Damage.Value, Vector2.zero, Vector2.zero);
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer < Time.timeSinceLevelLoad)
		{
			return TaskStatus.Success;
		}
		m_Attack();
		return TaskStatus.Running;
	}

	private void m_Attack()
	{
		float percentage = 1f - ((m_Timer - Time.timeSinceLevelLoad) / Duration.Value);
		m_AttackPoint = Vector2.Lerp(m_AttackStart, m_AttackEnd, percentage);
		if (HitPlayer(AttackInfo) && !m_AttackHit)
		{
			m_AttackHit = true;
			m_SpeedManager.SelfSpeed = Vector2.zero;
		}
	}

	private bool HitPlayer(EnemyAttackInfo Attack)
	{
		Vector2 AttackMiddlePoint = (m_AttackStart + m_AttackPoint) / 2f;
		float distance = Vector2.Distance(m_AttackStart, m_AttackPoint);
		float angle = Vector2.Angle(Vector2.right, m_AttackPoint - m_AttackStart);
		RaycastHit2D Hit = Physics2D.BoxCast(AttackMiddlePoint, new Vector2(distance, HitboxHeight.Value), angle, transform.right, 0, PlayerLayer);

		if (Hit)
		{
			Attack.Right = m_SpeedManager.GetTruePos().x < Hit.collider.gameObject.GetComponent<SpeedManager>().GetTruePos().x;
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
		UnityEditor.Handles.DrawWireDisc(Owner.transform.position + (Vector3)RelativeStartPosition.Value, Vector3.forward, 0.1f);
		UnityEditor.Handles.DrawWireDisc(Owner.transform.position + (Vector3)RelativeFinalPosition.Value, Vector3.forward, 0.1f);

		//Rect boxCastRect = new Rect((Vector2)Owner.transform.position + HitBoxOffset.Value, HitBoxSize.Value);
		//boxCastRect.center = (Vector2)Owner.transform.position + HitBoxOffset.Value;
		//UnityEditor.Handles.DrawSolidRectangleWithOutline(boxCastRect,
		//	new Color(0, 1, 1, 0.4f), Color.white);
#endif
	}
}
