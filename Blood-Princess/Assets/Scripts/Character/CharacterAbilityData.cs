using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityData : MonoBehaviour
{
    public int BaseDamage;

    public float SlashAnticipationTime;
    public float SlashStrikeTime;
    public float SlashRecoveryTime;
    public float SlashStepForwardSpeed;
    public float SlashDamageFactor;

    public int SlashEnergyGain;

    public float SlashEffectPreGenerateTime;
    public GameObject SlashEffect;
    public GameObject SlashImage;
    public Vector2 SlashOffset;
    public Vector2 SlashHitBoxSize;

    public float ParryAnticipationTime;
    public float ParryRecoveryTime;
    public float ParryEffectTime;
    public int ParryEnergyGain;

    public float PowerSlashAnticipationTime;
    public float PowerSlashStrikeTime;
    public float PowerSlashRecoveryTime;
    public float PowerSlashStepForwardSpeed;
    public float PowerSlashDamageFactor;
    public int PowerSlashAdvancedEnergyGain;

    public float PowerSlashDamageEnhancementFactor; 
    public int PowerSlashMaxEnhancementTime;
    public float PowerSlashExecutionHPCondition;
    public float PowerSlashExecutionDamageFactor;

    public GameObject PowerSlashImage;
    public Vector2 PowerSlashOffset;
    public Vector2 PowerSlashHitBoxSize;

    public float HarmonySlashAnticipationTime;
    public float HarmonySlashStrikeTime;
    public float HarmonySlashRecoveryTime;
    public float HarmonySlashStepBackSpeed;
    public float HarmonySlashStepBackTime;
    public float HarmonySlashStepForwardSpeed;
    public float HarmonySlashDamageFactor;
    public int HarmonySlashAdvancedEnergyGain;

    public float HarmonySlashFullAdvancedEnergyDamageBonus;
    public float HarmonySlashSlashDamageBonusFactor;
    public int HarmonySlashMaxSlashDamageBonusTime;

    public GameObject HarmonySlashImage;
    public Vector2 HarmonySlashOffset;
    public Vector2 HarmonySlashHitBoxSize;

    public float OneMindDamageEnhancementFactor;
    public int OneMindMaxDamageEnhancementTime;

    public float DancerDamageFactor;
    public Vector2 DancerHitBoxSize;

    public int InsanityAdvancedEnergyGain;

}
