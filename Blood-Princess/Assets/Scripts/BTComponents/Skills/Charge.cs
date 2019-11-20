using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
[RequiredComponent(typeof(BoxCollider2D))]
public class Charge : Action
{
	public SharedFloat MaxDuration;
	public SharedFloat ChargeSpeed;
	public SharedFloat DetectionRange;
	public LayerMask PlayerLayer;

	private GameObject m_Player;
	private SpeedManager m_SpeedManager;
	private float m_Timer;
	private BoxCollider2D m_BoxCollider2D;

	public override void OnAwake()
	{
		m_SpeedManager = Owner.GetComponent<SpeedManager>();
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_BoxCollider2D = Owner.GetComponent<BoxCollider2D>();
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + MaxDuration.Value;
	}

	private void m_Move()
	{
		m_SpeedManager.SelfSpeed.x = ChargeSpeed.Value * Owner.transform.right.x;
	}

	private bool m_PlayerInFront()
	{
		Vector2 origin = m_SpeedManager.GetTruePos();
		origin.x += (Owner.transform.right.x * m_BoxCollider2D.bounds.extents.x);
		return Physics2D.BoxCast(origin, m_BoxCollider2D.size, 0f, Owner.transform.right, DetectionRange.Value, PlayerLayer);
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer < Time.timeSinceLevelLoad || m_PlayerInFront())
		{
			return TaskStatus.Success;
		}

		m_Move();
		return TaskStatus.Running;
	}
}
