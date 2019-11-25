﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AttackInfo
{
    public GameObject Source;
}

public class CharacterAttackInfo : AttackInfo
{
    public CharacterAttackType Type;
    public DamageType DamType;
    public Direction Dir;
    public int Damage;
    public int BaseDamage;
    public int OriginalDamage;

    public float AnticipationTime;
    public float BaseAnticipationTime;
    public float StrikeTime;
    public float BaseStrikeTime;
    public float RecoveryTime;
    public float BaseRecoveryTime;

    public Vector2 HitBoxOffset;
    public Vector2 BaseHitBoxOffset;
    public Vector2 HitBoxSize;
    public Vector2 BaseHitBoxSize;

    public BattleArt ThisBattleArt;

    public CharacterAttackInfo(GameObject source, CharacterAttackType type, DamageType damtype, Direction dir, int damage, int basedamage, int originaldamage,
        Vector2 offset, Vector2 baseoffset, Vector2 size, Vector2 basesize, float anticipation = 0, float baseanticipation = 0, float strike = 0, float basestrike = 0, float recovery = 0, float baserecovery = 0, BattleArt thisbattleart = null)
    {
        Source = source;
        DamType = damtype;
        Type = type;
        Dir = dir;
        Damage = damage;
        BaseDamage = basedamage;
        OriginalDamage = originaldamage;

        AnticipationTime = anticipation;
        BaseAnticipationTime = baseanticipation;
        StrikeTime = strike;
        BaseStrikeTime = basestrike;
        RecoveryTime = recovery;
        BaseRecoveryTime = baserecovery;

        HitBoxOffset = offset;
        BaseHitBoxOffset = baseoffset;
        HitBoxSize = size;
        BaseHitBoxSize = basesize;

        ThisBattleArt = thisbattleart;
    }

    public CharacterAttackInfo(CharacterAttackInfo Attack)
    {
        Source = Attack.Source;
        Type = Attack.Type;
        DamType = Attack.DamType;
        Dir = Attack.Dir;
        Damage = Attack.Damage;
        BaseDamage = Attack.BaseDamage;
        OriginalDamage = Attack.OriginalDamage;

        AnticipationTime = Attack.AnticipationTime;
        BaseAnticipationTime = Attack.BaseAnticipationTime;
        StrikeTime = Attack.StrikeTime;
        BaseStrikeTime = Attack.BaseStrikeTime;
        RecoveryTime = Attack.RecoveryTime;
        BaseRecoveryTime = Attack.BaseRecoveryTime;

        HitBoxOffset = Attack.HitBoxOffset;
        BaseHitBoxOffset = Attack.BaseHitBoxOffset;
        HitBoxSize = Attack.HitBoxSize;
        BaseHitBoxSize =  Attack.BaseHitBoxSize;

        ThisBattleArt = Attack.ThisBattleArt;
    }
}

public class EnemyAttackInfo : AttackInfo
{
    public bool Right;
    public int Damage;
    public int BaseDamage;
    public Vector2 HitBoxOffset;
    public Vector2 HitBoxSize;
    public EnemyAttackInfo(GameObject source, bool right,int damage, int basedamage, Vector2 offset,Vector2 size)
    {
        Source = source;
        Right = right;
        Damage = damage;
        BaseDamage = basedamage;
        HitBoxOffset = offset;
        HitBoxSize = size;
    }
}

public abstract class CharacterAbility
{
    public string name;
    public Sprite Icon;
    public int Level;
}

public abstract class BattleArt : CharacterAbility
{
    public BattleArtType Type;
}

public abstract class PassiveAbility : CharacterAbility
{
    public PassiveAbilityType Type;
}

public class PowerSlash : BattleArt
{
    public PowerSlash(int level = 1)
    {
        name = "Power Slash";
        Type = BattleArtType.PowerSlash;
        Level = level;
    }
}

public class HarmonySlash : BattleArt
{
    public HarmonySlash(int level = 1)
    {
        name = "Harmony Slash";
        Type = BattleArtType.HarmonySlash;
        Level = level;
    }
}

public class SpiritBolt : BattleArt
{
    public SpiritBolt(int level = 1)
    {
        name = "Spirit Bolt";
        Type = BattleArtType.SpiritBolt;
        Level = level;
    }
}

public class SpiritFall : BattleArt
{
    public SpiritFall(int level = 1)
    {
        name = "Spirit Fall";
        Type = BattleArtType.SpiritFall;
        Level = level;
    }
}

public class SpiritShadow : BattleArt
{
    public SpiritShadow(int level = 1)
    {
        name = "Spirit Shadow";
        Type = BattleArtType.SpiritShadow;
        Level = level;
    }
}

public class SlashArt : PassiveAbility
{
    public SlashArt(int level = 1)
    {
        name = "Slash Art";
        Type = PassiveAbilityType.SlashArt;
        Level = level;
    }
}

public class AssassinHeart : PassiveAbility
{
    public AssassinHeart(int level = 1)
    {
        name = "Assassin's Heart";
        Type = PassiveAbilityType.AssassinHeart;
        Level = level;
    }
}

public class SpellStrike : PassiveAbility
{
    public SpellStrike(int level = 1)
    {
        name = "Spell Strike";
        Type = PassiveAbilityType.SpellStrike;
        Level = level;
    }
}

public class OneMind : PassiveAbility
{
    public OneMind(int level = 1)
    {
        name = "One Mind";
        Type = PassiveAbilityType.OneMind;
        Level = level;
    }
}

public class Dancer : PassiveAbility
{
    public Dancer(int level = 1)
    {
        name = "Dancer";
        Type = PassiveAbilityType.Dancer;
        Level = level;
    }
}

public class StepMaster : PassiveAbility
{
    public StepMaster(int level = 1)
    {
        name = "Step Master";
        Type = PassiveAbilityType.StepMaster;
        Level = level;
    }
}

public class Insanity : PassiveAbility
{
    public Insanity(int level = 1)
    {
        name = "Insanity";
        Type = PassiveAbilityType.Insanity;
        Level = level;
    }
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
    None
}

public enum BattleArtType
{
    PowerSlash,
    HarmonySlash,
    SpiritBolt,
    SpiritFall,
    SpiritShadow

}

public enum PassiveAbilityType
{
    SlashArt,
    AssassinHeart,
    SpellStrike,
    OneMind,
    Dancer,
    StepMaster,
    Insanity

}

public enum DamageType
{
    Normal,
    Strike,
    Spell
}

public enum CharacterAttackType
{
    Slash,
    Dancer,
    PowerSlash,
    HarmonySlash,
    SpiritBolt,
    SpiritFall,
    SpiritShadow
}

public enum InputType
{
    Jump,
    Slash,
    Roll,
    BattleArt,
    Parry,
    FallPlatform

}

public class InputInfo
{
    public InputType Type;
    public float TimeCount;
    public float KeptTime;
    public bool WithDirection;
    public bool DirectionRight;

    public InputInfo(InputType type, bool withdirection, bool directionright, float time = 0)
    {
        Type = type;
        TimeCount = 0;
        WithDirection = withdirection;
        DirectionRight = directionright;
        KeptTime = time;
    }
}

public class CharacterAction : MonoBehaviour
{
    public float CurrentGravity;
    public GameObject AttachedPassablePlatform;
    public GameObject AttachedLadder;

    public float RollCoolDownTimeCount;

    public CharacterAttackInfo CurrentAttack;
    public List<GameObject> HitEnemies;

    public BattleArt EquipedBattleArt;
    public List<PassiveAbility> EquipedPassiveAbility;

    private FSM<CharacterAction> CharacterActionFSM;

    // Start is called before the first frame update
    void Start()
    {
        SetUpAbility();

        CharacterActionFSM = new FSM<CharacterAction>(this);
        CharacterActionFSM.TransitionTo<Stand>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRollCoolDown();
        CharacterActionFSM.Update();

    }


    // <summary>
    /// return if player is in those states
    /// </summary>
    /// <param name="StateName">Anticipation, Recovery, Strike</param>
    /// <returns></returns>
    public bool InState(string StateName)
    {
        switch (StateName)
        {
            case "Anticipation":
                return CharacterActionFSM.CurrentState.GetType().Equals(typeof(SlashAnticipation));
            case "Recovery":
                return CharacterActionFSM.CurrentState.GetType().Equals(typeof(SlashRecovery));
            case "Strike":
                return CharacterActionFSM.CurrentState.GetType().Equals(typeof(SlashStrike));
        }
        return false;
    }



    private void SetUpAbility()
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        EquipedBattleArt = null;

        EquipedPassiveAbility = new List<PassiveAbility>();

        EquipedPassiveAbility.Add(null);
        EquipedPassiveAbility.Add(null);
        EquipedPassiveAbility.Add(null);

    }

    private void UpdateRollCoolDown()
    {
        RollCoolDownTimeCount -= Time.deltaTime;
        if (RollCoolDownTimeCount < 0)
        {
            RollCoolDownTimeCount = 0;
        }
    }


}

public abstract class CharacterActionState : FSM<CharacterAction>.State
{
    protected GameObject Entity;

    protected GameObject SlashImage;
    protected List<Sprite> CurrentSpriteSeries;

    protected const float DetectPassablePlatformDis = 0.01f;

    public override void Init()
    {
        base.Init();
        Entity = Context.gameObject;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log(this.GetType().Name);

    }

    public override void Update()
    {
        base.Update();
        Entity.GetComponent<SpeedManager>().SelfSpeed.y -= Context.CurrentGravity * Time.deltaTime * 10;
        SetCharacterSprite();

    }



    protected bool CheckGetInterrupted()
    {
        if (Entity.GetComponent<IHittable>().Interrupted)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void SetCharacterSprite()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        if (Status.IsEnergyFull())
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[3];
        }
        else
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[0];
        }
    }

    protected bool HitEnemy(CharacterAttackInfo Attack, List<GameObject> EnemyHit)
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        Vector2 Offset = Attack.HitBoxOffset;
        Offset.x = Entity.transform.right.x * Offset.x;

        RaycastHit2D[] AllHits = Physics2D.BoxCastAll(Entity.transform.position + (Vector3)Offset, Attack.HitBoxSize, 0, Entity.transform.right, 0, Data.EnemyLayer);
        if (AllHits.Length > 0)
        {
            for (int i = 0; i < AllHits.Length; i++)
            {
                GameObject Enemy = AllHits[i].collider.gameObject;

                if (!EnemyHit.Contains(Enemy))
                {
                    CharacterAttackInfo UpdatedAttack = new CharacterAttackInfo(Attack);

                    EventManager.instance.Fire(new PlayerHitEnemy(Attack, UpdatedAttack, Enemy));

                    Enemy.GetComponent<IHittable>().OnHit(UpdatedAttack);
                    EnemyHit.Add(Enemy);
                }
            }

            return true;
        }
        else
        {
            return false;
        }

    }


    protected bool CharacterClimbPlatform()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        Vector2 TopPos = SpeedManager.GetTruePos() + (SpeedManager.BodyHeight / 2 + DetectPassablePlatformDis/2) * Vector2.up;
        Vector2 BottomPos = SpeedManager.GetTruePos();

        RaycastHit2D TopHit = Physics2D.BoxCast(TopPos, new Vector2(SpeedManager.BodyWidth, DetectPassablePlatformDis), 0, Vector2.up, 0, Data.PassablePlatformLayer);
        RaycastHit2D BottomHit = Physics2D.BoxCast(BottomPos, new Vector2(SpeedManager.BodyWidth, SpeedManager.BodyHeight), 0, Vector2.up, 0, Data.PassablePlatformLayer);


        if(!TopHit && BottomHit && !Context.AttachedPassablePlatform && Utility.InputUp())
        {
            Context.AttachedPassablePlatform = BottomHit.collider.gameObject;
            return true;
        }
        else
        {
            Context.AttachedPassablePlatform = null;
            return false;
        }
    }

    protected bool CharacterDownToLadder()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        float Thickness = 0.01f;

        RaycastHit2D DownHit = Physics2D.BoxCast(SpeedManager.GetTruePos() + (SpeedManager.BodyHeight/2 + Thickness/2)*Vector2.down, new Vector2(SpeedManager.BodyWidth, Thickness), 0, Vector2.down, 0, Data.LadderLayer);
        RaycastHit2D TopHit = Physics2D.BoxCast(SpeedManager.GetTruePos() + (SpeedManager.BodyHeight / 2 + Thickness / 2) * Vector2.up, new Vector2(SpeedManager.BodyWidth, Thickness), 0, Vector2.up, 0, Data.LadderLayer);


        if (!TopHit && DownHit && Utility.InputDown())
        {
            Context.AttachedLadder = DownHit.collider.gameObject;
            return true;
        }
        else
        {
            Context.AttachedLadder = null;
            return false;
        }
    }

    protected bool CharacterClimbLadder()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        RaycastHit2D Hit = Physics2D.BoxCast(SpeedManager.GetTruePos(), new Vector2(SpeedManager.BodyWidth, SpeedManager.BodyHeight), 0, Vector2.up, 0, Data.LadderLayer);

        if(Hit && !Context.AttachedLadder && Utility.InputUp())
        {
            GameObject Ladder = Hit.collider.gameObject;

            float LadderTop = Ladder.transform.position.y + (Ladder.GetComponent<BoxCollider2D>().offset.y + Ladder.GetComponent<BoxCollider2D>().size.y / 2) * Ladder.transform.localScale.y;
            float LadderBottom = Ladder.transform.position.y + (Ladder.GetComponent<BoxCollider2D>().offset.y - Ladder.GetComponent<BoxCollider2D>().size.y / 2) * Ladder.transform.localScale.y;
            float SelfTop = SpeedManager.GetTruePos().y + SpeedManager.BodyHeight / 2;
            float SelfBottom = SpeedManager.GetTruePos().y - SpeedManager.BodyHeight / 2;

            if(LadderTop >=SelfTop && LadderBottom <= SelfBottom)
            {
                Context.AttachedLadder = Ladder;
                return true;
            }
            else
            {
                Context.AttachedLadder = null;
                return false;
            }
        }
        else
        {
            Context.AttachedLadder = null;
            return false;
        }
    }

    protected void DetectPassablePlatform()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (SpeedManager.Ground && SpeedManager.Ground.CompareTag("PassablePlatform"))
        {
            float PlayerLeft = SpeedManager.GetTruePos().x - SpeedManager.BodyWidth / 2;
            float PlayerRight = SpeedManager.GetTruePos().x + SpeedManager.BodyWidth / 2;
            float GroundLeft = SpeedManager.Ground.transform.position.x + SpeedManager.Ground.transform.localScale.x * (SpeedManager.Ground.GetComponent<BoxCollider2D>().offset.x - SpeedManager.Ground.GetComponent<BoxCollider2D>().size.x / 2);
            float GroundRight = SpeedManager.Ground.transform.position.x + SpeedManager.Ground.transform.localScale.x * (SpeedManager.Ground.GetComponent<BoxCollider2D>().offset.x + SpeedManager.Ground.GetComponent<BoxCollider2D>().size.x / 2);

            var Data = Entity.GetComponent<CharacterData>();

            RaycastHit2D LeftHit = Physics2D.Raycast(SpeedManager.GetTruePos() + SpeedManager.BodyWidth / 2*Vector2.left + SpeedManager.BodyHeight / 2*Vector2.down, Vector2.down, 0.02f, Data.PassablePlatformLayer);
            RaycastHit2D RightHit = Physics2D.Raycast(SpeedManager.GetTruePos() + SpeedManager.BodyWidth / 2 * Vector2.right + SpeedManager.BodyHeight / 2 * Vector2.down, Vector2.down, 0.02f, Data.PassablePlatformLayer);

            if ((!LeftHit || LeftHit.collider.gameObject.CompareTag("PassablePlatform")) && (!RightHit || RightHit.collider.gameObject.CompareTag("PassablePlatform")))
            {
                Context.AttachedPassablePlatform = SpeedManager.Ground;
            }
            else
            {
                Context.AttachedPassablePlatform = null;
            }

            /*if (PlayerLeft >= GroundLeft && PlayerRight <= GroundRight)
            {
                Context.AttachedPassablePlatform = SpeedManager.Ground;
            }
            else
            {
                Context.AttachedPassablePlatform = null;
            }*/
        }
        else
        {
            Context.AttachedPassablePlatform = null;
        }

    }



    protected bool CheckFallPlatform(InputInfo Info)
    {
        DetectPassablePlatform();
        if(Context.AttachedPassablePlatform && Info.Type == InputType.FallPlatform)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

            var SpeedManager = Entity.GetComponent<SpeedManager>();
            var Data = Entity.GetComponent<CharacterData>();

            RaycastHit2D LeftHit = Physics2D.Raycast(SpeedManager.GetTruePos() + SpeedManager.BodyWidth / 2 * Vector2.left + SpeedManager.BodyHeight / 2 * Vector2.down, Vector2.down, 0.02f, Data.PassablePlatformLayer);
            RaycastHit2D RightHit = Physics2D.Raycast(SpeedManager.GetTruePos() + SpeedManager.BodyWidth / 2 * Vector2.right + SpeedManager.BodyHeight / 2 * Vector2.down, Vector2.down, 0.02f, Data.PassablePlatformLayer);

            if (LeftHit)
            {
                LeftHit.collider.GetComponent<ColliderInfo>().TopPassable = true;
                LeftHit.collider.gameObject.GetComponent<PassablePlatform>().Player = Entity;
            }
            if (RightHit)
            {
                RightHit.collider.GetComponent<ColliderInfo>().TopPassable = true;
                RightHit.collider.gameObject.GetComponent<PassablePlatform>().Player = Entity;
            }


            Context.AttachedPassablePlatform.GetComponent<ColliderInfo>().TopPassable = true;
            Context.AttachedPassablePlatform.GetComponent<PassablePlatform>().Player = Entity;
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void RectifyDirectionWithInput()
    {
        if(Utility.InputRight() && !Utility.InputLeft())
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if(!Utility.InputRight() && Utility.InputLeft())
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    protected void GroundStateReceiveImmediateInput(ref InputInfo ImmediateInput)
    {
        bool WithDirection = false;
        bool Right = false;
        if (Utility.InputRight() && !Utility.InputLeft())
        {
            WithDirection = true;
            Right = true;
        }
        else if (!Utility.InputRight() && Utility.InputLeft())
        {
            WithDirection = true;
            Right = false;
        }

        if (Utility.InputDown() && Utility.InputJump())
        {
            ImmediateInput = new InputInfo(InputType.FallPlatform, WithDirection, Right);
            return;
        }

        if (Utility.InputJump())
        {
            ImmediateInput = new InputInfo(InputType.Jump, WithDirection, Right);
            return;
        }

        if (Utility.InputRoll())
        {
            ImmediateInput = new InputInfo(InputType.Roll, WithDirection, Right);
            return;
        }

        if (Utility.InputNormalSlash())
        {
            ImmediateInput = new InputInfo(InputType.Slash, WithDirection, Right);
            return;
        }

        if (Utility.InputBattleArt())
        {
            ImmediateInput = new InputInfo(InputType.BattleArt, WithDirection, Right);
            return;
        }

        if (Utility.InputParry())
        {
            ImmediateInput = new InputInfo(InputType.Parry, WithDirection, Right);
            return;
        }
    }

    protected bool GroundStateActionTransitionCheck(InputInfo ImmediateInput)
    {
        if (ImmediateInput == null)
        {
            if (CharacterClimbLadder())
            {
                TransitionTo<ClimbLadder>();
                return true;
            }

            if (CharacterDownToLadder())
            {
                TransitionTo<DownToLadder>();
                return true;
            }

            return false;
        }

        switch (ImmediateInput.Type)
        {
            case InputType.FallPlatform:
                if (CheckFallPlatform(ImmediateInput))
                {
                    TransitionTo<Air>();
                }
                else
                {
                    PerformJump(ImmediateInput);

                }
                return true;

            case InputType.Jump:
                PerformJump(ImmediateInput);
                return true;
            case InputType.Roll:
                if (CheckPerformRoll(ImmediateInput))
                {
                    PerformRoll(ImmediateInput);
                    return true;
                }
                else
                {
                    return false;
                }
            case InputType.Slash:
                PerformSlash(ImmediateInput);
                return true;
            case InputType.BattleArt:
                if (CheckPerformBattleArt(ImmediateInput))
                {
                    PerformBattleArt(ImmediateInput);
                    return true;
                }
                else
                {
                    return false;
                }
            case InputType.Parry:
                PerformParry(ImmediateInput);
                return true;
        }


        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return true;
        }

        if (CharacterDownToLadder())
        {
            TransitionTo<DownToLadder>();
            return true;
        }

        return false;
    }

    protected void AirStateReceiveImmediateInput(ref InputInfo ImmediateInput)
    {
        bool WithDirection = false;
        bool Right = false;
        if (Utility.InputRight() && !Utility.InputLeft())
        {
            WithDirection = true;
            Right = true;
        }
        else if (!Utility.InputRight() && Utility.InputLeft())
        {
            WithDirection = true;
            Right = false;
        }

        if (Utility.InputNormalSlash())
        {
            ImmediateInput = new InputInfo(InputType.Slash, WithDirection, Right);
            return;
        }

        if (Utility.InputBattleArt())
        {
            ImmediateInput = new InputInfo(InputType.BattleArt, WithDirection, Right);
            return;
        }
    }

    protected bool AirStateActionTransitionCheck(InputInfo ImmediateInput)
    {
        if (ImmediateInput == null)
        {
            if (CharacterClimbPlatform())
            {
                TransitionTo<ClimbPlatform>();
                return true;
            }

            if (CharacterClimbLadder())
            {
                TransitionTo<ClimbLadder>();
                return true;
            }

            return false;
        }

        switch (ImmediateInput.Type)
        {
            case InputType.Slash:
                PerformSlash(ImmediateInput);
                return true;
            case InputType.BattleArt:
                if (CheckPerformBattleArt(ImmediateInput))
                {
                    PerformBattleArt(ImmediateInput);
                    return true;
                }
                else
                {
                    return false;
                }
        }

        if (CharacterClimbPlatform())
        {
            TransitionTo<ClimbPlatform>();
            return true;
        }

        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return true;
        }

        return false;
    }

    protected void AirStateReceiveSavedInput(List<InputInfo> SavedInputList)
    {
        bool WithDirection = false;
        bool Right = false;
        if (Utility.InputRight() && !Utility.InputLeft())
        {
            WithDirection = true;
            Right = true;
        }
        else if (!Utility.InputRight() && Utility.InputLeft())
        {
            WithDirection = true;
            Right = false;
        }

        if (Utility.InputRoll())
        {
            SavedInputList.Add(new InputInfo(InputType.Roll, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if(Utility.InputDown() && Utility.InputJump())
        {
            SavedInputList.Add(new InputInfo(InputType.FallPlatform, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputJump())
        {
            SavedInputList.Add(new InputInfo(InputType.Jump, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputParry())
        {
            SavedInputList.Add(new InputInfo(InputType.Parry, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputBattleArt())
        {
            SavedInputList.Add(new InputInfo(InputType.BattleArt, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }
    }

    protected bool AirStateGroundedCheck(List<InputInfo> SavedInputList)
    {
        if (SavedInputList.Count == 0)
        {
            return false;
        }

        InputInfo Last = SavedInputList[SavedInputList.Count - 1];

        switch (Last.Type)
        {
            case InputType.FallPlatform:
                if (CheckFallPlatform(Last))
                {
                    TransitionTo<Air>();
                }
                else
                {
                    PerformJump(Last);
                }
                break;
            case InputType.Jump:
                PerformJump(Last);
                break;
            case InputType.Parry:
                PerformParry(Last);
                break;
            case InputType.Roll:
                if (CheckPerformRoll(Last))
                {
                    PerformRoll(Last);
                }
                else
                {
                    return false;
                }
                break;
            case InputType.BattleArt:
                if (CheckPerformBattleArt(Last))
                {
                    PerformBattleArt(Last);
                }
                else
                {
                    return false;
                }
                break;

        }

        return true;
    }


    protected void ManageSavedInputList(List<InputInfo> SavedInputList)
    {
        List<InputInfo> RemoveList = new List<InputInfo>();

        for(int i = 0; i < SavedInputList.Count; i++)
        {
            SavedInputList[i].TimeCount += Time.deltaTime;
            if(SavedInputList[i].TimeCount >= SavedInputList[i].KeptTime)
            {
                RemoveList.Add(SavedInputList[i]);
            }
        }

        for(int i = 0; i < RemoveList.Count; i++)
        {
            SavedInputList.Remove(RemoveList[i]);
        }

    }

    protected void RecoveryStateReceiveSavedInput(List<InputInfo> SavedInputList)
    {
        bool WithDirection = false;
        bool Right = false;
        if (Utility.InputRight() && !Utility.InputLeft())
        {
            WithDirection = true;
            Right = true;
        }
        else if (!Utility.InputRight() && Utility.InputLeft())
        {
            WithDirection = true;
            Right = false;
        }

        if (Utility.InputDown() && Utility.InputJump())
        {
            SavedInputList.Add(new InputInfo(InputType.FallPlatform, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputJump())
        {
            SavedInputList.Add(new InputInfo(InputType.Jump, WithDirection, Right,Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputRoll())
        {
            SavedInputList.Add(new InputInfo(InputType.Roll, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputNormalSlash())
        {
            SavedInputList.Add(new InputInfo(InputType.Slash, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputBattleArt())
        {
            SavedInputList.Add(new InputInfo(InputType.BattleArt, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }

        if (Utility.InputParry())
        {
            SavedInputList.Add(new InputInfo(InputType.Parry, WithDirection, Right, Entity.GetComponent<CharacterData>().InputSaveTime));
        }
    }

    protected bool RecoveryEndTransitionCheck(List<InputInfo> SavedInputList, bool Grounded)
    {
        if (SavedInputList.Count == 0)
        {
            return false;
        }

        InputInfo Last = SavedInputList[SavedInputList.Count - 1];

        switch (Last.Type)
        {
            case InputType.FallPlatform:
                if (CheckFallPlatform(Last))
                {
                    TransitionTo<Air>();
                }
                else
                {
                    PerformJump(Last);
                }
                break;
            case InputType.Jump:
                if (!Grounded)
                {
                    return false;
                }
                PerformJump(Last);
                break;
            case InputType.Roll:
                if (!Grounded)
                {
                    return false;
                }
                if (CheckPerformRoll(Last))
                {
                    PerformRoll(Last);
                }
                else
                {
                    return false;
                }
                break;
            case InputType.Slash:
                PerformSlash(Last);
                break;
            case InputType.Parry:
                if (!Grounded)
                {
                    return false;
                }
                PerformParry(Last);
                break;
            case InputType.BattleArt:
                if (CheckPerformBattleArt(Last))
                {
                    PerformBattleArt(Last);
                }
                else
                {
                    return false;
                }
                break;
        }
        return true;
    }

    protected bool CheckGrounded() 
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (SpeedManager.HitGround)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckPerformSlash(InputInfo Info)
    {
        return Info.Type == InputType.Slash;
    }

    protected bool CheckPerformBattleArt(InputInfo Info)
    {
        var Status = Entity.GetComponent<StatusManager_Character>();
        if (Info.Type == InputType.BattleArt && Status.IsEnergyFull() && Context.EquipedBattleArt != null)
        {
            switch (Context.EquipedBattleArt.Type)
            {
                case BattleArtType.PowerSlash:
                    return CheckGrounded();
                case BattleArtType.HarmonySlash:
                    return CheckGrounded();
                case BattleArtType.SpiritBolt:
                    return true;
                case BattleArtType.SpiritFall:
                    return CheckGrounded();
                case BattleArtType.SpiritShadow:
                    return true;
            }

            return true;
        }

        return false;
    }

    protected bool CheckPerformParry(InputInfo Info)
    {
        return Info.Type == InputType.Parry;
    }

    protected bool CheckPerformRoll(InputInfo Info)
    {
        return Info.Type == InputType.Roll && Context.RollCoolDownTimeCount <= 0;
    }

    protected bool CheckPerformJump(InputInfo Info)
    {
        return Info.Type == InputType.Jump;
    }

    protected void ChangeDirection(InputInfo Info)
    {
        if (Info.WithDirection)
        {
            if (Info.DirectionRight)
            {
                Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }

    }

    protected void PerformSlash(InputInfo Info)
    {
        ChangeDirection(Info);
        TransitionTo<SlashAnticipation>();
    }

    protected void PerformBattleArt(InputInfo Info)
    {
        switch (Context.EquipedBattleArt.Type)
        {
            case BattleArtType.PowerSlash:
                ChangeDirection(Info);
                TransitionTo<PowerSlashAnticipation>();
                break;

            case BattleArtType.HarmonySlash:
                ChangeDirection(Info);
                TransitionTo<HarmonySlashAnticipation>();
                break;
        }
    }

    protected void PerformRoll(InputInfo Info)
    {
        ChangeDirection(Info);
        TransitionTo<Roll>();
    }

    protected void PerformJump(InputInfo Info)
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        var Data = Entity.GetComponent<CharacterData>();

        ChangeDirection(Info);
        SpeedManager.SelfSpeed.y = Data.JumpSpeed;

        TransitionTo<Air>();
    }

    protected void PerformParry(InputInfo Info)
    {
        ChangeDirection(Info);
        TransitionTo<ParryAnticipation>();
    }

    protected bool CheckCharacterMove(bool Ground)
    {
        var Data = Entity.GetComponent<CharacterData>();

        var SpeedManager = Entity.GetComponent<SpeedManager>();
        Vector2Int MoveVector = Vector2Int.zero;
        if (Utility.InputRight())
        {
            MoveVector += Vector2Int.right;
        }

        if (Utility.InputLeft())
        {
            MoveVector += Vector2Int.left;
        }

        bool HaveSpeed = true;


        if (MoveVector.x > 0)
        {
            Entity.transform.eulerAngles = Vector3.zero;

            if (SpeedManager.SelfSpeed.x >= 0)
            {
                if (Ground)
                {
                    SpeedManager.SelfSpeed.x += Data.GroundAcceleration * Time.deltaTime;
                }
                else
                {
                    SpeedManager.SelfSpeed.x += Data.AirAcceleration * Time.deltaTime;
                }

                if (SpeedManager.SelfSpeed.x > Data.MaxSpeed)
                {
                    SpeedManager.SelfSpeed.x = Data.MaxSpeed;
                }
            }
            else
            {
                if (Ground)
                {
                    SpeedManager.SelfSpeed.x = Data.GroundAcceleration * Time.deltaTime;
                }
                else
                {
                    SpeedManager.SelfSpeed.x = Data.AirAcceleration * Time.deltaTime;
                }
            }
        }
        else if (MoveVector.x < 0)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            if (SpeedManager.SelfSpeed.x <= 0)
            {
                if (Ground)
                {
                    SpeedManager.SelfSpeed.x -= Data.GroundAcceleration * Time.deltaTime;
                }
                else
                {
                    SpeedManager.SelfSpeed.x -= Data.AirAcceleration * Time.deltaTime;
                }

                if (SpeedManager.SelfSpeed.x < -Data.MaxSpeed)
                {
                    SpeedManager.SelfSpeed.x = -Data.MaxSpeed;
                }
            }
            else
            {
                if (Ground)
                {
                    SpeedManager.SelfSpeed.x = -Data.GroundAcceleration * Time.deltaTime;
                }
                else
                {
                    SpeedManager.SelfSpeed.x = -Data.AirAcceleration * Time.deltaTime;
                }
            }
        }
        else
        {
            if (SpeedManager.SelfSpeed.x > 0)
            {
                if (Ground)
                {
                    SpeedManager.SelfSpeed.x -= Data.GroundDeceleration * Time.deltaTime;
                }
                else
                {
                    SpeedManager.SelfSpeed.x -= Data.AirDeceleration * Time.deltaTime;
                }

                if (SpeedManager.SelfSpeed.x <= 0)
                {
                    SpeedManager.SelfSpeed.x = 0;
                    HaveSpeed = false;
                }
            }
            else
            {
                if (Ground)
                {
                    SpeedManager.SelfSpeed.x += Data.GroundDeceleration * Time.deltaTime;
                }
                else
                {
                    SpeedManager.SelfSpeed.x += Data.AirDeceleration * Time.deltaTime;
                }

                if (SpeedManager.SelfSpeed.x >= 0)
                {
                    SpeedManager.SelfSpeed.x = 0;
                    HaveSpeed = false;
                }
            }
        }

        return HaveSpeed;
    }
}

public class Stand : CharacterActionState
{
    private InputInfo ImmediateInput;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();

        ImmediateInput = null;

        GroundStateReceiveImmediateInput(ref ImmediateInput);
    }

    public override void Update()
    {
        base.Update();
        RunCheck();
        
    }

    private void RunCheck()
    {
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        if (!CheckGrounded())
        {
            TransitionTo<Air>();
            return;
        }

        if (GroundStateActionTransitionCheck(ImmediateInput))
        {
            return;
        }
        ImmediateInput = null;
        GroundStateReceiveImmediateInput(ref ImmediateInput);

        if (CheckCharacterMove(true))
        {
            TransitionTo<GroundMove>();
            return;
        }
    }

    private void SetUp()
    {
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }
}

public class GroundMove : CharacterActionState
{
    private InputInfo ImmediateInput;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();

        ImmediateInput = null;
        GroundStateReceiveImmediateInput(ref ImmediateInput);
    }

    public override void Update()
    {
        base.Update();
        RunCheck();
    }

    private void SetUp()
    {
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void RunCheck()
    {
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        if (!CheckGrounded())
        {
            TransitionTo<Air>();
            return;
        }

        if (GroundStateActionTransitionCheck(ImmediateInput))
        {
            return;
        }

        ImmediateInput = null;
        GroundStateReceiveImmediateInput(ref ImmediateInput);

        if (!CheckCharacterMove(true))
        {
            TransitionTo<Stand>();
            return;
        }
    }
}

public class Air : CharacterActionState
{
    private List<InputInfo> SavedInputList;
    private InputInfo ImmediateInput;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();

        ImmediateInput = null;
        AirStateReceiveImmediateInput(ref ImmediateInput);
    }

    public override void Update()
    {
        base.Update();
        RunCheck();
    }

    private void SetUp()
    {
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

        SavedInputList = new List<InputInfo>();
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void RunCheck()
    {
        CheckCharacterMove(false);

        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        if (CheckGrounded())
        {
            if (!AirStateGroundedCheck(SavedInputList))
            {
                TransitionTo<Stand>();
                return;
            }
        }

        AirStateReceiveImmediateInput(ref ImmediateInput);

        if (AirStateActionTransitionCheck(ImmediateInput))
        {
            return;
        }

        ManageSavedInputList(SavedInputList);
        AirStateReceiveSavedInput(SavedInputList);

    }
}

public class SlashAnticipation : CharacterActionState
{
    private float TimeCount;

    private float Anticipation;
    private float Strike;
    private float Recovery;

    private int Damage;
    private Vector2 Offset;
    private Vector2 Size;

    private GameObject SlashEffect;

    private bool Grounded;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();

        EventManager.instance.Fire(new PlayerStartAttackAnticipation(Context.CurrentAttack));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            GameObject.Destroy(SlashEffect);
            TransitionTo<GetInterrupted>();
            return;
        }

        if (!Grounded)
        {
            CheckCharacterMove(false);
            if (CheckGrounded())
            {
                Grounded = true;
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            }
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        EventManager.instance.Fire(new PlayerEndAttackAnticipation(Context.CurrentAttack));
    }

    private void SetUp()
    {
        RectifyDirectionWithInput();

        TimeCount = 0;

        var Status = Entity.GetComponent<StatusManager_Character>();

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Data = Entity.GetComponent<CharacterData>();

        int SlashDamage = Mathf.RoundToInt(Status.CurrentBaseDamage * AbilityData.SlashDamageFactor);

        Direction dir;
        if(Entity.transform.right.x > 0)
        {
            dir = Direction.Right;
        }
        else
        {
            dir = Direction.Left;
        }


        SetAttribute(AbilityData.SlashAnticipationTime, AbilityData.SlashStrikeTime, AbilityData.SlashRecoveryTime, SlashDamage, AbilityData.SlashOffset, AbilityData.SlashHitBoxSize);

        Context.CurrentAttack = new CharacterAttackInfo(Entity, CharacterAttackType.Slash, DamageType.Normal, dir, Damage, Damage, Damage, Offset, Offset, Size, Size, Anticipation, Anticipation, Strike, Strike, Recovery, Recovery);

        Context.HitEnemies = new List<GameObject>();


        if (CheckGrounded())
        {
            Grounded = true;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else
        {
            Grounded = false;
        }

        SlashEffect = null;


    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void CheckTime()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        TimeCount += Time.deltaTime;

        if (TimeCount >= Context.CurrentAttack.AnticipationTime)
        {
            TransitionTo<SlashStrike>();
            return;
        }
    }

    private void SetAttribute(float anticipation, float strike, float recovery, int damage, Vector2 offset, Vector2 size)
    {
        Anticipation = anticipation;
        Strike = strike;
        Recovery = recovery;

        Damage = damage;
        Offset = offset;
        Size = size;
    }
}

public class SlashStrike : CharacterActionState
{
    private float StepForwardSpeed;
    private GameObject SlashEffect;

    private float TimeCount;
    private bool Grounded;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        GenerateSlashEffect(Context.CurrentAttack);
        EventManager.instance.Fire(new PlayerStartAttackStrike(Context.CurrentAttack));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        if (!Grounded)
        {
            //CheckCharacterMove(false);
            if (CheckGrounded())
            {
                Grounded = true;
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            }
        }

        HitEnemy(Context.CurrentAttack, Context.HitEnemies);
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        SpeedManager.AttackStepSpeed.x = 0;
        GameObject.Destroy(SlashImage);
        EventManager.instance.Fire(new PlayerEndAttackStrike(Context.CurrentAttack, Context.HitEnemies));
    }

    private void SetUp()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        SlashEffect = AbilityData.SlashEffect;

        TimeCount = 0;

        Context.CurrentGravity = Data.NormalGravity;

        if (CheckGrounded())
        {
            Grounded = true;
            SpeedManager.AttackStepSpeed.x = AbilityData.SlashStepForwardSpeed * Entity.transform.right.x;
        }
        else
        {
            Grounded = false;
            SpeedManager.AttackStepSpeed.x = 0;
        }

    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.LightRecoverySeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
    }

    private void GenerateSlashEffect(CharacterAttackInfo Attack)
    {
        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        float EulerAngle = 0;
        Vector2 Offset = AbilityData.SlashEffectOffset;

        if (Attack.Dir == Direction.Left)
        {
            EulerAngle = 180;
            Offset.x = -Offset.x;
        }

        GameObject Effect = GameObject.Instantiate(SlashEffect, (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        Effect.transform.parent = Entity.transform;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > Context.CurrentAttack.StrikeTime)
        {
            TransitionTo<SlashRecovery>();
            return;
        }
    }

}

public class SlashRecovery : CharacterActionState
{
    private float TimeCount;
    private List<InputInfo> SavedInputList;

    private bool Grounded;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        RecoveryStateReceiveSavedInput(SavedInputList);
        EventManager.instance.Fire(new PlayerStartAttackRecovery(Context.CurrentAttack,Context.HitEnemies));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        if (!Grounded)
        {
            CheckCharacterMove(false);
            if (CheckGrounded())
            {
                Grounded = true;
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            }
        }


        ManageSavedInputList(SavedInputList);
        RecoveryStateReceiveSavedInput(SavedInputList);


        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        EventManager.instance.Fire(new PlayerEndAttackRecovery(Context.CurrentAttack, Context.HitEnemies));

        Context.CurrentAttack = null;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > Context.CurrentAttack.RecoveryTime)
        {
            if (!RecoveryEndTransitionCheck(SavedInputList,true))
            {
                TransitionTo<Stand>();
            }
        }
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();

        Context.CurrentGravity = Data.NormalGravity;
        Entity.GetComponent<SpeedManager>().AttackStepSpeed.x = 0;

        TimeCount = 0;

        SavedInputList = new List<InputInfo>();

        if (CheckGrounded())
        {
            Grounded = true;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else
        {
            Grounded = false;
        }
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.LightRecoverySeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
    }

}

public class HarmonySlashAnticipation : CharacterActionState
{
    private float TimeCount;

    private float Anticipation;
    private float Strike;
    private float Recovery;

    private int Damage;
    private Vector2 Offset;
    private Vector2 Size;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        EventManager.instance.Fire(new PlayerStartAttackAnticipation(Context.CurrentAttack));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        EventManager.instance.Fire(new PlayerEndAttackAnticipation(Context.CurrentAttack));
    }

    private void SetUp()
    {
        RectifyDirectionWithInput();

        TimeCount = 0;

        var Status = Entity.GetComponent<StatusManager_Character>();

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Data = Entity.GetComponent<CharacterData>();

        Direction dir;
        if (Entity.transform.right.x > 0)
        {
            dir = Direction.Right;
        }
        else
        {
            dir = Direction.Left;
        }

        int HarmonySlashDamage = Mathf.RoundToInt(Status.CurrentBaseDamage * AbilityData.HarmonySlashDamageFactor);
        
        SetAttribute(AbilityData.HarmonySlashAnticipationTime, AbilityData.HarmonySlashStrikeTime, AbilityData.HarmonySlashRecoveryTime, HarmonySlashDamage, AbilityData.HarmonySlashOffset, AbilityData.HarmonySlashHitBoxSize);

        Context.CurrentAttack = new CharacterAttackInfo(Entity, CharacterAttackType.HarmonySlash, DamageType.Strike, dir, Damage, Damage, Damage, Offset, Offset, Size, Size, Anticipation, Anticipation, Strike, Strike, Recovery, Recovery, Context.EquipedBattleArt);

        Context.HitEnemies = new List<GameObject>();

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        Entity.GetComponent<SpeedManager>().AttackStepSpeed.x = -Entity.transform.right.x * AbilityData.HarmonySlashStepBackSpeed;

    }

    private void SetAttribute(float anticipation, float strike, float recovery, int damage, Vector2 offset, Vector2 size)
    {
        Anticipation = anticipation;
        Strike = strike;
        Recovery = recovery;

        Damage = damage;
        Offset = offset;
        Size = size;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.HeavyAnticipationSeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyAnticipationOffset, SpriteData.HeavyAnticipationSize);
    }

    private void CheckTime()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        TimeCount += Time.deltaTime;

        if (TimeCount >= Context.CurrentAttack.AnticipationTime)
        {
            TransitionTo<HarmonySlashStrike>();
            return;
        }
        else if(TimeCount >= AbilityData.HarmonySlashStepBackTime)
        {
            Entity.GetComponent<SpeedManager>().AttackStepSpeed.x = 0;
        }
    }


}

public class HarmonySlashStrike : CharacterActionState
{
    private float StepForwardSpeed;
    private GameObject Effect;

    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        EventManager.instance.Fire(new PlayerStartAttackStrike(Context.CurrentAttack));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        HitEnemy(Context.CurrentAttack,Context.HitEnemies);
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        GameObject.Destroy(SlashImage);
        EventManager.instance.Fire(new PlayerEndAttackStrike(Context.CurrentAttack, Context.HitEnemies));
    }

    private void SetUp()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        TimeCount = 0;

        Effect = AbilityData.HarmonySlashEffect;
        GenerateSlashEffect(Effect, Context.CurrentAttack);

        SpeedManager.AttackStepSpeed.x = AbilityData.HarmonySlashStepForwardSpeed* Entity.transform.right.x;

    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
    }



    private void GenerateSlashEffect(GameObject Effect, CharacterAttackInfo Attack)
    {

        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        float EulerAngle = 0;
        Vector2 Offset = AbilityData.HarmonySlashEffectOffset;

        if (Attack.Dir == Direction.Left)
        {
            EulerAngle = 180;
            Offset.x = -Offset.x;
        }

        SlashImage = GameObject.Instantiate(Effect, (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        SlashImage.transform.parent = Entity.transform;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > Context.CurrentAttack.StrikeTime)
        {
            TransitionTo<HarmonySlashRecovery>();
            return;
        }
    }
}

public class HarmonySlashRecovery : CharacterActionState
{
    private float TimeCount;
    private List<InputInfo> SavedInputList;
    private InputInfo ImmediateInput;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        RecoveryStateReceiveSavedInput(SavedInputList);
        EventManager.instance.Fire(new PlayerStartAttackRecovery(Context.CurrentAttack, Context.HitEnemies));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }


        ManageSavedInputList(SavedInputList);
        RecoveryStateReceiveSavedInput(SavedInputList);

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        EventManager.instance.Fire(new PlayerEndAttackRecovery(Context.CurrentAttack, Context.HitEnemies));

        Context.CurrentAttack = null;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > Context.CurrentAttack.RecoveryTime)
        {
            if (!RecoveryEndTransitionCheck(SavedInputList,true))
            {
                TransitionTo<Stand>();
            }
        }
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();

        Entity.GetComponent<SpeedManager>().AttackStepSpeed.x = 0;

        TimeCount = 0;

        SavedInputList = new List<InputInfo>();
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
    }
}

public class PowerSlashAnticipation : CharacterActionState
{
    private float TimeCount;

    private float Anticipation;
    private float Strike;
    private float Recovery;

    private int Damage;
    private Vector2 Offset;
    private Vector2 Size;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        EventManager.instance.Fire(new PlayerStartAttackAnticipation(Context.CurrentAttack));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        EventManager.instance.Fire(new PlayerEndAttackAnticipation(Context.CurrentAttack));
    }

    private void SetUp()
    {
        RectifyDirectionWithInput();

        TimeCount = 0;

        var Status = Entity.GetComponent<StatusManager_Character>();

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Data = Entity.GetComponent<CharacterData>();

        Direction dir;
        if (Entity.transform.right.x > 0)
        {
            dir = Direction.Right;
        }
        else
        {
            dir = Direction.Left;
        }

        int PowerSlashDamage = Mathf.RoundToInt(Status.CurrentBaseDamage * AbilityData.PowerSlashDamageFactor);

        SetAttribute(AbilityData.PowerSlashAnticipationTime, AbilityData.PowerSlashStrikeTime, AbilityData.PowerSlashRecoveryTime, PowerSlashDamage, AbilityData.PowerSlashOffset, AbilityData.PowerSlashHitBoxSize);

        Context.CurrentAttack = new CharacterAttackInfo(Entity, CharacterAttackType.PowerSlash, DamageType.Strike, dir, Damage, Damage, Damage, Offset, Offset, Size, Size, Anticipation, Anticipation, Strike, Strike, Recovery, Recovery, Context.EquipedBattleArt);

        Context.HitEnemies = new List<GameObject>();

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

    }

    private void SetAttribute(float anticipation, float strike, float recovery, int damage, Vector2 offset, Vector2 size)
    {
        Anticipation = anticipation;
        Strike = strike;
        Recovery = recovery;

        Damage = damage;
        Offset = offset;
        Size = size;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.HeavyAnticipationSeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyAnticipationOffset, SpriteData.HeavyAnticipationSize);
    }

    private void CheckTime()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        TimeCount += Time.deltaTime;

        if (TimeCount >= Context.CurrentAttack.AnticipationTime)
        {
            TransitionTo<PowerSlashStrike>();
            return;
        }
    }
}

public class PowerSlashStrike : CharacterActionState
{
    private float StepForwardSpeed;
    private GameObject Effect;

    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        EventManager.instance.Fire(new PlayerStartAttackStrike(Context.CurrentAttack));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        HitEnemy(Context.CurrentAttack, Context.HitEnemies);
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        GameObject.Destroy(SlashImage);
        EventManager.instance.Fire(new PlayerEndAttackStrike(Context.CurrentAttack, Context.HitEnemies));
    }

    private void SetUp()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        TimeCount = 0;

        Effect = AbilityData.PowerSlashEffect;
        GenerateSlashEffect(Effect, Context.CurrentAttack);

        SpeedManager.AttackStepSpeed.x = AbilityData.PowerSlashStepForwardSpeed * Entity.transform.right.x;

    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
    }

    private void GenerateSlashEffect(GameObject Image, CharacterAttackInfo Attack)
    {

        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        float EulerAngle = 0;
        Vector2 Offset = AbilityData.PowerSlashEffectOffset;

        if (Attack.Dir == Direction.Left)
        {
            EulerAngle = 180;
            Offset.x = -Offset.x;
        }

        SlashImage = GameObject.Instantiate(Image, (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        SlashImage.transform.parent = Entity.transform;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > Context.CurrentAttack.StrikeTime)
        {
            TransitionTo<PowerSlashRecovery>();
            return;
        }
    }
}

public class PowerSlashRecovery : CharacterActionState
{
    private float TimeCount;
    private List<InputInfo> SavedInputList;
    private InputInfo ImmediateInput;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        RecoveryStateReceiveSavedInput(SavedInputList);
        EventManager.instance.Fire(new PlayerStartAttackRecovery(Context.CurrentAttack, Context.HitEnemies));
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }


        ManageSavedInputList(SavedInputList);
        RecoveryStateReceiveSavedInput(SavedInputList);

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        EventManager.instance.Fire(new PlayerEndAttackRecovery(Context.CurrentAttack, Context.HitEnemies));

        Context.CurrentAttack = null;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > Context.CurrentAttack.RecoveryTime)
        {
            if (!RecoveryEndTransitionCheck(SavedInputList, true))
            {
                TransitionTo<Stand>();
            }
        }
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();

        Entity.GetComponent<SpeedManager>().AttackStepSpeed.x = 0;

        TimeCount = 0;

        SavedInputList = new List<InputInfo>();
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;
        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
    }
}

public class ParryAnticipation : CharacterActionState
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        CheckTime();
    }

    private void SetUp()
    {
        RectifyDirectionWithInput();

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        TimeCount = 0;
        StateTime = AbilityData.ParryAnticipationTime;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= StateTime)
        {
            TransitionTo<Parry>();
            return;
        }
    }
}

public class Parry : CharacterActionState
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Entity.GetComponent<StatusManager_Character>().SetParryInvulnerability(false);
    }

    private void SetUp()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        TimeCount = 0;
        StateTime = AbilityData.ParryEffectTime;
        Entity.GetComponent<StatusManager_Character>().SetParryInvulnerability(true);


    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<ParryRecovery>();
            return;
        }
    }

}

public class ParryRecovery : CharacterActionState
{
    private float TimeCount;
    private float StateTime;
    private List<InputInfo> SavedInputList;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        RecoveryStateReceiveSavedInput(SavedInputList);
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        ManageSavedInputList(SavedInputList);
        RecoveryStateReceiveSavedInput(SavedInputList);

        CheckTime();
    }

    private void SetUp()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        TimeCount = 0;
        StateTime = AbilityData.ParryRecoveryTime;

        SavedInputList = new List<InputInfo>();
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            if (!RecoveryEndTransitionCheck(SavedInputList,true))
            {
                TransitionTo<Stand>();
            }
        }
    }
}

public class Roll: CharacterActionState
{
    private float TimeCount;
    private float StateTime;
    private GameObject RollEffect;

    private List<InputInfo> SavedInputList;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        RecoveryStateReceiveSavedInput(SavedInputList);
        EventManager.instance.Fire(new PlayerStartRoll());
    }

    public override void Update()
    {
        base.Update();

        if (CheckGetInterrupted())
        {
            //GameObject.Destroy(RollEffect);
            TransitionTo<GetInterrupted>();
            return;
        }

        ManageSavedInputList(SavedInputList);
        RecoveryStateReceiveSavedInput(SavedInputList);

        if (HitWall())
        {
            if (!RecoveryEndTransitionCheck(SavedInputList,true))
            {
                TransitionTo<Stand>();
                return;
            }
        }

        if (!CheckGrounded())
        {
            TransitionTo<Air>();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        EventManager.instance.Fire(new PlayerEndRoll());

        SpeedManager.IgnoredLayers = Data.NormalIgnoredLayers;
        SpeedManager.SelfSpeed.x = 0;

        Context.RollCoolDownTimeCount = Entity.GetComponent<CharacterData>().RollCoolDown;

        Entity.GetComponent<StatusManager_Character>().SetRollInvulnerability(false);
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        TimeCount = 0;
        StateTime = Data.RollTime;

        SpeedManager.IgnoredLayers = Data.RollIgnoredLayers;
        if (Entity.transform.right.x > 0)
        {
            SpeedManager.SelfSpeed.x = Data.RollSpeed;
        }
        else
        {
            SpeedManager.SelfSpeed.x = -Data.RollSpeed;
        }

        Context.RollCoolDownTimeCount = Entity.GetComponent<CharacterData>().RollCoolDown;

        Entity.GetComponent<StatusManager_Character>().SetRollInvulnerability(true);

        SavedInputList = new List<InputInfo>();

        GenerateRollEffect();

    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            if (!RecoveryEndTransitionCheck(SavedInputList,true))
            {
                TransitionTo<Stand>();
            }
        }
    }

    private bool HitWall()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (Entity.transform.right.x > 0)
        {
            if (SpeedManager.HitRight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (SpeedManager.HitLeft)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void GenerateRollEffect()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        float EulerAngle;
        Vector2 Offset;

        if (Entity.transform.right.x > 0)
        {
            EulerAngle = 0;
            Offset =  new Vector2(-SpeedManager.BodyWidth / 2, -SpeedManager.BodyHeight / 2);
        }
        else
        {
            EulerAngle = 180;
            Offset =  new Vector2(SpeedManager.BodyWidth / 2, -SpeedManager.BodyHeight / 2);
        }

        RollEffect = GameObject.Instantiate(Data.RollEffect, SpeedManager.GetTruePos() + Offset, Quaternion.Euler(0, EulerAngle, 0));

        //RollEffect.transform.parent = Entity.transform;
        
    }
}

public class ClimbPlatform : CharacterActionState
{
    private float TimeCount;
    private float StateTime;

    private float StartHeight;
    private float TargetHeight;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        Climb();
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.ClimbPlatformTime;

        Context.CurrentGravity = 0;

        var SpeedManager = Entity.GetComponent<SpeedManager>();

        SpeedManager.SelfSpeed = Vector2.zero;

        float PlatformTop = Context.AttachedPassablePlatform.transform.position.y + Context.AttachedPassablePlatform.transform.localScale.y*(Context.AttachedPassablePlatform.GetComponent<BoxCollider2D>().offset.y+ Context.AttachedPassablePlatform.GetComponent<BoxCollider2D>().size.y / 2);
        float SelfTop = SpeedManager.GetTruePos().y + SpeedManager.BodyHeight / 2;

        Entity.transform.position += (SelfTop - PlatformTop)*Vector3.down;

        StartHeight = Entity.transform.position.y;

        TargetHeight = PlatformTop + SpeedManager.BodyHeight / 2 - (SpeedManager.GetTruePos().y-Entity.transform.position.y);
    }

    private void Climb()
    {
        TimeCount += Time.deltaTime;
        Entity.transform.position = new Vector2(Entity.transform.position.x, Mathf.Lerp(StartHeight, TargetHeight, TimeCount / StateTime));
        if(TimeCount > StateTime)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.y = Entity.GetComponent<CharacterData>().ClimbPlatformJumpOverSpeed;
            TransitionTo<Air>();
        }
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }
}

public class DownToLadder : CharacterActionState
{
    private float TimeCount;
    private float StateTime;

    private float StartHeight;
    private float TargetHeight;

    public override void OnEnter()
    {
        base.OnEnter();

        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        MoveDown();
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.ClimbPlatformTime;

        Context.CurrentGravity = 0;

        var SpeedManager = Entity.GetComponent<SpeedManager>();

        SpeedManager.SelfSpeed = Vector2.zero;

        float LadderTop = Context.AttachedLadder.transform.position.y + Context.AttachedLadder.transform.localScale.y * (Context.AttachedLadder.GetComponent<BoxCollider2D>().offset.y + Context.AttachedLadder.GetComponent<BoxCollider2D>().size.y / 2);
        float SelfTop = SpeedManager.GetTruePos().y + SpeedManager.BodyHeight / 2;

        StartHeight = Entity.transform.position.y;

        TargetHeight = LadderTop - SpeedManager.BodyHeight / 2 - (SpeedManager.GetTruePos().y - Entity.transform.position.y);

        Entity.transform.position = new Vector2(Context.AttachedLadder.transform.position.x - (SpeedManager.GetTruePos().x - Entity.transform.position.x),Entity.transform.position.y);
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void MoveDown()
    {
        TimeCount += Time.deltaTime;
        Entity.transform.position = new Vector2(Entity.transform.position.x, Mathf.Lerp(StartHeight, TargetHeight, TimeCount / StateTime));
        if (TimeCount > StateTime)
        {
            TransitionTo<ClimbLadder>();
        }
    }
}

public class ClimbLadder : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        if (CharacterClimbPlatform())
        {
            Context.AttachedLadder = null;
            TransitionTo<ClimbPlatform>();
            return;
        }

        if (CheckMovement())
        {
            TransitionTo<Stand>();
            return;
        }

        if (Utility.InputJump())
        {
            TransitionTo<Air>();
            return;
        }

        if (!OnLadder())
        {
            TransitionTo<Air>();
            return;
        }
    }

    private bool CheckMovement()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        var Data = Entity.GetComponent<CharacterData>();

        if(Utility.InputUp() && !Utility.InputDown())
        {
            SpeedManager.SelfSpeed.y = Data.ClimbLadderSpeed;
        }
        else if(!Utility.InputUp() && Utility.InputDown())
        {
            SpeedManager.SelfSpeed.y = -Data.ClimbLadderSpeed;
            if (CheckGrounded())
            {
                return true;
            }
        }
        else
        {
            SpeedManager.SelfSpeed.y = 0;
        }

        return false;
    }

    private bool OnLadder()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        float Thickness = 0.01f;

        RaycastHit2D Hit = Physics2D.BoxCast(SpeedManager.GetTruePos() + (SpeedManager.BodyHeight/2 + Thickness/2) * Vector2.up, new Vector2(SpeedManager.BodyWidth, Thickness), 0, Vector2.up, 0, Data.LadderLayer);

        if (Hit && Hit.collider.gameObject == Context.AttachedLadder)
        {
            return true;
        }
        else
        {
            Context.AttachedLadder = null;
            return false;
        }
    }
    
    private void SetUp()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        SpeedManager.SelfSpeed = Vector2.zero;

        Entity.transform.position = new Vector2(Context.AttachedLadder.transform.position.x - (SpeedManager.GetTruePos().x-Entity.transform.position.x), Entity.transform.position.y);
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
    }
}

public class GetInterrupted : CharacterActionState
{
    private List<InputInfo> SavedInputList;
    private float TimeCount;
    
    public override void OnEnter()
    {
        base.OnEnter();

        SetUp();

        SetAppearance();
        //RecoveryStateReceiveSavedInput(SavedInputList);

        GameObject.Destroy(SlashImage);
    }

    public override void Update()
    {
        base.Update();

        if (CheckGetInterrupted())
        {
            SetUp();
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HitSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HitOffset, SpriteData.HitSize);
    }

    private void SetUp()
    {
        Context.RollCoolDownTimeCount = 0;

        TimeCount = 0;

        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        Entity.GetComponent<IHittable>().Interrupted = false;
        Context.CurrentGravity = Data.NormalGravity;

        EnemyAttackInfo Temp = Entity.GetComponent<StatusManager_Character>().CurrentTakenAttack;
        SpeedManager.SelfSpeed = Vector2.zero;
        if (Temp.Right)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);

            SpeedManager.SelfSpeed.x = Data.InterruptedSpeed;
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);

            SpeedManager.SelfSpeed.x = -Data.InterruptedSpeed;

        }

        SavedInputList = new List<InputInfo>();
    }

    private void CheckTime()
    {
        var Data = Entity.GetComponent<CharacterData>();

        TimeCount += Time.deltaTime;

        if(TimeCount >= Data.InterruptedTime)
        {
            if (CheckGrounded())
            {
                if (!RecoveryEndTransitionCheck(SavedInputList, true))
                {
                    TransitionTo<Stand>();
                }

            }
            else
            {
                if (RecoveryEndTransitionCheck(SavedInputList, false))
                {
                    TransitionTo<Air>();
                }
            }

        }
    }
}

