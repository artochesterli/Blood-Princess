using System.Collections;
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
    public bool Right;
    public int Damage;
    public int BaseDamage;
    public int ShieldBreak;
    public int Cost;
    public Vector2 HitBoxOffset;
    public Vector2 HitBoxSize;

    public CharacterAttackInfo(GameObject source, CharacterAttackType type, bool right, int damage, int basedamage,int shieldbreak,int cost, Vector2 offset, Vector2 size)
    {
        Source = source;
        Type = type;
        Right = right;
        Damage = damage;
        BaseDamage = basedamage;
        ShieldBreak = shieldbreak;
        Cost = cost;
        HitBoxOffset = offset;
        HitBoxSize = size;
    }
}

public class EnemyAttackInfo : AttackInfo
{
    public bool Right;
    public int Damage;
    public Vector2 HitBoxOffset;
    public Vector2 HitBoxSize;
    public EnemyAttackInfo(GameObject source, bool right,int damage,Vector2 offset,Vector2 size)
    {
        Source = source;
        Right = right;
        Damage = damage;
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


[System.Serializable]
public class BattleArtEnhancement : CharacterAbility
{
    public BattleArtEnhancementType Type;
    public BattleArtEnhancement(string s, BattleArtEnhancementType type, int level)
    {
        name = s;
        Type = type;
        Level = level;
    }
}

[System.Serializable]
public class CharacterPassiveAbility : CharacterAbility
{
    public CharacterPassiveAbilityType Type;
    public CharacterPassiveAbility(string s, CharacterPassiveAbilityType type, int level)
    {
        name = s;
        Type = type;
        Level = level;
    }
}

public enum CharacterPassiveAbilityType
{
    Null,
    SpiritBlade,
    Vines
}

public enum BattleArtEnhancementType
{
    Null,
    CriticleEye,
    Harmony,
    ShieldBreaker,
    SpiritSlash,
    Laceration
}

public enum CharacterAttackType
{
    NormalSlash,
    BloodSlash,
    DeadSlash,
}

public enum InputType
{
    Jump,
    NormalSlash,
    Roll,
    FirstSkill,
    SecondSkill
}

public class InputInfo
{
    public InputType Type;
    public float TimeCount;

    public InputInfo(InputType type)
    {
        Type = type;
        TimeCount = 0;
    }
}

public class CharacterAction : MonoBehaviour
{
    public bool InRecovery;
    public float CurrentGravity;
    public float JumpHoldingTimeCount;
    public GameObject AttachedPassablePlatform;
    public GameObject AttachedLadder;
    public GameObject FirstSkillInfo;
    public GameObject SecondSkillInfo;
    public GameObject PassiveSkillInfo;

    public GameObject CriticalEyeIcon;

    public float RollCoolDownTimeCount;

    public List<BattleArtEnhancement> BloodSlashEnhancements;
    public List<BattleArtEnhancement> DeadSlashEnhancements;

    public List<CharacterPassiveAbility> EquipedPassiveAbilities;

    public CharacterAttackType CurrentAttackType;

    public List<InputInfo> SavedInputInfo;
    private FSM<CharacterAction> CharacterActionFSM;

    // Start is called before the first frame update
    void Start()
    {
        SetUpAbility();
        SetCanvasInfo();

        SavedInputInfo = new List<InputInfo>();
        CharacterActionFSM = new FSM<CharacterAction>(this);
        CharacterActionFSM.TransitionTo<Stand>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRollCoolDown();
        UpdateSavedInputInfo();
        GetSaveableInput();
        CharacterActionFSM.Update();

    }

    private void SetCanvasInfo()
    {
        FirstSkillInfo.GetComponent<Text>().text = "BloodSlash: ";
        SecondSkillInfo.GetComponent<Text>().text = "DeadSlash: ";
        PassiveSkillInfo.GetComponent<Text>().text = "Passive:";

        for (int i = 0; i < BloodSlashEnhancements.Count; i++)
        {
            if (BloodSlashEnhancements[i].Type!=BattleArtEnhancementType.Null)
            {
                FirstSkillInfo.GetComponent<Text>().text += BloodSlashEnhancements[i].name + "(" + BloodSlashEnhancements[i].Level + ") ";
            }
        }
        for (int i = 0; i < DeadSlashEnhancements.Count; i++)
        {
            if (DeadSlashEnhancements[i].Type != BattleArtEnhancementType.Null)
            {
                SecondSkillInfo.GetComponent<Text>().text += DeadSlashEnhancements[i].name + "(" + DeadSlashEnhancements[i].Level + ") ";
            }
        }
        for (int i = 0; i < EquipedPassiveAbilities.Count; i++)
        {
            if (EquipedPassiveAbilities[i].Type != CharacterPassiveAbilityType.Null)
            {
                PassiveSkillInfo.GetComponent<Text>().text += EquipedPassiveAbilities[i].name + "(" + EquipedPassiveAbilities[i].Level + ") ";
            }
        }
    }

    private void SetUpAbility()
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        BloodSlashEnhancements = new List<BattleArtEnhancement>();
        DeadSlashEnhancements = new List<BattleArtEnhancement>();
        EquipedPassiveAbilities = new List<CharacterPassiveAbility>();

        for(int i = 0; i < AbilityData.MaxBattleArtEnhancementNumber; i++)
        {
            BloodSlashEnhancements.Add(new BattleArtEnhancement("",BattleArtEnhancementType.Null,0));
            DeadSlashEnhancements.Add(new BattleArtEnhancement("", BattleArtEnhancementType.Null, 0));
        }

        for(int i = 0; i < AbilityData.MaxPassiveSkillNumber; i++)
        {
            EquipedPassiveAbilities.Add(new CharacterPassiveAbility("", CharacterPassiveAbilityType.Null, 0));
        }
    }

    private void UpdateRollCoolDown()
    {
        RollCoolDownTimeCount -= Time.deltaTime;
        if (RollCoolDownTimeCount < 0)
        {
            RollCoolDownTimeCount = 0;
        }
    }

    private void UpdateSavedInputInfo()
    {
        List<InputInfo> RemoveList = new List<InputInfo>();
        for(int i = 0; i < SavedInputInfo.Count; i++)
        {
            SavedInputInfo[i].TimeCount += Time.deltaTime;
            if (SavedInputInfo[i].TimeCount >= GetComponent<CharacterData>().InputSaveTime)
            {
                RemoveList.Add(SavedInputInfo[i]);
            }
        }

        for(int i = 0; i < RemoveList.Count; i++)
        {
            SavedInputInfo.Remove(RemoveList[i]);
        }
    }

    private void GetSaveableInput()
    {
        if (Utility.InputJump())
        {
            SavedInputInfo.Add(new InputInfo(InputType.Jump));
        }

        if (Utility.InputNormalSlash())
        {
            SavedInputInfo.Add(new InputInfo(InputType.NormalSlash));
        }

        if (Utility.InputFirstSkill())
        {
            SavedInputInfo.Add(new InputInfo(InputType.FirstSkill));
        }

        if (Utility.InputSecondSkill())
        {
            SavedInputInfo.Add(new InputInfo(InputType.SecondSkill));
        }

        if (Utility.InputRoll())
        {
            SavedInputInfo.Add(new InputInfo(InputType.Roll));
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
        //Debug.Log(this.GetType().Name);

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

        if(Status.CurrentEnergy == 3)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[3];
        }
        else if(Status.CurrentEnergy == 2)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[2];
        }
        else if(Status.CurrentEnergy == 1)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[1];
        }
        else
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[0];
        }
    }

    protected void RecoveryTransition()
    {
        if(Utility.InputRight() && !Utility.InputLeft())
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if(!Utility.InputRight() && Utility.InputLeft())
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (CheckCharacterJump())
        {
            Context.SavedInputInfo.Clear();
            TransitionTo<JumpHoldingStay>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterRoll())
        {
            Context.SavedInputInfo.Clear();
            TransitionTo<RollAnticipation>();
            return;
        }

        if (CheckGrounded())
        {
            TransitionTo<Stand>();
        }
        else
        {
            TransitionTo<AirStay>();
        }
        return;
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
                    var AbilityData = Entity.GetComponent<CharacterAbilityData>();

                    switch (Attack.Type)
                    {
                        case CharacterAttackType.NormalSlash:

                            Status.CurrentEnergy ++;
                            if (Status.CurrentEnergy > Data.MaxEnergy)
                            {
                                Status.CurrentEnergy = Data.MaxEnergy;
                            }
                            break;
                        case CharacterAttackType.BloodSlash:

                            break;
                        case CharacterAttackType.DeadSlash:
                            break;
                    }

                    Enemy.GetComponent<IHittable>().OnHit(Attack);
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

    protected BattleArtEnhancement EnhancementEquiped(BattleArtEnhancementType Type, CharacterAttackType AttackType)
    {
        switch (AttackType)
        {
            case CharacterAttackType.BloodSlash:
                for(int i = 0; i < Context.BloodSlashEnhancements.Count; i++)
                {
                    if(Context.BloodSlashEnhancements[i].Type == Type)
                    {
                        return Context.BloodSlashEnhancements[i];
                    }
                }
                break;
            case CharacterAttackType.DeadSlash:
                for (int i = 0; i < Context.DeadSlashEnhancements.Count; i++)
                {
                    if (Context.DeadSlashEnhancements[i].Type == Type)
                    {
                        return Context.DeadSlashEnhancements[i];
                    }
                }
                break;
        }

        return new BattleArtEnhancement("",BattleArtEnhancementType.Null,0);
    }

    protected void AddCriticalEye(BattleArtEnhancement Enhancement, CharacterAttackInfo Attack)
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        if (!Entity.GetComponent<CriticalEyeManager>())
        {
            Entity.AddComponent<CriticalEyeManager>();

            if (Enhancement.Level == 1)
            {
                Entity.GetComponent<CriticalEyeManager>().Bonus = AbilityData.CriticalEyeBonusLv1;
            }
            else
            {
                Entity.GetComponent<CriticalEyeManager>().Bonus = AbilityData.CriticalEyeBonusLv2;
            }
            Entity.GetComponent<CriticalEyeManager>().Type = Attack.Type;
            Entity.GetComponent<CriticalEyeManager>().Icon = Context.CriticalEyeIcon;
        }
    }

    protected void CriticalEyeEnhanceBattleArt(BattleArtEnhancement Enhancement, int HitNumber)
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        if (Enhancement.Level == 3)
        {
            Entity.GetComponent<CriticalEyeManager>().BattleArtBonusNumber += HitNumber;
            if (Entity.GetComponent<CriticalEyeManager>().BattleArtBonusNumber > AbilityData.CriticalEyeMaxBattleArtBonusNumber)
            {
                Entity.GetComponent<CriticalEyeManager>().BattleArtBonusNumber = AbilityData.CriticalEyeMaxBattleArtBonusNumber;
            }
        }
    }

    protected void CriticalEyeBonusToDamage(BattleArtEnhancement Enhancement, CharacterAttackInfo Attack)
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        int Bonus = 0;

        if (Entity.GetComponent<CriticalEyeManager>())
        {
            Bonus += Entity.GetComponent<CriticalEyeManager>().Bonus;
        }

        if(Attack.Type != CharacterAttackType.NormalSlash && Attack.Type != Entity.GetComponent<CriticalEyeManager>().Type)
        {
            Bonus += Mathf.FloorToInt(Attack.BaseDamage * AbilityData.CriticalEyeBattleArtBonusUnit * Entity.GetComponent<CriticalEyeManager>().BattleArtBonusNumber);
        }

        Attack.Damage += Bonus;
    }

    protected void HarmonyHeal(BattleArtEnhancement Enhancement,int HitNumber, int Cost)
    {
        if(HitNumber == 0)
        {
            return;
        }

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        int HealNum = 0;
        if(Enhancement.Level == 1)
        {
            HealNum = AbilityData.HarmonyHealAmountLv1;
        }
        else
        {
            HealNum = AbilityData.HarmonyHealAmountLv2;
        }

        HealNum *= HitNumber;

        if (Enhancement.Level == 3)
        {
            HealNum *= Cost;
        }

        Heal(HealNum);
    }

    protected void Heal(int amount)
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();
        if (amount > Data.MaxHP - Status.CurrentHP)
        {
            amount = Data.MaxHP - Status.CurrentHP;
        }

        if (amount > 0)
        {
            GameObject DamageText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/DamageText"), Entity.transform.position, Quaternion.Euler(0, 0, 0));
            DamageText.GetComponent<DamageText>().TravelVector = Vector2.up;
            DamageText.GetComponent<Text>().color = Color.green;
            DamageText.transform.parent = Status.Canvas.transform;
            DamageText.GetComponent<Text>().text = amount.ToString();
            Status.CurrentHP += amount;
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

            if (LeftHit && LeftHit.collider.gameObject.CompareTag("PassablePlatform") && RightHit && RightHit.collider.gameObject.CompareTag("PassablePlatform"))
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

    protected bool CharacterFallPlatform()
    {
        if(Context.AttachedPassablePlatform && Utility.InputDown() && Utility.InputJump())
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

            var SpeedManager = Entity.GetComponent<SpeedManager>();
            var Data = Entity.GetComponent<CharacterData>();

            RaycastHit2D LeftHit = Physics2D.Raycast(SpeedManager.GetTruePos() + SpeedManager.BodyWidth / 2 * Vector2.left + SpeedManager.BodyHeight / 2 * Vector2.down, Vector2.down, 0.02f, Data.PassablePlatformLayer);
            RaycastHit2D RightHit = Physics2D.Raycast(SpeedManager.GetTruePos() + SpeedManager.BodyWidth / 2 * Vector2.right + SpeedManager.BodyHeight / 2 * Vector2.down, Vector2.down, 0.02f, Data.PassablePlatformLayer);

            LeftHit.collider.GetComponent<PassableInfo>().TopPassable = true;
            RightHit.collider.GetComponent<PassableInfo>().TopPassable = true;
            LeftHit.collider.gameObject.GetComponent<PassablePlatform>().Player = Entity;
            RightHit.collider.gameObject.GetComponent<PassablePlatform>().Player = Entity;

            Context.AttachedPassablePlatform.GetComponent<PassableInfo>().TopPassable = true;
            Context.AttachedPassablePlatform.GetComponent<PassablePlatform>().Player = Entity;
            return true;
        }
        else
        {
            return false;
        }
    }

    

    protected bool CheckGrounded() 
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (SpeedManager.HitGround && SpeedManager.SelfSpeed.y<=0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckCharacterNormalSlash()
    {
        bool Usable = true;

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        if (!CheckGrounded() && !AbilityData.NormalSlashAirUsable)
        {
            Usable = false;
        }
        //return Utility.InputNormalSlash();
        if (Usable && Context.SavedInputInfo.Count>0 && Context.SavedInputInfo[0].Type == InputType.NormalSlash)
        {
            return true;
        }

        return false;
    }

    protected bool CheckCharacterFirstSkill()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        if (Context.SavedInputInfo.Count > 0 && Context.SavedInputInfo[0].Type == InputType.FirstSkill && Status.CurrentEnergy >= AbilityData.BloodSlashEnergyCost)
        {
            return true;
        }
        return false;
    }

    protected bool CheckCharacterSecondSkill()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        if(CheckGrounded() && Context.SavedInputInfo.Count > 0 && Context.SavedInputInfo[0].Type == InputType.SecondSkill && Status.CurrentEnergy >= AbilityData.DeadSlashEnergyCost)
        {
            return true;
        }
        return false;
    }

    protected bool CheckCharacterJump()
    {
        if(Context.SavedInputInfo.Count>0 && Context.SavedInputInfo[0].Type == InputType.Jump)
        {
            return true;
        }

        return false;
    }

    protected bool CheckCharacterRoll()
    {
        if (Context.RollCoolDownTimeCount<=0 && Context.SavedInputInfo.Count > 0 && Context.SavedInputInfo[0].Type == InputType.Roll && CheckGrounded())
        {
            return true;
        }

        return false;
    }

    protected bool CheckCharacterMove<StopState>(bool Ground, bool Transition) where StopState: CharacterActionState
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

        bool HaveInput = false;

        if (MoveVector.x > 0)
        {
            HaveInput = true;
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
            HaveInput = true;
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
            HaveInput = false;
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
                    if (Transition)
                    {
                        TransitionTo<StopState>();
                    }
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
                    if (Transition)
                    {
                        TransitionTo<StopState>();
                    }
                }
            }
        }

        return HaveInput;
    }
}

public class Stand : CharacterActionState
{

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
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

        if (CheckCharacterRoll())
        {
            TransitionTo<RollAnticipation>();
            return;
        }

        DetectPassablePlatform();
        if (CharacterFallPlatform())
        {
            TransitionTo<AirStay>();
            return;
        }

        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return;
        }

        if (CharacterDownToLadder())
        {
            TransitionTo<DownToLadder>();
            return;
        }

        if (CheckCharacterJump())
        {
            Context.SavedInputInfo.Clear();
            Context.JumpHoldingTimeCount = 0;
            TransitionTo<JumpHoldingStay>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (!CheckGrounded())
        {
            TransitionTo<AirStay>();
            return;
        }

        if (CheckCharacterMove<Stand>(true,false))
        {
            TransitionTo<GroundMove>();
            return;
        }
    }
}

public class GroundMove : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
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

        if (CheckCharacterRoll())
        {
            TransitionTo<RollAnticipation>();
            return;
        }

        DetectPassablePlatform();
        if (CharacterFallPlatform())
        {
            TransitionTo<AirStay>();
            return;
        }


        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return;
        }

        if (CharacterDownToLadder())
        {
            TransitionTo<DownToLadder>();
            return;
        }

        if (CheckCharacterJump())
        {
            Context.SavedInputInfo.Clear();
            Context.JumpHoldingTimeCount = 0;
            TransitionTo<JumpHoldingMove>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (!CheckGrounded())
        {
            TransitionTo<AirMove>();
            return;
        }

        CheckCharacterMove<Stand>(true,true);
    }
}

public class JumpHoldingStay : CharacterActionState
{

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentGravity = 0;
        Entity.GetComponent<SpeedManager>().SelfSpeed.y = Entity.GetComponent<CharacterData>().JumpSpeed;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
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

        if (CheckHolding())
        {
            return;
        }

        if (CheckGrounded())
        {
            TransitionTo<Stand>();
            return;
        }

        if (CharacterClimbPlatform())
        {
            TransitionTo<ClimbPlatform>();
            return;
        }


        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterMove<JumpHoldingStay>(false,false))
        {
            TransitionTo<JumpHoldingMove>();
            return;
        }
    }

    private bool CheckHolding()
    {
        if (!Utility.InputJumpHold())
        {
            TransitionTo<AirStay>();
            return true;
        }

        Context.JumpHoldingTimeCount += Time.deltaTime;
        if (Context.JumpHoldingTimeCount >= Entity.GetComponent<CharacterData>().JumpHoldingTime)
        {
            TransitionTo<AirStay>();
            return true;
        }

        return false;
    }
}

public class JumpHoldingMove : CharacterActionState
{

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentGravity = 0;
        Entity.GetComponent<SpeedManager>().SelfSpeed.y = Entity.GetComponent<CharacterData>().JumpSpeed;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
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

        if (CheckHolding())
        {
            return;
        }
        if (CheckGrounded())
        {
            TransitionTo<GroundMove>();
            return;
        }

        if (CharacterClimbPlatform())
        {
            TransitionTo<ClimbPlatform>();
            return;
        }


        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        CheckCharacterMove<JumpHoldingStay>(false,true);
    }


    private bool CheckHolding()
    {
        if (!Utility.InputJumpHold())
        {
            TransitionTo<AirMove>();
            return true;
        }

        Context.JumpHoldingTimeCount += Time.deltaTime;
        if (Context.JumpHoldingTimeCount >= Entity.GetComponent<CharacterData>().JumpHoldingTime)
        {
            TransitionTo<AirMove>();
            return true;
        }

        return false;
    }
}

public class AirStay : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
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

        if (CheckGrounded())
        {
            TransitionTo<Stand>();
            return;
        }
        if (CharacterClimbPlatform())
        {
            TransitionTo<ClimbPlatform>();
            return;
        }

        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        CheckCharacterMove<AirStay>(false,false);
    }
}

public class AirMove : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
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

        if (CheckGrounded())
        {
            TransitionTo<GroundMove>();
            return;
        }
        if (CharacterClimbPlatform())
        {
            TransitionTo<ClimbPlatform>();
            return;
        }

        if (CharacterClimbLadder())
        {
            TransitionTo<ClimbLadder>();
            return;
        }

        if (CheckCharacterNormalSlash())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.NormalSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterFirstSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.BloodSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        if (CheckCharacterSecondSkill())
        {
            Context.SavedInputInfo.Clear();
            Context.CurrentAttackType = CharacterAttackType.DeadSlash;
            TransitionTo<SlashAnticipation>();
            return;
        }

        CheckCharacterMove<AirStay>(false,true);
    }
}

public class SlashAnticipation : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeed();
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
        TimeCount = 0;

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        switch (Context.CurrentAttackType)
        {
            case CharacterAttackType.NormalSlash:
                StateTime = AbilityData.NormalSlashAnticipationTime;
                break;
            case CharacterAttackType.BloodSlash:
                StateTime = AbilityData.BloodSlashAnticipationTime;
                break;
            case CharacterAttackType.DeadSlash:
                StateTime = AbilityData.DeadSlashAnticipationTime;
                break;
        }

    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        switch (Context.CurrentAttackType)
        {
            case CharacterAttackType.NormalSlash:
                CurrentSpriteSeries = SpriteData.LightAnticipationSeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
                break;
            case CharacterAttackType.BloodSlash:
                CurrentSpriteSeries = SpriteData.LightAnticipationSeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
                break;
            case CharacterAttackType.DeadSlash:
                CurrentSpriteSeries = SpriteData.HeavyAnticipationSeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyAnticipationOffset, SpriteData.HeavyAnticipationSize);
                break;
        }
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<SlashStrike>();
            return;
        }
    }

    private void SetSpeed()
    {
        if (!CheckGrounded())
        {
            Context.CurrentGravity = 0;
            Entity.GetComponent<SpeedManager>().SelfSpeed.y = 0;
        }
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}

public class SlashStrike : CharacterActionState
{
    private float StateTime;
    private int Damage;
    private int EnergyCost;
    private Vector2 Offset;
    private Vector2 Size;
    private float StepForwardSpeed;
    private GameObject Image;

    private BattleArtEnhancement CriticleEyeEnhancement;
    private BattleArtEnhancement HarmonyEnhancement;
    private CharacterAttackInfo AttackInfo;
    private List<GameObject> EnemyHit;
    private float TimeCount;

    private bool HitAnything;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeed();
    }

    public override void Update()
    {
        base.Update();
        CheckHitEnemy();
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();
        Status.CurrentEnergy -= EnergyCost;
        GameObject.Destroy(SlashImage);

        CheckHarmonyHeal();
        CheckRemoveCriticalEye();
    }

    private void CheckHarmonyHeal()
    {
        if (AttackInfo.Type != CharacterAttackType.NormalSlash && HarmonyEnhancement.Type != BattleArtEnhancementType.Null && EnemyHit.Count > 0)
        {
            HarmonyHeal(HarmonyEnhancement, EnemyHit.Count, AttackInfo.Cost);
        }
    }

    private void CheckRemoveCriticalEye()
    {
        var CriticalEye = Entity.GetComponent<CriticalEyeManager>();

        if (CriticalEye && AttackInfo.Type != CharacterAttackType.NormalSlash && AttackInfo.Type != CriticalEye.Type)
        {
            CriticalEye.DestroySelf();
        }
    }


    private void SetUp()
    {
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        TimeCount = 0;
        switch (Context.CurrentAttackType)
        {
            case CharacterAttackType.NormalSlash:
                SetAttribute(AbilityData.NormalSlashStrikeTime, AbilityData.NormalSlashDamage,
                    0, AbilityData.NormalSlashOffset, AbilityData.NormalSlashHitBoxSize,
                    AbilityData.NormalSlashImage, AbilityData.NormalSlashStepForwardSpeed);

                break;
            case CharacterAttackType.BloodSlash:
                SetAttribute(AbilityData.BloodSlashStrikeTime, AbilityData.BloodSlashDamage,
                    AbilityData.BloodSlashEnergyCost, AbilityData.BloodSlashOffset, AbilityData.BloodSlashHitBoxSize,
                    AbilityData.BloodSlashImage, AbilityData.BloodSlashStepForwardSpeed);
                break;
            case CharacterAttackType.DeadSlash:
                SetAttribute(AbilityData.DeadSlashStrikeTime, AbilityData.DeadSlashDamage, 
                    AbilityData.DeadSlashEnergyCost, AbilityData.DeadSlashOffset, AbilityData.DeadSlashHitBoxSize,
                    AbilityData.DeadSlashImage, 0);
                break;
        }
        EnemyHit = new List<GameObject>();
        AttackInfo = new CharacterAttackInfo(Entity, Context.CurrentAttackType, Entity.transform.right.x > 0, Damage, Damage, EnergyCost, EnergyCost, Offset, Size);
        HitAnything = false;
        GenerateSlashImage(Image, AttackInfo);


        CriticleEyeEnhancement = EnhancementEquiped(BattleArtEnhancementType.CriticleEye, AttackInfo.Type);
        HarmonyEnhancement = EnhancementEquiped(BattleArtEnhancementType.Harmony, AttackInfo.Type);

        if (Entity.GetComponent<CriticalEyeManager>())
        {
            CriticalEyeBonusToDamage(CriticleEyeEnhancement,AttackInfo);
        }
    }

    private void SetAttribute(float time, int damage, int cost, Vector2 offset, Vector2 size, GameObject image, float stepspeed)
    {
        StateTime = time;
        Damage = damage;
        EnergyCost = cost;
        Offset = offset;
        Size = size;
        Image = image;
        StepForwardSpeed = stepspeed;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        switch (Context.CurrentAttackType)
        {
            case CharacterAttackType.NormalSlash:
                CurrentSpriteSeries = SpriteData.LightRecoverySeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
                break;
            case CharacterAttackType.BloodSlash:
                CurrentSpriteSeries = SpriteData.LightRecoverySeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
                break;
            case CharacterAttackType.DeadSlash:
                CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
                break;
        }
    }

    private void GenerateSlashImage(GameObject Image, CharacterAttackInfo Attack)
    {
        float EulerAngle = 0;

        if (!Attack.Right)
        {
            EulerAngle = 180;
        }

        var Data = Entity.GetComponent<CharacterData>();

        Vector2 Offset = Attack.HitBoxOffset;

        if (!Attack.Right)
        {
            Offset.x = -Offset.x;
        }

        SlashImage = GameObject.Instantiate(Image, (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        SlashImage.transform.parent = Entity.transform;
    }

    private void SetSpeed()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var AbilityData = Entity.GetComponent<CharacterAbilityData>();
        Context.CurrentGravity = Data.NormalGravity;

        if (CheckGrounded())
        {
            if (Entity.transform.right.x > 0 && Utility.InputRight() && !Utility.InputLeft())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = StepForwardSpeed;
            }
            else if (Entity.transform.right.x < 0 && !Utility.InputRight() && Utility.InputLeft())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -StepForwardSpeed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            }
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > StateTime)
        {
            TransitionTo<SlashRecovery>();
            return;
        }
    }

    private void CheckHitEnemy()
    {
        if (HitEnemy(AttackInfo, EnemyHit))
        {
            if (!HitAnything && CriticleEyeEnhancement.Type !=BattleArtEnhancementType.Null)
            {
                HitAnything = true;
                AddCriticalEye(CriticleEyeEnhancement, AttackInfo);
            }
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

}

public class SlashRecovery : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeed();
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
        Context.InRecovery = false;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > StateTime)
        {
            RecoveryTransition();
        }
    }

    private void SetUp()
    {
        TimeCount = 0;
        Context.InRecovery = true;

        var AbilityData = Entity.GetComponent<CharacterAbilityData>();

        switch (Context.CurrentAttackType)
        {
            case CharacterAttackType.NormalSlash:
                StateTime = AbilityData.NormalSlashRecoveryTime;
                break;
            case CharacterAttackType.BloodSlash:
                StateTime = AbilityData.BloodSlashRecoveryTime;
                break;
            case CharacterAttackType.DeadSlash:
                StateTime = AbilityData.DeadSlashRecoveryTime;
                break;
        }

    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();

        switch (Context.CurrentAttackType)
        {
            case CharacterAttackType.NormalSlash:
                CurrentSpriteSeries = SpriteData.LightRecoverySeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
                break;
            case CharacterAttackType.BloodSlash:
                CurrentSpriteSeries = SpriteData.LightRecoverySeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
                break;
            case CharacterAttackType.DeadSlash:
                CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;
                SetCharacterSprite();
                Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
                break;
        }
    }

    private void SetSpeed()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}

public class RollAnticipation : CharacterActionState
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
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.RollAnticipationTime;
        Entity.GetComponent<SpeedManager>().SelfSpeed = Vector2.zero;
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
            TransitionTo<Roll>();
        }
    }
}

public class Roll: CharacterActionState
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

        if (HitWall())
        {

            TransitionTo<Stand>();
            return;
        }

        if (!CheckGrounded())
        {
            TransitionTo<AirStay>();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        var Data = Entity.GetComponent<CharacterData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        SpeedManager.IgnoredLayers = Data.NormalIgnoredLayers;
        SpeedManager.SelfSpeed.x = 0;

        Context.RollCoolDownTimeCount = Entity.GetComponent<CharacterData>().RollCoolDown;
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
            TransitionTo<Stand>();
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
            TransitionTo<Stand>();
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
            TransitionTo<AirStay>();
            return;
        }

        if (!OnLadder())
        {
            TransitionTo<AirStay>();
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
    private float TimeCount;
    
    public override void OnEnter()
    {
        base.OnEnter();

        SetUp();

        SetAppearance();

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

        Entity.GetComponent<StatusManager_Character>().CurrentEnergy = 0;
        Entity.GetComponent<IHittable>().Interrupted = false;
        Context.CurrentGravity = Data.NormalGravity;

        EnemyAttackInfo Temp = (EnemyAttackInfo)Entity.GetComponent<IHittable>().HitAttack;
        SpeedManager.SelfSpeed = Vector2.zero;
        if (Temp.Right)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);

            SpeedManager.SelfSpeed.x = Data.InterruptedSpeedX;
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);

            SpeedManager.SelfSpeed.x = -Data.InterruptedSpeedX;

        }
    }

    private void CheckTime()
    {
        var Data = Entity.GetComponent<CharacterData>();

        TimeCount += Time.deltaTime;

        if(TimeCount >= Data.InterruptedTime)
        {
            if (CheckGrounded())
            {
                TransitionTo<GroundMove>();
            }
            else
            {
                TransitionTo<AirMove>();
            }
        }
        else if(TimeCount >= Data.InterruptedMoveTime)
        {
            if (CheckGrounded())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            }
        }
    }
}

