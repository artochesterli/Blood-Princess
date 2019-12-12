using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChaosBladeControl : MonoBehaviour
{
    public AnimationCurve ScaleEase;
    public GameObject ExplosionEffectPrefab;
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

                EnemyAttackInfo attackInfo = new EnemyAttackInfo(Owner, isRight ? Direction.Right : Direction.Left, Damage, Damage, BladeHitBoxSize, BladeHitBoxOffset);
                hit.gameObject.GetComponent<IHittable>().OnHit(attackInfo);
                m_HitOnce = true;
                Destroy(gameObject);
            }
        }
    }

    private void m_ExplodePlayerToHit()
    {
        if (!m_HitOnce)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position,
                0.5f,
                PlayerLayer);
            if (hit != null)
            {
                // Hit Player
                bool isRight = transform.position.x < hit.gameObject.transform.position.x;

                EnemyAttackInfo attackInfo = new EnemyAttackInfo(Owner, isRight ? Direction.Right : Direction.Left, Damage, Damage, BladeHitBoxSize, BladeHitBoxOffset);
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
            float duration = Context.MaxDistance / Context.FlySpeed;
            Context.transform.DOScale(0f, duration).SetEase(Context.ScaleEase);
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
                TransitionTo<ExplodeState>();
                return;
            }
        }
    }

    private class ExplodeState : BladeState
    {
        private float Timer;
        public override void OnEnter()
        {
            base.OnEnter();
            Context.m_HitOnce = false;
            Timer = Time.timeSinceLevelLoad + 0.2f;
            GameObject.Instantiate(Context.ExplosionEffectPrefab, Context.transform.position, Context.ExplosionEffectPrefab.transform.rotation);
        }

        public override void Update()
        {
            base.Update();
            Context.m_ExplodePlayerToHit();
            if (Timer < Time.timeSinceLevelLoad)
            {
                Destroy(Context.gameObject);
                return;
            }
        }
    }
}
