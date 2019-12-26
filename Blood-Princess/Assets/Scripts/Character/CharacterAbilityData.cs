using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityData : MonoBehaviour
{
    public int PowerUpgradeAmount;
    public int HPUpgradeAmount;

    public int BasePower;

    public float SlashAnticipationTime;
    public float SlashStrikeTime;
    public float SlashRecoveryTime;
    public float SlashStepForwardSpeed;
    public int SlashPotency;
    public int SlashAwakenPotency;
    public int SlashInterruptLevel;

    public int SlashEnergyGain;

    public GameObject SlashEffect;
    public GameObject SlashHitEffect;

    public Vector2 SlashOffset;
    public Vector2 SlashHitBoxSize;
    public Vector2 SlashEffectOffset;

    public float PowerSlashAnticipationTime;
    public float PowerSlashStrikeTime;
    public float PowerSlashRecoveryTime;
    public float PowerSlashStepForwardSpeed;
    public int PowerSlashPotency;
    public int PowerSlashInterruptLevel;

    public int PowerSlashAwakenPotencyBonus_Normal;
    public int PowerSlashAwakenPotencyBonus_Upgraded;
    public int PowerSlashEnergyGain;

    public GameObject PowerSlashEffect;
    public GameObject PowerSlashHitEffect;

    public Vector2 PowerSlashEffectOffset;
    public Vector2 PowerSlashOffset;
    public Vector2 PowerSlashHitBoxSize;




    public float CrossSlashAnticipationTime;
    public float CrossSlashStrikeTime;
    public float CrossSlashRecoveryTime;
    public float CrossSlashStepForwardSpeed;
    public int CrossSlashStrikeNumber_Normal;
    public int CrossSlashStrikeNumber_Awaken;
    public List<int> CrossSlashPotencyList;
    public int CrossSlashInterruptLevel;

    public int CrossSlashAwakenPotencyBonus;
    public int CrossSlashSealBreakBonus;

    public GameObject CrossSlashEffect;
    public GameObject CrossSlashEffect_Upgrade;
    public GameObject CrossSlashHitEffect;

    public Vector2 CrossSlashEffectOffset;
    public Vector2 CrossSlashOffset;
    public Vector2 CrossSlashHitBoxSize;

    public Vector2 CrossSlashEffectOffset_Upgrade;
    public Vector2 CrossSlashOffset_Upgrade;
    public Vector2 CrossSlashHitBoxSize_Upgrade;

    public int HarmonyHealPotency;

    public int SpiritMasterNormalMaxEnergy;

    public int UltimateAwakeningExtraPotency;

    public int CriticalEyeExtraPotency;
    public int CriticalEyePotencyRequired;

    public int BattleArtMasterExtraPotency;

    public int OneMindSlashPotencyIncrement;
    public int OneMindMaxIncrement;

    public GameObject DancerHitEffect;
    public int DancerPotency;
    public int DancerInterruptLevel;
    public Vector2 DancerHitBoxSize;

}
