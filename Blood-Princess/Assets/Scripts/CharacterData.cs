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

    public float NormalSlashAnticipationTime;
    public float NormalSlashStrikeTime;
    public float NormalSlashRecoveryTime;
    public float NormalSlashStepForwardSpeed;
    public int NormalSlashDamage;
    public Vector2 NormalSlashOffset;
    public Vector2 NormalSlashHitBoxSize;

    public float BloodSlashAnticipationTime;
    public float BloodSlashStrikeTime;
    public float BloodSlashRecoveryTime;
    public float BloodSlashStepForwardSpeed;
    public int BloodSlashDamage;
    public int BloodSlashEnergyCost;
    public Vector2 BloodSlashOffset;
    public Vector2 BloodSlashHitBoxSize;

    public float DeadSlashAnticipationTime;
    public float DeadSlashStrikeTime;
    public float DeadSlashRecoveryTime;
    public int DeadSlashDamage;
    public int DeadSlashHeal;
    public int DeadSlashEnergyCost;
    public Vector2 DeadSlashOffset;
    public Vector2 DeadSlashHitBoxSize;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
