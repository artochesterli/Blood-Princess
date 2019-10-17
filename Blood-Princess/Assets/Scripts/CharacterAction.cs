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
    DeadSlash
}

public class CharacterAction : MonoBehaviour
{
    public LayerMask EnemyLayers;
    public bool InRecovery;

    private FSM<CharacterAction> CharacterActionFSM; 
    // Start is called before the first frame update
    void Start()
    {
        CharacterActionFSM = new FSM<CharacterAction>(this);
        CharacterActionFSM.TransitionTo<Stand>();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterActionFSM.Update();
    }
}

public abstract class CharacterActionState : FSM<CharacterAction>.State
{
    protected GameObject Entity;
    protected float CurrentGravity;
    protected float JumpHoldingTimeCount;
    protected CharacterAttackInfo Attack;
    protected GameObject SlashImage;
    protected List<Sprite> CurrentSpriteSeries;

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
        Entity.GetComponent<SpeedManager>().SelfSpeed.y -= CurrentGravity * Time.deltaTime * 10;
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

        if(Status.CurrentEnergy == 2)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[3];
        }
        else if(Status.CurrentEnergy == 1)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[2];
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

        RaycastHit2D[] AllHits = Physics2D.BoxCastAll(Entity.transform.position + (Vector3)Offset, Attack.HitBoxSize, 0, Entity.transform.right, 0, Context.EnemyLayers);
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

        if (SpeedManager.HitGround && SpeedManager.SelfSpeed.y+SpeedManager.ForcedSpeed.y<=0)
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
        return Utility.InputNormalSlash();
    }

    protected bool CheckCharacterBloodSlash()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        return Utility.InputBloodSlash() && Status.CurrentEnergy >= Data.BloodSlashEnergyCost;
    }

    protected bool CheckCharacterDeadSlash()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        return Utility.InputDeadSlash() && Status.CurrentEnergy >= Data.DeadSlashEnergyCost;
    }

    protected bool CheckCharacterJump<JumpState>() where JumpState : CharacterActionState
    {
        if (Utility.InputJump())
        {
            TransitionTo<JumpState>();
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
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

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

        if (Utility.InputBlock())
        {
            TransitionTo<Block>();
            return;
        }

        if (CheckCharacterJump<JumpHoldingStay>())
        {
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
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

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

        if (Utility.InputBlock())
        {
            TransitionTo<Block>();
            return;
        }

        if (CheckCharacterJump<JumpHoldingMove>())
        {
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

        JumpHoldingTimeCount = 0;
        CurrentGravity = 0;
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
        }
    }

    private bool CheckHolding()
    {
        if (!Utility.InputJumpHold())
        {
            TransitionTo<AirStay>();
            return true;
        }

        JumpHoldingTimeCount += Time.deltaTime;
        if (JumpHoldingTimeCount >= Entity.GetComponent<CharacterData>().JumpHoldingTime)
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

        JumpHoldingTimeCount = 0;
        CurrentGravity = 0;
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

        JumpHoldingTimeCount += Time.deltaTime;
        if (JumpHoldingTimeCount >= Entity.GetComponent<CharacterData>().JumpHoldingTime)
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
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

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
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

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
            CurrentGravity = 0;
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
        CurrentGravity = Data.NormalGravity;

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
        CurrentGravity = Data.NormalGravity;
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
            CurrentGravity = 0;
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
        CurrentGravity = Data.NormalGravity;

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
        CurrentGravity = Data.NormalGravity;
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
        CurrentGravity = Data.NormalGravity;
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
            TransitionTo<Stand>();
            return;
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
        CurrentGravity = Data.NormalGravity;
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

public class Block : CharacterActionState
{
    private float TimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        TimeCount = 0;
        var Status = Entity.GetComponent<StatusManager_Character>();
        if (Status.CurrentEnergy > 0)
        {
            Status.CurrentEnergy--;
            Status.Invulnerable = true;
            Status.InvulnerableMark.SetActive(true);
        }

        Status.Blocking = true;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HitOffset, SpriteData.HitSize);
    }

    public override void Update()
    {
        base.Update();
        if (!Utility.InputBlock())
        {
            TransitionTo<Stand>();
            return;
        }

        if (CheckGetInterrupted())
        {
            TransitionTo<GetInterrupted>();
            return;
        }

        if (Utility.InputLeft() && !Utility.InputRight())
        {
            Entity.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if(!Utility.InputLeft() && Utility.InputRight())
        {
            Entity.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        CheckBlockTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        var Status = Entity.GetComponent<StatusManager_Character>();
        Status.Blocking = false;
        Status.Invulnerable = false;
        Status.InvulnerableMark.SetActive(false);
    }

    private void CheckBlockTime()
    {
        TimeCount += Time.deltaTime;
        var Status = Entity.GetComponent<StatusManager_Character>();
        var Data = Entity.GetComponent<CharacterData>();
        if (TimeCount >= Data.BlockInvulnerableTime)
        {
            Status.Invulnerable = false;
            Status.InvulnerableMark.SetActive(false);
        }
    }
}

public class GetInterrupted : CharacterActionState
{
    private float GetHitTimeCount;
    private bool FinishOnGround;

    public override void OnEnter()
    {
        base.OnEnter();

        Entity.GetComponent<StatusManager_Character>().CurrentEnergy = 0;

        EnemyAttackInfo Temp = (EnemyAttackInfo)Entity.GetComponent<IHittable>().HitAttack;

        float HitSpeed;
        if (Temp.Right)
        {
            HitSpeed = Entity.GetComponent<CharacterData>().GetHitSpeed;
        }
        else
        {
            HitSpeed = -Entity.GetComponent<CharacterData>().GetHitSpeed;
        }

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        Entity.GetComponent<SpeedManager>().ForcedSpeed.x = HitSpeed;
        GetHitTimeCount = 0;

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HitSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HitOffset, SpriteData.HitSize);

        GameObject.Destroy(SlashImage);
    }

    public override void Update()
    {
        base.Update();
        GetHitTimeCount += Time.deltaTime;
        if(GetHitTimeCount>= Entity.GetComponent<CharacterData>().GetHitTime)
        {
            if (CheckGrounded())
            {
                FinishOnGround = true;
                TransitionTo<Stand>();
                return;
            }

            FinishOnGround = false;
            TransitionTo<AirStay>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (!FinishOnGround)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.GetComponent<SpeedManager>().ForcedSpeed.x;
        }
        Entity.GetComponent<SpeedManager>().ForcedSpeed.x = 0;
        Entity.GetComponent<IHittable>().Interrupted = false;
    }
}

