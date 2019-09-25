using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterAttackState
{
    Free,
    LightAttackAnticipation,
    LightAttackRecovery,
    HeavyAttackAnticipation,
    HeavyAttackRecovery,
}

public enum CharacterState
{
    Normal,
    Hit
}

public enum CharacterGravityState
{
    Normal,
    Ignore
}

public class CharacterStateManager : MonoBehaviour
{
    public CharacterState State;
    public CharacterAttackState AttackState;
    public CharacterGravityState GravityState;

    // Start is called before the first frame update
    void Start()
    {
        State = CharacterState.Normal;
        AttackState = CharacterAttackState.Free;
        GravityState = CharacterGravityState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
