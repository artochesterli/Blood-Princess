using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(BehaviorTree))]
public class HitPool : MonoBehaviour
{
	public float Interval;
	public int MaxDamageThreshold;
	public bool ResetWhenPlayerIsHit = true;

	private int m_CurrentDamage;
	private float m_Timer;
	private FSM<HitPool> m_HitPoolFSM;
	private BehaviorTree m_BT;

	private void Awake()
	{
		m_HitPoolFSM = new FSM<HitPool>(this);
		m_HitPoolFSM.TransitionTo<IdleState>();
		m_BT = GetComponent<BehaviorTree>();
	}

	private void Update()
	{
		m_HitPoolFSM.Update();
	}

	private void m_OnHit(PlayerHitEnemy ev)
	{
		if ((bool)m_BT.GetVariable("Staggering").GetValue())
		{
			return;
		}
		m_HitPoolFSM.TransitionTo<HitState>();
		m_CurrentDamage += ev.UpdatedAttack.Damage;
		if (m_CurrentDamage >= MaxDamageThreshold)
		{
			Debug.Log("Send Stagger Event");
			m_BT.SendEvent("Stagger");
		}
	}

	private void m_OnHitPlayer(PlayerGetHit ev)
	{
		if (ResetWhenPlayerIsHit)
		{
			m_CurrentDamage = 0;
		}
	}

	private void OnEnable()
	{
		EventManager.instance.AddHandler<PlayerHitEnemy>(m_OnHit);
		EventManager.instance.AddHandler<PlayerGetHit>(m_OnHitPlayer);
	}

	private void OnDisable()
	{
		EventManager.instance.RemoveHandler<PlayerHitEnemy>(m_OnHit);
		EventManager.instance.RemoveHandler<PlayerGetHit>(m_OnHitPlayer);
	}

	private abstract class PoolState : FSM<HitPool>.State
	{
	}

	private class IdleState : PoolState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.m_CurrentDamage = 0;
			Context.m_Timer = 0f;
		}
	}

	private class HitState : PoolState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.m_Timer = Time.timeSinceLevelLoad + Context.Interval;
		}

		public override void Update()
		{
			base.Update();
			if (Context.m_Timer < Time.timeSinceLevelLoad)
			{
				TransitionTo<IdleState>();
				return;
			}
		}
	}
}
