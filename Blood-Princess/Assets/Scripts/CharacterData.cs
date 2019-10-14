using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public float MaxSpeed;
    public float GroundAcceleration;
    public float GroundDeceleration;
    public float AirAcceleration;
    public float AirDeceleration;
    public float NormalGravity;
    public float JumpSpeed;
    public float JumpHoldingTime;
    public float WallHittingPause;

    public float GetHitTime;
    public float GetHitSpeed;

    public int MaxHP;
    public int MaxEnergy;

    public float LightAttackAnticipation;
    public float LightAttackRecovery;
    public float LightAttackTime;
    public int LightAttackDamage;
    public int LightAttackEnergyGain;
    public Vector2 LightAttackOffset;
    public Vector2 LightAttackHitBoxSize;

    public float HeavyAttackLongAnticipation;
    public float HeavyAttackLongRecovery;
    public float HeavyAttackShortAnticipation;
    public float HeavyAttackShortRecovery;
    public float HeavyAttackTime;
    public int HeavyAttackBaseDamage;
    public int Lv2HeavyAttackEnergyThreshold;
    public int Lv3HeavyAttackEnergyThreshold;
    public int HeavyAttackDamage_Lv2;
    public int HeavyAttackDamage_Lv3;
    public int HeavyAttackMaxDamage;
    public int HeavyAttackMaxDamageBonus;
    public int DrainBonus;
    public Vector2 HeavyAttackOffset;
    public Vector2 HeavyAttackHitBoxSize;

    public int MaxEnergyOrb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
