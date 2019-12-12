using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using DG.Tweening;

[RequireComponent(typeof(BehaviorTree))]
public class HitReaction : MonoBehaviour
{
    public float Interval;
    public int MaxDamageThreshold;
    public bool ResetWhenPlayerIsHit = true;

    private int m_CurrentDamage;
    private float m_Timer;
    private FSM<HitReaction> m_HitPoolFSM;
    private BehaviorTree m_BT;

    private int m_CurrentDamageThreshold
    {
        get
        {
            if ((bool)m_BT.GetVariable("HasEntered2ndPhase").GetValue())
            {
                return MaxDamageThreshold + 15;
            }
            else if ((bool)m_BT.GetVariable("HasEntered3rdPhase").GetValue())
            {
                return MaxDamageThreshold + 20;
            }
            return MaxDamageThreshold;
        }
    }

    private void Awake()
    {
        m_HitPoolFSM = new FSM<HitReaction>(this);
        m_HitPoolFSM.TransitionTo<IdleState>();
        m_BT = GetComponent<BehaviorTree>();
    }

    private void Update()
    {
        m_HitPoolFSM.Update();
    }

    private void m_OnHit(PlayerHitEnemy ev)
    {
        // transform.DOShakePosition(0.2f, new Vector3(0.4f, 0f), 50, 90).SetRelative(true);
        // Debug.Log("Shake");
        if ((bool)m_BT.GetVariable("Staggering").GetValue() ||
        (bool)m_BT.GetVariable("OneToTwoTransition").GetValue() ||
        (bool)m_BT.GetVariable("TwoToThreeTransition").GetValue())
        {
            return;
        }
        m_HitPoolFSM.TransitionTo<HitState>();

        m_CurrentDamage += Utility.GetEffectValue(ev.UpdatedAttack.Power, ev.UpdatedAttack.Potency);
        if (m_CurrentDamage >= MaxDamageThreshold)
        {
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

    private abstract class PoolState : FSM<HitReaction>.State
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
