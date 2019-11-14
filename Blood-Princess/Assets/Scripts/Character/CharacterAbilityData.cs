using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityData : MonoBehaviour
{

    public float SlashAnticipationTime;
    public float SlashStrikeTime;
    public float SlashRecoveryTime;
    public float SlashStepForwardSpeed;
    public int SlashBaseDamage;
    public float SlashCriticalEyeDamageBonus;
    public int SlashEnergyGain;
    public int SlashAdvancedEnergyGainInCriticalEye;

    public GameObject SlashImageCriticalEye;
    public GameObject SlashImage;
    public Vector2 SlashOffset;
    public Vector2 SlashHitBoxSize;

    public float ParryAnticipationTime;
    public float ParryRecoveryTime;
    public float ParryEffectTime;
    public int ParryEnergyGain;

    public float SpiritSlashAnticipationTime;
    public float SpiritSlashStrikeTime;
    public float SpiritSlashRecoveryTime;
    public float SpiritSlashStepBackSpeed;
    public float SpiritSlashStepBackTime;
    public float SpiritSlashStepForwardSpeed;
    public int SpiritSlashBaseDamage;
    public float SpiritSlashBackStabDamageBonus;
    public int SpiritSlashAdvancedEnergyGain;

    public GameObject SpiritSlashImage;
    public Vector2 SpiritSlashOffset;
    public Vector2 SpiritSlashHitBoxSize;

}
