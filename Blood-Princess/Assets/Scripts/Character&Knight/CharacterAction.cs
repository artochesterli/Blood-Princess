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
    public Vector2 HitBoxOffset;
    public Vector2 HitBoxSize;
    public int Level;
    public CharacterAttackInfo(GameObject source, CharacterAttackType type, bool right, int damage, Vector2 offset, Vector2 size,int level = 0)
    {
        Source = source;
        Type = type;
        Right = right;
        Damage = damage;
        HitBoxOffset = offset;
        HitBoxSize = size;
        Level = level;
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


public enum CharacterAttackType
{
    NormalSlash,
    BloodSlash,
    DeadSlash,
    Explosion
}

public enum InputType
{
    Jump,
    NormalSlash,
    BloodSlash,
    DeadSlash,
    Roll
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
    public bool InBuff;
    public float CurrentBuffTime;

    public List<InputInfo> SavedInputInfo;
    private FSM<CharacterAction> CharacterActionFSM;
    private float BuffTimeCount;

    // Start is called before the first frame update
    void Start()
    {
        SavedInputInfo = new List<InputInfo>();
        CharacterActionFSM = new FSM<CharacterAction>(this);
        CharacterActionFSM.TransitionTo<Stand>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSavedInputInfo();
        GetInput();
        CharacterActionFSM.Update();
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

    private void GetInput()
    {
        if (Utility.InputJump())
        {
            SavedInputInfo.Add(new InputInfo(InputType.Jump));
        }

        if (Utility.InputNormalSlash())
        {
            SavedInputInfo.Add(new InputInfo(InputType.NormalSlash));
        }

        if (Utility.InputBloodSlash())
        {
            SavedInputInfo.Add(new InputInfo(InputType.BloodSlash));
        }

        if (Utility.InputDeadSlash())
        {
            SavedInputInfo.Add(new InputInfo(InputType.DeadSlash));
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

    protected CharacterAttackInfo Attack;
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            Context.SavedInputInfo.Clear();
            TransitionTo<BloodSlashAnticipation>();
            return;
        }

        if (CheckCharacterDeadSlash())
        {
            Context.SavedInputInfo.Clear();
            TransitionTo<DeadSlashAnticipation>();
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
            int AvailableCount = 0;

            for(int i = 0; i < AllHits.Length; i++)
            {
                if (!EnemyHit.Contains(AllHits[i].collider.gameObject))
                {
                    AllHits[i].collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
                    EnemyHit.Add(AllHits[i].collider.gameObject);
                    AvailableCount++;
                }
            }

            switch (Attack.Type)
            {
                case CharacterAttackType.NormalSlash:

                    Status.CurrentEnergy += AvailableCount;
                    if (Status.CurrentEnergy > Data.MaxEnergy)
                    {
                        Status.CurrentEnergy = Data.MaxEnergy;
                    }
                    break;
                case CharacterAttackType.BloodSlash:
                    break;
                case CharacterAttackType.DeadSlash:
                    int Drained = Data.DeadSlashHeal * AvailableCount;
                    if (Drained > Data.MaxHP - Status.CurrentHP)
                    {
                        Drained = Data.MaxHP - Status.CurrentHP;
                    }

                    if (Drained > 0)
                    {
                        GameObject DamageText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/DamageText"), Entity.transform.position, Quaternion.Euler(0, 0, 0));
                        DamageText.GetComponent<DamageText>().TravelVector = Vector2.up;
                        DamageText.GetComponent<Text>().color = Color.green;
                        DamageText.transform.parent = Status.Canvas.transform;
                        DamageText.GetComponent<Text>().text = Drained.ToString();
                        Status.CurrentHP += Drained;
                    }
                    break;
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

    protected void GenerateSlashImage(CharacterAttackType Type,CharacterAttackInfo Attack)
    {
        float EulerAngle = 0;

        if (!Attack.Right)
        {
            EulerAngle = 180;
        }

        var Data = Entity.GetComponent<CharacterData>();

        Vector2 Offset;

        switch (Type)
        {
            case CharacterAttackType.NormalSlash:
                Offset = Data.NormalSlashOffset;
                if (!Attack.Right)
                {
                    Offset.x = -Offset.x;
                }
                SlashImage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/NormalSlash"), (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
                break;
            case CharacterAttackType.BloodSlash:
                Offset = Data.BloodSlashOffset;
                if (!Attack.Right)
                {
                    Offset.x = -Offset.x;
                }
                SlashImage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/BloodSlash"), (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
                break;
            case CharacterAttackType.DeadSlash:
                Offset = Data.DeadSlashOffset;
                if (!Attack.Right)
                {
                    Offset.x = -Offset.x;
                }
                SlashImage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/DeadSlash"), (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
                break;
        }
        SlashImage.transform.parent = Entity.transform;
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
        //return Utility.InputNormalSlash();
        if(Context.SavedInputInfo.Count>0 && Context.SavedInputInfo[0].Type == InputType.NormalSlash)
        {
            return true;
        }

        return false;
    }

    protected bool CheckCharacterBloodSlash()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        if (Context.SavedInputInfo.Count > 0 && Context.SavedInputInfo[0].Type == InputType.BloodSlash && Status.CurrentEnergy >= Data.BloodSlashEnergyCost)
        {
            return true;
        }

        return false;

        //return Utility.InputBloodSlash() && Status.CurrentEnergy >= Data.BloodSlashEnergyCost;
    }

    protected bool CheckCharacterDeadSlash()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        if (Context.SavedInputInfo.Count > 0 && Context.SavedInputInfo[0].Type == InputType.DeadSlash && Status.CurrentEnergy >= Data.DeadSlashEnergyCost)
        {
            return true;
        }

        return false;
        //return Utility.InputDeadSlash() && Status.CurrentEnergy >= Data.DeadSlashEnergyCost;
    }

    protected bool CheckCharacterBlink()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();
        if(Status.CurrentEnergy >= Data.InvulberableEnergyCost && Utility.InputBlink())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckCharacterJump()
    {
        /*if (Utility.InputJump())
        {
            return true;
        }*/

        if(Context.SavedInputInfo.Count>0 && Context.SavedInputInfo[0].Type == InputType.Jump)
        {
            return true;
        }

        return false;
    }

    protected bool CheckCharacterRoll()
    {
        if (Context.SavedInputInfo.Count > 0 && Context.SavedInputInfo[0].Type == InputType.Roll && CheckGrounded())
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

        if (Utility.InputRoll())
        {
            TransitionTo<RollAnticipation>();
            return;
        }

        if (CheckCharacterBlink())
        {
            TransitionTo<BlinkAnticipation>();
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            TransitionTo<BloodSlashAnticipation>();
            return;
        }

        if (CheckCharacterDeadSlash())
        {
            TransitionTo<DeadSlashAnticipation>();
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

        if (CheckCharacterBlink())
        {
            TransitionTo<BlinkAnticipation>();
            return;
        }
        if (Utility.InputRoll())
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            TransitionTo<BloodSlashAnticipation>();
            return;
        }

        if (CheckCharacterDeadSlash())
        {
            TransitionTo<DeadSlashAnticipation>();
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

        if (CheckCharacterBlink())
        {
            TransitionTo<BlinkAnticipation>();
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            TransitionTo<BloodSlashAnticipation>();
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

        if (CheckCharacterBlink())
        {
            TransitionTo<BlinkAnticipation>();
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            TransitionTo<BloodSlashAnticipation>();
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

        if (CheckCharacterBlink())
        {
            TransitionTo<BlinkAnticipation>();
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            TransitionTo<BloodSlashAnticipation>();
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

        if (CheckCharacterBlink())
        {
            TransitionTo<BlinkAnticipation>();
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
            TransitionTo<NormalSlashAnticipation>();
            return;
        }

        if (CheckCharacterBloodSlash())
        {
            TransitionTo<BloodSlashAnticipation>();
            return;
        }

        CheckCharacterMove<AirStay>(false,true);
    }
}

public class NormalSlashAnticipation : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<NormalSlashStrike>();
            return;
        }
    }

    private void SetUp()
    {
        TimeCount = 0;
        StateTime = Entity.GetComponent<CharacterData>().NormalSlashAnticipationTime;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void SetSpeedInfo()
    {
        if (!CheckGrounded())
        {
            Context.CurrentGravity = 0;
            Entity.GetComponent<SpeedManager>().SelfSpeed.y = 0;
        }
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}

public class NormalSlashStrike : CharacterActionState
{
    private float StateTime;
    private float TimeCount;
    private CharacterAttackInfo AttackInfo;
    private List<GameObject> EnemyHit;


    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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
        GameObject.Destroy(SlashImage);
    }

    private void CheckHitEnemy()
    {
        if (HitEnemy(AttackInfo,EnemyHit))
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > StateTime)
        {
            TransitionTo<NormalSlashRecovery>();
            return;
        }
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.NormalSlashStrikeTime;
        EnemyHit = new List<GameObject>();
        AttackInfo = new CharacterAttackInfo(Entity, CharacterAttackType.NormalSlash, Entity.transform.right.x > 0, Data.NormalSlashDamage, Data.NormalSlashOffset, Data.NormalSlashHitBoxSize);
        GenerateSlashImage(CharacterAttackType.NormalSlash,AttackInfo);
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightRecoverySeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
    }

    private void SetSpeedInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;

        if (CheckGrounded())
        {
            if (Entity.transform.right.x > 0 && Utility.InputRight() && !Utility.InputLeft())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.NormalSlashStepForwardSpeed;
            }
            else if (Entity.transform.right.x < 0 && !Utility.InputRight() && Utility.InputLeft())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.NormalSlashStepForwardSpeed;
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
    
}

public class NormalSlashRecovery : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightRecoverySeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
    }

    private void SetSpeedInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetUp()
    {
        TimeCount = 0;
        var Data = Entity.GetComponent<CharacterData>();
        StateTime = Data.NormalSlashRecoveryTime;
        Context.InRecovery = true;
    }
}

public class BloodSlashAnticipation : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<BloodSlashStrike>();
            return;
        }
    }

    private void SetUp()
    {
        TimeCount = 0;
        StateTime = Entity.GetComponent<CharacterData>().BloodSlashAnticipationTime;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    private void SetSpeedInfo()
    {
        if (!CheckGrounded())
        {
            Context.CurrentGravity = 0;
            Entity.GetComponent<SpeedManager>().SelfSpeed.y = 0;
        }
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}

public class BloodSlashStrike : CharacterActionState
{
    private float StateTime;
    private float TimeCount;
    private CharacterAttackInfo AttackInfo;
    private List<GameObject> EnemyHit;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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
        Status.CurrentEnergy -= Data.BloodSlashEnergyCost;

        GameObject.Destroy(SlashImage);
    }

    private void CheckHitEnemy()
    {
        if (HitEnemy(AttackInfo,EnemyHit))
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > StateTime)
        {
            TransitionTo<BloodSlashRecovery>();
            return;
        }
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.BloodSlashStrikeTime;
        EnemyHit = new List<GameObject>();
        AttackInfo = new CharacterAttackInfo(Entity, CharacterAttackType.BloodSlash, Entity.transform.right.x > 0, Data.BloodSlashDamage, Data.BloodSlashOffset, Data.BloodSlashHitBoxSize);
        GenerateSlashImage(CharacterAttackType.BloodSlash,AttackInfo);
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightRecoverySeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
    }

    private void SetSpeedInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;

        if (CheckGrounded())
        {
            if (Entity.transform.right.x > 0 && Utility.InputRight() && !Utility.InputLeft())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.BloodSlashStepForwardSpeed;
            }
            else if (Entity.transform.right.x < 0 && !Utility.InputRight() && Utility.InputLeft())
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.BloodSlashStepForwardSpeed;
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
}

public class BloodSlashRecovery : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightRecoverySeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
    }

    private void SetSpeedInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetUp()
    {
        TimeCount = 0;
        var Data = Entity.GetComponent<CharacterData>();
        StateTime = Data.BloodSlashRecoveryTime;
        Context.InRecovery = true;
    }
}

public class DeadSlashAnticipation : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<DeadSlashStrike>();
            return;
        }
    }

    private void SetUp()
    {
        TimeCount = 0;
        StateTime = Entity.GetComponent<CharacterData>().DeadSlashAnticipationTime;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HeavyAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyAnticipationOffset, SpriteData.HeavyAnticipationSize);
    }

    private void SetSpeedInfo()
    {
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}

public class DeadSlashStrike : CharacterActionState
{
    private float StateTime;
    private float TimeCount;
    private CharacterAttackInfo AttackInfo;
    private List<GameObject> EnemyHit;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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
        Status.CurrentEnergy -= Data.DeadSlashEnergyCost;
        GameObject.Destroy(SlashImage);
    }

    private void CheckHitEnemy()
    {
        if (HitEnemy(AttackInfo,EnemyHit))
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > StateTime)
        {
            TransitionTo<DeadSlashRecovery>();
            return;
        }
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.DeadSlashStrikeTime;
        EnemyHit = new List<GameObject>();
        AttackInfo = new CharacterAttackInfo(Entity, CharacterAttackType.DeadSlash, Entity.transform.right.x > 0, Data.DeadSlashDamage, Data.DeadSlashOffset, Data.DeadSlashHitBoxSize);
        GenerateSlashImage(CharacterAttackType.DeadSlash,AttackInfo);
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
    }

    private void SetSpeedInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}

public class DeadSlashRecovery : CharacterActionState
{
    private float StateTime;
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        SetSpeedInfo();
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

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
    }

    private void SetSpeedInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        Context.CurrentGravity = Data.NormalGravity;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetUp()
    {
        TimeCount = 0;
        var Data = Entity.GetComponent<CharacterData>();
        StateTime = Data.DeadSlashRecoveryTime;
        Context.InRecovery = true;
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
            TransitionTo<RollRecovery>();
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
        //Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.GetComponent<SpriteRenderer>().enabled = false;
        //Entity.GetComponent<StatusManager_Character>().Invulnerable = false;
        SpeedManager.IgnoredLayers = Data.NormalIgnoredLayers;
        SpeedManager.SelfSpeed.x = 0;
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

        //Entity.GetComponent<StatusManager_Character>().Invulnerable = true;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
        //Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.GetComponent<SpriteRenderer>().enabled = true;
        Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.transform.position = Entity.GetComponent<SpeedManager>().GetTruePos();
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<RollRecovery>();
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

public class RollRecovery : CharacterActionState
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
        StateTime = Data.RollRecoveryTime;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
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
            RecoveryTransition();
        }
    }
}

public class BlinkAnticipation : CharacterActionState
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
        StateTime = Data.InvulnerableAnticipationTime;
        Context.CurrentGravity = 0;
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
            TransitionTo<BlinkActivated>();
        }
    }
}

public class BlinkActivated : CharacterActionState
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
        Entity.GetComponent<StatusManager_Character>().Invulnerable = false;
        Entity.GetComponent<SpriteRenderer>().enabled = true;
        Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.InvulnerableTime;
        Context.CurrentGravity = 0;
        Entity.GetComponent<StatusManager_Character>().CurrentEnergy -= Data.InvulberableEnergyCost;
        Entity.GetComponent<StatusManager_Character>().Invulnerable = true;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
        Entity.GetComponent<SpriteRenderer>().enabled = false;
        Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.GetComponent<SpriteRenderer>().enabled = true;
        Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.transform.position = Entity.GetComponent<SpeedManager>().GetTruePos();
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<BlinkRecovery>();
        }
    }
}

public class BlinkRecovery : CharacterActionState
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
            SetAppearance();
            TransitionTo<GetInterrupted>();
            return;
        }
        CheckTime();
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<CharacterData>();
        TimeCount = 0;
        StateTime = Data.InvulnerableRecoveryTime;
        Context.CurrentGravity = Data.NormalGravity;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.IdleSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.IdleOffset, SpriteData.IdleSize);
        Entity.GetComponent<SpriteRenderer>().enabled = true;
        Entity.GetComponent<StatusManager_Character>().InvulnerableEffect.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            if (CheckGrounded())
            {
                TransitionTo<Stand>();
            }
            else
            {
                TransitionTo<AirStay>();
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

