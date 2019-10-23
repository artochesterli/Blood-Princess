using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public float InputSaveTime;

    public float MaxSpeed;
    public float GroundAcceleration;
    public float GroundDeceleration;
    public float AirAcceleration;
    public float AirDeceleration;
    public float NormalGravity;
    public float JumpSpeed;
    public float JumpHoldingTime;

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

    public float InvulnerableAnticipationTime;
    public float InvulnerableTime;
    public float InvulnerableRecoveryTime;
    public int InvulberableEnergyCost;
    public float InvulnerableExplosionStartRadius;
    public float InvulnerableExplosionEndRadius;
    public float InvulnerableExplosionTime;
    public int InvulnerableExplosionDamage;

    public float InterruptedTime;
    public float InterruptedMoveTime;
    public float InterruptedSpeed;
    public float InterruptedSpeedX;

    public float RollAnticipationTime;
    public float RollTime;
    public float RollRecoveryTime;
    public float RollSpeed;

    public LayerMask NormalIgnoredLayers;
    public LayerMask RollIgnoredLayers;

    public float ClimbPlatformTime;
    public LayerMask PassablePlatformLayer;

    public float ClimbLadderSpeed;
    public LayerMask LadderLayer;

    public LayerMask EnemyLayer;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
