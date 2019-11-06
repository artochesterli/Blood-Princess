using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightData : MonoBehaviour
{
    public int MaxHP;
    public int MaxShield;

    public float NormalMoveSpeed;
    public float KeepDisMoveSpeed;
    public float AttackStepForwardSpeed;


    public float StepBackAndForthMinTime;
    public float StepBackAndForthMaxTime;

    public float SingleAttackAnticipationTime;
    public float DoubleAttackFirstAnticipationTime;
    public float DoubleAttackSecondAnticipationTime;
    public float ChaseAttackAnticipationTime;
    public float ChaseAttackSpeed;
    public float ChaseAttackTriggerDistance;
    public float MaxChaseAttackDistance;
    public float AttackTime;
    public float SingleAttackRecoveryTime;
    public float DoubleAttackFirstRecoveryTime;
    public float DoubleAttackSecondRecoveryTime;
    public int Damage;

    public Vector2 AttackOffset;
    public Vector2 AttackHitBoxSize;
    public GameObject SlashImage;

    //public int ShieldLevel;
    public float InterruptedTime;
    public float InterruptedSpeed;
    public float InterruptedMoveTime;

    public float TacticDistance;
    public float DangerDistance;
    public float ChaseForAttackDistance;
    public float TacticDecisionInterval;
    public float AttackDecisionChance;
    public float ShortDisSingleAttackChance;
    public float LongDisSingleAttackChance;

    public int MaxStamina;
    public int MaxPatience;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
