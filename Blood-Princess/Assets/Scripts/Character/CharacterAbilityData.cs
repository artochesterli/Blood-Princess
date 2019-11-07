using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityData : MonoBehaviour
{
    public float NormalSlashAnticipationTime;
    public float NormalSlashStrikeTime;
    public float NormalSlashRecoveryTime;
    public float NormalSlashStepForwardSpeed;
    public int NormalSlashBaseDamage;
    public int NormalSlashEnergyGain;
    public float NormalSlashCriticalEyeDamageBonus;
    public int NormalSlashCriticalEyeEnergyGainBonus;
    public GameObject NormalSlashImage;
    public Vector2 NormalSlashOffset;
    public Vector2 NormalSlashHitBoxSize;
    public bool NormalSlashAirUsable;

    public float SpiritSlashAnticipationTime;
    public float SpiritSlashStrikeTime;
    public float SpiritSlashRecoveryTime;
    public float SpiritSlashParriedRecoveryTime;
    public float SpiritSlashParriedAnticipation;
    public float SpiritSlashStepBackSpeed;
    public float SpiritSlashStepBackTime;
    public float SpiritSlashStepForwardSpeed;
    public int SpiritSlashBaseDamage;
    public float SpiritSlashFullEnergyDamageBonus;
    public float SpiritSlashHeal;
    public GameObject SpiritSlashImage;
    public Vector2 SpiritSlashOffset;
    public Vector2 SpiritSlashHitBoxSize;
    public bool SpiritSlashAirUseable;

    public int MaxBattleArtEnhancementNumber;
    public int MaxPassiveSkillNumber;

}
