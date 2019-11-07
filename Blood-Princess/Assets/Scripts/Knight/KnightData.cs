using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightData : MonoBehaviour
{
    public int MaxHP;

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

    public float AttackTime;
    public float SingleAttackRecoveryTime;
    public float DoubleAttackFirstRecoveryTime;
    public float DoubleAttackSecondRecoveryTime;
    public int Damage;

    public Vector2 AttackOffset;
    public Vector2 AttackHitBoxSize;
    public GameObject SlashImage;

    public float KnockedBackTime;
    public float KnockedBackSpeed;

    public float RecoveryKnockedBackStunTime;

    public float AttackCoolDown;

    public float MinChaseAttackChaseDistance;
    public float MaxChaseAttackChaseDistance;

    public float SingleAttackChance;

    public float FirstGetHitAttackCoolDown;

    public float CharacterSpiritSlashAttackCoolDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
