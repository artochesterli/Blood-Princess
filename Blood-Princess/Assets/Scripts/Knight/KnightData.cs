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
    public float ChaseAttackAnticipationTime;
    public float ChaseAttackSpeed;

    public float AttackTime;
    public float SingleAttackRecoveryTime;
    public float DoubleAttackFirstRecoveryTime;
    public float DoubleAttackSecondRecoveryTime;
    public int Damage;

    public float AttackAvailableHitBoxPercentage;
    public Vector2 AttackOffset;
    public Vector2 AttackHitBoxSize;
    public GameObject SlashImage;

    public float KnockedBackTime;
    public float KnockedBackSpeed;

    public float KnockedRecoveryTime;
    public float GetHitOnBackKnockedTime;

    public float AttackCoolDown;

    public float MinChaseAttackChaseDistance;
    public float MaxChaseAttackChaseDistance;

    public float SingleAttackChance;
    public float DoubleAttackChance;

    public float BlinkChance;

    public float BlinkPrepareTime;
    public float BlinkForChaseAttackDis;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
