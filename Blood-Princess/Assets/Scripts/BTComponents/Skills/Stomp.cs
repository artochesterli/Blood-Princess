using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Stomp : Action
{
	public SharedInt Damage;
	public SharedBool Right;
	public SharedFloat Speed;
	public SharedFloat Duration;
	public SharedVector2 StartOffset;
	public SharedVector2 HitBoxSize;
	public LayerMask PlayerLayer;

	private float m_Timer;
	private Vector2 m_StartPosition;
	private EnemyAttackInfo m_EnemyAttackInfo;
	private GameObject m_Player;
	private GameObject m_WaveObject;
	private bool m_Hit;

	public override void OnAwake()
	{
		base.OnAwake();
		m_Player = GameObject.FindGameObjectWithTag("Player");
		Debug.Assert(m_Player != null, "Player cannot be found");
	}

	public override void OnStart()
	{
		base.OnStart();
		m_Timer = Time.timeSinceLevelLoad + Duration.Value;
		m_StartPosition = (Vector2)Owner.transform.position + StartOffset.Value;
		m_EnemyAttackInfo = new EnemyAttackInfo(Owner.gameObject, true, Damage.Value, Damage.Value, Vector2.zero, HitBoxSize.Value);
		m_WaveObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Wave"));
		m_WaveObject.transform.position = m_StartPosition;
		m_WaveObject.GetComponent<SpriteRenderer>().size = HitBoxSize.Value;
		m_Hit = false;
	}

	public override TaskStatus OnUpdate()
	{
		if (m_Timer < Time.timeSinceLevelLoad)
		{
			GameObject.Destroy(m_WaveObject);
			return TaskStatus.Success;
		}
		m_ScanHit();
		return TaskStatus.Running;
	}

	private void m_ScanHit()
	{
		m_StartPosition.x += Speed.Value * (Right.Value ? 1f : -1f) * Time.deltaTime;
		m_EnemyAttackInfo.Right = m_StartPosition.x < m_Player.transform.position.x;
		m_WaveObject.transform.position = m_StartPosition;
		if (!m_Hit && HitPlayer(m_EnemyAttackInfo))
		{
			m_Hit = true;
		}
	}

	private bool HitPlayer(EnemyAttackInfo Attack)
	{
		Vector2 Offset = Attack.HitBoxOffset;
		Offset.x = transform.right.x * Offset.x;

		RaycastHit2D Hit = Physics2D.BoxCast(m_StartPosition, Attack.HitBoxSize, 0, transform.right, 0, PlayerLayer);

		if (Hit)
		{
			Attack.Right = Owner.GetComponent<SpeedManager>().GetTruePos().x < Hit.collider.gameObject.GetComponent<SpeedManager>().GetTruePos().x;
			Hit.collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
			return true;
		}
		else
		{
			return false;
		}
	}
}
