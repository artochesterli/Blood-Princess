using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent { }

public class PlayerStartAttackAnticipation : GameEvent
{
    public CharacterAttackInfo Attack;
    public PlayerStartAttackAnticipation(CharacterAttackInfo attack)
    {
        Attack = attack;
    }
}

public class PlayerEndAttackAnticipation : GameEvent
{
    public CharacterAttackInfo Attack;
    public PlayerEndAttackAnticipation(CharacterAttackInfo attack)
    {
        Attack = attack;
    }
}

public class PlayerStartAttackRecovery : GameEvent
{
    public CharacterAttackInfo Attack;
    public List<GameObject> HitEnemies;
    public PlayerStartAttackRecovery(CharacterAttackInfo attack, List<GameObject> hitEnemies)
    {
        Attack = attack;
        HitEnemies = hitEnemies;
    }
}

public class PlayerEndAttackRecovery : GameEvent
{
    public CharacterAttackInfo Attack;
    public List<GameObject> HitEnemies;
    public PlayerEndAttackRecovery(CharacterAttackInfo attack, List<GameObject> hitEnemies)
    {
        Attack = attack;
        HitEnemies = hitEnemies;
    }
}

public class PlayerStartAttackStrike : GameEvent
{
    public CharacterAttackInfo Attack;
    public PlayerStartAttackStrike(CharacterAttackInfo attack)
    {
        Attack = attack;
    }
}

public class PlayerEndAttackStrike : GameEvent
{
    public CharacterAttackInfo Attack;
    public List<GameObject> HitEnemies;
    public PlayerEndAttackStrike(CharacterAttackInfo attack, List<GameObject> hitEnemies)
    {
        Attack = attack;
        HitEnemies = hitEnemies;
    }
}

public class PlayerStartRollAnticipation : GameEvent
{

}

public class PlayerEndRollAnticipation : GameEvent
{

}

public class PlayerStartRoll : GameEvent
{

}

public class PlayerEndRoll : GameEvent
{

}

public class PlayerStartParry : GameEvent
{

}

public class PlayerEndParry : GameEvent
{

}



public class PlayerHitEnemy : GameEvent
{
    public CharacterAttackInfo OriginalAttack;
    public CharacterAttackInfo UpdatedAttack;
    public GameObject Enemy;

    public PlayerHitEnemy(CharacterAttackInfo originalattack, CharacterAttackInfo updatedattack, GameObject enemy)
    {
        OriginalAttack = originalattack;
        UpdatedAttack = updatedattack;
        Enemy = enemy;
    }
}

public class PlayerBreakEnemyShield : GameEvent
{
    public CharacterAttackInfo Attack;
    public GameObject Enemy;

    public PlayerBreakEnemyShield(CharacterAttackInfo attack, GameObject enemy)
    {
        Attack = attack;
        Enemy = enemy;
    }
}

public class PlayerKillEnemy : GameEvent
{
    public CharacterAttackInfo Attack;
    public GameObject Enemy;

    public PlayerKillEnemy(CharacterAttackInfo attack, GameObject enemy)
    {
        Attack = attack;
        Enemy = enemy;
    }
}

public class PlayerGetHit : GameEvent
{
    public EnemyAttackInfo EnemyAttack;
    public bool PlayerInRollInvulnerability;
    public PlayerGetHit(EnemyAttackInfo attack, bool inroll)
    {
        EnemyAttack = attack;
        PlayerInRollInvulnerability = inroll;
    }
}

public class PlayerDied : GameEvent
{
    public PlayerDied()
    {

    }
}

public class PlayerGetMoney : GameEvent
{
    public int Value;
    public PlayerGetMoney(int value)
    {
        Value = value;
    }
}

public class PlayerPickUpAbility : GameEvent
{
    public CharacterAbility Ability;
    public PlayerPickUpAbility(CharacterAbility ability)
    {
        Ability = ability;
    }
}

public class PlayerEquipBattleArt : GameEvent
{
    public BattleArt ThisBattleArt;
    public PlayerEquipBattleArt(BattleArt b)
    {
        ThisBattleArt = b;
    }

}

public class PlayerEquipPassiveAbility : GameEvent
{
    public PassiveAbility ThisPassiveAbility;
    public int Index;
    public PlayerEquipPassiveAbility(PassiveAbility p, int index)
    {
        ThisPassiveAbility = p;
        Index = index;
    }
}

public class PlayerUnequipBattleArt : GameEvent
{
    public BattleArt ThisBattleArt;
    public PlayerUnequipBattleArt(BattleArt b)
    {
        ThisBattleArt = b;
    }
}

public class PlayerUnequipPassiveAbility : GameEvent
{
    public PassiveAbility ThisPassiveAbility;
    public int Index;
    public PlayerUnequipPassiveAbility(PassiveAbility p, int index)
    {
        ThisPassiveAbility = p;
        Index = index;
    }
}

