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
    public float SlashEnergyGain;
    public float SlashEnergyLostInCriticalEye;

    public GameObject SlashImage;
    public Vector2 SlashOffset;
    public Vector2 SlashHitBoxSize;

    public float ParryAnticipationTime;
    public float ParryRecoveryTime;
    public float ParryEffectTime;
    public float ParryEnergyGain;

    public float SpiritSlashAnticipationTime;
    public float SpiritSlashStrikeTime;
    public float SpiritSlashRecoveryTime;
    public float SpiritSlashStepBackSpeed;
    public float SpiritSlashStepBackTime;
    public float SpiritSlashStepForwardSpeed;
    public int SpiritSlashBaseDamage;
    public float SpiritSlashBackStabDamageBonus;
    public float SpiritSlashEnergyCost;

    public GameObject SpiritSlashImage;
    public Vector2 SpiritSlashOffset;
    public Vector2 SpiritSlashHitBoxSize;

}
