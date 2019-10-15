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

    public float BlockDamageDeduction;
    public float BlockInvulnerableTime;

    public float LightAttackAnticipation;
    public float LightAttackRecovery;
    public float LightAttackTime;
    public int LightAttackDamage;
    public int LightAttackEnergyGain;
    public Vector2 LightAttackOffset;
    public Vector2 LightAttackHitBoxSize;

    public float Lv1HeavyAttackAnticipation;
    public float Lv1HeavyAttackRecovery;
    public float Lv1HeavyAttackTime;
    public int Lv1HeavyAttackDamage;

    public float Lv2HeavyAttackAnticipation;
    public float Lv2HeavyAttackRecovery;
    public float Lv2HeavyAttackTime;
    public int Lv2HeavyAttackDamage;

    public Vector2 HeavyAttackOffset;
    public Vector2 HeavyAttackHitBoxSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
