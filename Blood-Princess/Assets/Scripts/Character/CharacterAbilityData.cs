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
    public float NormalSlashDamageBonusPerEnergy;
    public GameObject NormalSlashImage;
    public Vector2 NormalSlashOffset;
    public Vector2 NormalSlashHitBoxSize;
    public bool NormalSlashAirUsable;

    public float SpiritSlashAnticipationTime;
    public float SpiritSlashStrikeTime;
    public float SpiritSlashRecoveryTime;
    public float SpiritSlashStepBackSpeed;
    public float SpiritSlashStepBackTime;
    public float SpiritSlashStepForwardSpeed;
    public List<int> SpiritSlashDamageList;
    public float SpiritSlashHeal;
    public GameObject SpiritSlashImage;
    public Vector2 SpiritSlashOffset;
    public Vector2 SpiritSlashHitBoxSize;
    public bool SpiritSlashAirUseable;



    public string BloodSlashName;
    public float BloodSlashAnticipationTime;
    public float BloodSlashStrikeTime;
    public float BloodSlashRecoveryTime;
    public float BloodSlashStepForwardSpeed;
    public int BloodSlashShieldBreak;
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
    public int DeadSlashShieldBreak;
    public int DeadSlashDamage;
    public int DeadSlashEnergyCost;
    public GameObject DeadSlashImage;
    public Vector2 DeadSlashOffset;
    public Vector2 DeadSlashHitBoxSize;
    public bool DeadSlashAirUsable;

    public int MaxBattleArtEnhancementNumber;
    public int MaxPassiveSkillNumber;

    public string CriticalEyeName;
    public int MaxCriticalEyeLevel;
    public float CriticalEyeBonusUnit;
    public int CriticalEyeMaxBonusNumberLv1;
    public int CriticalEyeMaxBonusNumberLv2;

    public string HarmonyName;
    public int MaxHarmonyLevel;
    public float HarmonyHealLv1;
    public float HarmonyHealLv2;
    public int HarmonyEnergyRecovery;

    public string ShieldBreakerName;
    public int MaxShieldBreakerLevel;
    public float ShieldBreakerShieldTime;
    public int ShieldBreakerBonusLv1;
    public int ShieldBreakerBonusLv2;
    public float ShieldBreakerDamageBonus;

    public string ExecutionerName;
    public int MaxExecutionerLevel;
    public int ExecutionerEnergyRecovery;
    public float ExecutionerAnticipationCut;

    public string DancerName;
    public int MaxDancerLevel;
    public int DancerDamage;
    public int DancerShieldBreak;
    public Vector2 DancerHitBoxSize;

    public string AccurateBladeName;
    public int MaxAccurateBladeLevel;
    public float AccurateBladeBonusUnit;
    public int AccurateBladeMaxBonusNumber;
    public float AccurateBladeHeal;

    public string CursedBladeName;
    public int MaxCursedBladeLevel;
    public float CursedBladeExtraDamageRecieved;
    public float CursedBladeBattleArtDamageDecrease;
    public float CursedBladeNormalSlashDamageIncrease;

}
