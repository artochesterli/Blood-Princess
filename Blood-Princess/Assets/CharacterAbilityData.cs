using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityData : MonoBehaviour
{
    public float NormalSlashAnticipationTime;
    public float NormalSlashStrikeTime;
    public float NormalSlashRecoveryTime;
    public float NormalSlashStepForwardSpeed;
    public int NormalSlashDamage;
    public int NormalSlashInterruptLevel;
    public GameObject NormalSlashImage;
    public Vector2 NormalSlashOffset;
    public Vector2 NormalSlashHitBoxSize;
    public bool NormalSlashAirUsable;

    public string BloodSlashName;
    public float BloodSlashAnticipationTime;
    public float BloodSlashStrikeTime;
    public float BloodSlashRecoveryTime;
    public float BloodSlashStepForwardSpeed;
    public int BloodSlashDamage;
    public int BloodSlashEnergyCost;
    public GameObject BloodSlashImage;
    public Vector2 BloodSlashOffset;
    public Vector2 BloodSlashHitBoxSize;
    public bool BloodSlashAirUsable;

    public string DeadSlashName;
    public float DeadSlashAnticipationTime;
    public float DeadSlashStrikeTime;
    public float DeadSlashRecoveryTime;
    public int DeadSlashDamage;
    public int DeadSlashEnergyCost;
    public GameObject DeadSlashImage;
    public Vector2 DeadSlashOffset;
    public Vector2 DeadSlashHitBoxSize;
    public bool DeadSlashAirUsable;

    public int MaxBattleArtEnhancementNumber;
    public int MaxPassiveSkillNumber;

    public string CriticalEyeName;
    public int CriticalEyeBonus;
    public float CriticalEyeBattleArtBonusUnit;
    public int CriticalEyeMaxBattleArtBonusNumber;

    public string HarmonyName;
    public int HarmonyHealAmountLv1;
    public int HarmonyHealAmountLv2;
    public int HarmonyHealNoEnergyExtraAmount;

}
