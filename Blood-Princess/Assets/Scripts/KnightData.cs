using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightData : MonoBehaviour
{
    public int MaxHP;

    public float NormalMoveSpeed;
    public float KeepDisMoveSpeed;
    public float AttackStepForwardSpeed;

    public float SingleAttackAnticipationTime;
    public float DoubleAttackFirstAnticipationTime;
    public float DoubleAttackSecondAnticipationTime;
    public float AttackTime;
    public float AttackRecoveryTime;
    public int Damage;

    public Vector2 AttackOffset;
    public Vector2 AttackHitBoxSize;

    public float InterruptedTime;
    public float InterruptedSpeed_BloodSlash;
    public float InterruptedSpeed_DeadSlash;
    public float InterruptedSpeed_Explosion;
    public float InterruptedMoveTime;

    public float TacticDistance;
    public float DangerDistance;
    public float ChaseForAttackDistance;
    public float TacticDecisionInterval;
    public float ShortDisAttackDecisionChance;
    public float LongDisAttackDecisionChance;
    public float ShortDisSingleAttackChance;
    public float LongDisSingleAttackChance;

    public int MaxStamina;
    public int MaxPatience;

    public int ShockDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
