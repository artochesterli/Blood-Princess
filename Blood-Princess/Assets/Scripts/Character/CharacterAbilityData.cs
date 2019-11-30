using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityData : MonoBehaviour
{
    public int BasePower;

    public float SlashAnticipationTime;
    public float SlashStrikeTime;
    public float SlashRecoveryTime;
    public float SlashStepForwardSpeed;
    public int SlashPotency;
    public int SlashInterruptLevel;

    public int SlashEnergyGain;
    public int SlashEnergyAwakenGain;

    public GameObject SlashEffect;
    public Vector2 SlashOffset;
    public Vector2 SlashHitBoxSize;
    public Vector2 SlashEffectOffset;

    public float ParryAnticipationTime;
    public float ParryRecoveryTime;
    public float ParryEffectTime;
    public int ParryEnergyGain;

    public float PowerSlashAnticipationTime;
    public float PowerSlashStrikeTime;
    public float PowerSlashRecoveryTime;
    public float PowerSlashStepForwardSpeed;
    public int PowerSlashPotency;
    public int PowerSlashInterruptLevel;

    public int PowerSlashPotencyIncrement;
    public int PowerSlashMaxIncrement_Normal;
    public int PowerSlashMaxIncrement_Upgraded;
    public int PowerSlashMaxIncrementEnergyGain;

    public GameObject PowerSlashEffect;
    public Vector2 PowerSlashEffectOffset;
    public Vector2 PowerSlashOffset;
    public Vector2 PowerSlashHitBoxSize;


    public float CrossSlashAnticipationTime;
    public float CrossSlashStrikeTime;
    public float CrossSlashRecoveryTime;
    public float CrossSlashStepForwardSpeed;
    public int CrossSlashStrikeNumber_Normal;
    public int CrossSlashStrikeNumber_Upgraded;
    public int CrossSlashPotency;
    public int CrossSlashInterruptLevel;

    public int CrossSlashAwakenPotencyBonus;
    public int CrossSlashSealBreakBonus;

    public GameObject CrossSlashEffect;
    public Vector2 CrossSlashEffectOffset;
    public Vector2 CrossSlashOffset;
    public Vector2 CrossSlashHitBoxSize;

    public int HarmonyHealPotency;

    public List<int> SpiritMasterExtraEnergyGainListWithSeal;

    public int UltimateAwakeningExtraPower;

    public int CriticalEyeExtraPotency;
    public int CriticalEyePotencyRequired;

    public int BattleArtMasterExtraPotency;

    public int OneMindSlashPotencyIncrement;
    public int OneMindMaxIncrement;

    public int DancerPotency;
    public int DancerInterruptLevel;
    public Vector2 DancerHitBoxSize;

}
