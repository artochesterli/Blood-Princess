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
    public float ChaseAttackRecoveryTime;
    public int Damage;

    public float AttackAvailableHitBoxPercentage;

    public Vector2 SingleAttackOffset;
    public Vector2 SingleAttackHitBoxSize;
    public Vector2 DoubleAttackOffset;
    public Vector2 DoubleAttackHitBoxSize;
    public GameObject SlashImage;
    public GameObject SingleSlashImage;
    public GameObject DoubleSlashImage;

    public float KnockedBackTime;
    public float KnockedBackSpeed;

    public float OffBalanceBackTime;
    public float OffBalanceBackSpeed;
    public float OffBalanceStayTime;

    public int OffBalanceInterruptLevel;

    public float KnockedRecoveryTime;
    public float GetHitOnBackKnockedTime;

    public float AttackCoolDown;

    public float MinChaseAttackChaseDistance;
    public float MaxChaseAttackChaseDistance;

    public float SingleAttackChance;
    public float DoubleAttackChance;

    public GameObject BlinkEffect;
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
