﻿using System.Collections;
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

public class PlayerHitEnemy : GameEvent
{
    public CharacterAttackInfo Attack;
    public GameObject Enemy;

    public PlayerHitEnemy(CharacterAttackInfo attack, GameObject enemy)
    {
        Attack = attack;
        Enemy = enemy;
    }
}

public class PlayerEquipEnhancement : GameEvent
{
    public BattleArtEnhancement Enhancement;
    public PlayerEquipEnhancement(BattleArtEnhancement enhancement)
    {
        Enhancement = enhancement;
    }
}

public class PlayerUnequipEnhancement : GameEvent
{
    public BattleArtEnhancement Enhancement;
    public PlayerUnequipEnhancement(BattleArtEnhancement enhancement)
    {
        Enhancement = enhancement;
    }
}

public class PlayerEquipPassiveAbility : GameEvent
{
    public CharacterPassiveAbility PassiveAbility;
    public PlayerEquipPassiveAbility(CharacterPassiveAbility ability)
    {
        PassiveAbility = ability;
    }
}

public class PlayerUnequipPassiveAbility : GameEvent
{
    public CharacterPassiveAbility PassiveAbility;
    public PlayerUnequipPassiveAbility(CharacterPassiveAbility ability)
    {
        PassiveAbility = ability;
    }
}

public class PlayerUpgradeEnhancement : GameEvent
{
    public BattleArtEnhancement Enhancement;
    public PlayerUpgradeEnhancement(BattleArtEnhancement enhancement)
    {
        Enhancement = enhancement;
    }
}

public class PlayerDowngradeEnhancement : GameEvent
{
    public BattleArtEnhancement Enhancement;
    public PlayerDowngradeEnhancement(BattleArtEnhancement enhancement)
    {
        Enhancement = enhancement;
    }
}

public class PlayerUpgradePassiveAbility : GameEvent
{
    public CharacterPassiveAbility PassiveAbility;
    public PlayerUpgradePassiveAbility(CharacterPassiveAbility ability)
    {
        PassiveAbility = ability;
    }
}

public class PlayerDowngradePassiveAbility : GameEvent
{
    public CharacterPassiveAbility PassiveAbility;
    public PlayerDowngradePassiveAbility(CharacterPassiveAbility ability)
    {
        PassiveAbility = ability;
    }
}

