using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulWarriorData : MonoBehaviour
{
    public int MaxHP;

    public float NormalMoveSpeed;
    public float AttackStepForwardSpeed;

    public float SlashAnticipationTime;
    public float SlashStrikeTime;
    public float SlashRecoveryTime;

    public float MagicAnticipationTime;
    public float MagicStrikeTime;
    public float MagicRecoveryTime;

    public int SlashDamage;
    public int MagicDamage;

    public float BlinkPrepareTime;

    public float SlashAvailableHitBoxPercentage;
    public Vector2 SlashOffset;
    public Vector2 SlashHitBoxSize;
    public GameObject SlashImage;

    public float MagicUseableDis;
    public float MagicPredictionDis;
    public Vector2 MagicHitBoxSize;
    public GameObject MagicPrefab;

    public float KnockedBackTime;
    public float KnockedBackSpeed;
    public float OffBalanceBackTime;
    public float OffBalanceBackSpeed;
    public float OffBalanceStayTime;

    public int OffBalanceInterruptLevel;


    public float AttackCoolDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
