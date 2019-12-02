using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(SpeedManager))]
public class DownwardSkewedSmash : Action
{
	public SharedFloat AnticipateDuration;
	public SharedFloat ChargeSpeed;
	public LayerMask DefaultLayerMask;

	private GameObject m_Player;
	private Vector2 m_Direction;
	private float m_Timer;
	private SpeedManager m_SpeedManager;

	public override void OnAwake()
	{
		base.OnAwake();
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_SpeedManager = Owner.GetComponent<SpeedManager>();
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Direction = (m_Player.transform.position - Owner.transform.position).normalized;
		m_Timer = Time.timeSinceLevelLoad + AnticipateDuration.Value;
	}

	private bool m_ChargeStop()
	{
		m_SpeedManager.SelfSpeed = m_Direction * ChargeSpeed.Value;
		if (Physics2D.Raycast(Owner.transform.position, m_Direction, 2f, DefaultLayerMask))
		{
			return true;
		}
		return false;
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer > Time.timeSinceLevelLoad)
		{
			return TaskStatus.Running;
		}
		if (m_ChargeStop())
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
