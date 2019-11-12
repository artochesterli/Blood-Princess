using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosBladeControl : MonoBehaviour
{
	private FSM<ChaosBladeControl> m_ChaosBladeFSM;
	private GameObject Owner;
	private Vector2 BladeDirection;
	private float MaxDistance;
	private float FlySpeed;
	private float WaitTime;
	public Vector2 BladeHitBoxSize;
	public Vector2 BladeHitBoxOffset;
	private int Damage;
	private LayerMask PlayerLayer;

	private bool m_HitOnce;
	private float m_CurrentDistance;
	// private Vector2 m_DestinationPosition;

	private void Awake()
	{
		m_ChaosBladeFSM = new FSM<ChaosBladeControl>(this);
		m_ChaosBladeFSM.TransitionTo<IdleState>();
	}

	private void Update()
	{
		m_ChaosBladeFSM.Update();
		if (Owner == null)
			Destroy(gameObject);
	}

	public void InitiatedBlade(GameObject owner, Vector2 bladeDirection, float maxDistance, float flySpeed, float waitSpeed, int damage, LayerMask playerLayer)
	{
		Owner = owner;
		BladeDirection = bladeDirection.normalized;
		MaxDistance = maxDistance;
		FlySpeed = flySpeed;
		WaitTime = waitSpeed;
		Damage = damage;
		PlayerLayer = playerLayer;
		// m_DestinationPosition = (Vector2)transform.position + BladeDirection * MaxDistance;
		m_ChaosBladeFSM.TransitionTo<FlyOutState>();
	}

	private void m_CastPlayerToHit()
	{
		if (!m_HitOnce)
		{
			Collider2D hit = Physics2D.OverlapBox(transform.position,
				BladeHitBoxSize,
				0f,
				PlayerLayer);
			if (hit != null)
			{
				// Hit Player
				bool isRight = transform.position.x < hit.gameObject.transform.position.x;
				EnemyAttackInfo attackInfo = new EnemyAttackInfo(Owner, isRight, Damage, Damage, BladeHitBoxSize, BladeHitBoxOffset);
				hit.gameObject.GetComponent<IHittable>().OnHit(attackInfo);
				m_HitOnce = true;
			}
		}
	}

	private abstract class BladeState : FSM<ChaosBladeControl>.State { }

	private class IdleState : BladeState { }

	private class FlyOutState : BladeState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.m_CurrentDistance = 0f;
			Context.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(Context.BladeDirection.y, Context.BladeDirection.x) * Mathf.Rad2Deg);
		}

		public override void Update()
		{
			base.Update();
			if (Mathf.Abs(Context.m_CurrentDistance - Context.MaxDistance) < Context.FlySpeed * 0.02f)
			{
				TransitionTo<WaitState>();
				return;
			}
			Context.transform.Translate((Vector3)Context.BladeDirection * Time.deltaTime * Context.FlySpeed, Space.World);
			Context.m_CurrentDistance += Time.deltaTime * Context.FlySpeed;
			Context.m_CastPlayerToHit();
		}
	}

	private class WaitState : BladeState
	{
		private float _timer;
		public override void OnEnter()
		{
			base.OnEnter();
			_timer = Time.timeSinceLevelLoad + Context.WaitTime;
		}

		public override void Update()
		{
			base.Update();
			if (_timer < Time.timeSinceLevelLoad)
			{
				TransitionTo<FlyInState>();
				return;
			}
		}
	}

	private class FlyInState : BladeState
	{
		private Vector2 _dir;
		public override void OnEnter()
		{
			base.OnEnter();
			Context.m_HitOnce = false;
			Context.m_CurrentDistance = 0f;
			_dir = -Context.BladeDirection;
			Context.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg);
		}

		public override void Update()
		{
			base.Update();
			if (Mathf.Abs(Context.m_CurrentDistance - Context.MaxDistance) < Context.FlySpeed * 0.02f)
			{
				Destroy(Context.gameObject);
				return;
			}
			Context.transform.Translate((Vector3)_dir * Time.deltaTime * Context.FlySpeed, Space.World);
			Context.m_CurrentDistance += Time.deltaTime * Context.FlySpeed;
			Context.m_CastPlayerToHit();
		}
	}
}
