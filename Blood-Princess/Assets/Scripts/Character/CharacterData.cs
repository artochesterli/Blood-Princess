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

    public int MaxHP;
    public int MaxEnergy;

    public List<int> SealSpotEnergyCap;

    public float InterruptedTime;
    public float InterruptedSpeed;

    public GameObject RollEffect;
    public float RollAnticipationTime;
    public float RollTime;
    public float RollRecoveryTime;
    public float RollCoolDown;
    public float RollSpeed;

    public float PushedOutSpeed;

    public LayerMask NormalIgnoredLayers;
    public LayerMask RollIgnoredLayers;

    public float ClimbPlatformTime;
    public float ClimbPlatformJumpOverSpeed;
    public LayerMask PassablePlatformLayer;

    public float ClimbLadderSpeed;
    public LayerMask LadderLayer;

    public LayerMask EnemyLayer;
    public LayerMask DoorLayer;

    public int BattleArtTypeNumber;
    public int PassiveAbilityTypeNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
