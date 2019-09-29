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
    public bool Drain;
    public CharacterAttackInfo(GameObject source, CharacterAttackType type, bool right, int damage, Vector2 offset, Vector2 size,bool drain)
    {
        Source = source;
        Type = type;
        Right = right;
        Damage = damage;
        HitBoxOffset = offset;
        HitBoxSize = size;
        Drain = drain;
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
    Light,
    Heavy
}

public enum AttackState
{
    Anticipation,
    Charge,
    Attack,
    Recovery
}

public class CharacterAction : MonoBehaviour
{
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
    protected bool WallHitted;
    protected float JumpHoldingTimeCount;
    protected float AttackTimeCount;
    protected AttackState CurrentAttackState;
    protected bool AttackHit;
    protected float AnticipationTime;
    protected float AttackTime;
    protected float RecoveryTime;
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
        CheckDrainInput();
    }

    protected bool CheckWallHitting()
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();
        if(!WallHitted&& SpeedManager.SelfSpeed.y>0 && (SpeedManager.HitLeft && Utility.InputLeft() || SpeedManager.HitRight && Utility.InputRight()))
        {
            //TransitionTo<WallHitting>();
            //return true;
            return false;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckGetHit()
    {
        if (Entity.GetComponent<IHittable>().hit)
        {
            TransitionTo<GetHit>();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void SetCharacterSprite()
    {
        var Status = Entity.GetComponent<StatusManager_Character>();
        if (Status.CurrentEnergy >= 10)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[3];
        }
        else if(Status.CurrentEnergy >= 6)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[2];
        }
        else if(Status.CurrentEnergy > 0)
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[1];
        }
        else
        {
            Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[0];
        }
    }

    protected bool AttackHitEnemy(CharacterAttackInfo Attack)
    {
        int Mask = 1 << LayerMask.NameToLayer("Enemy");
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();

        Vector2 Offset = Attack.HitBoxOffset;
        Offset.x = Entity.transform.right.x * Offset.x;

        RaycastHit2D[] AllHits = Physics2D.BoxCastAll(Entity.transform.position + (Vector3)Offset, Attack.HitBoxSize, 0, Entity.transform.right,0, Mask);
        if (AllHits.Length > 0)
        {

            int Count = 0;
            for(int i = 0; i < AllHits.Length; i++)
            {
                if (AllHits[i].collider.gameObject.GetComponent<IHittable>().OnHit(Attack))
                {
                    Count++;
                }
            }

            switch (Attack.Type)
            {
                case CharacterAttackType.Light:
                    Status.CurrentEnergy += Data.LightAttackEnergyGain;
                    if (Status.CurrentEnergy > Data.MaxEnergy)
                    {
                        Status.CurrentEnergy = Data.MaxEnergy;
                    }
                    break;
                case CharacterAttackType.Heavy:

                    float Drained = 0;

                    while (Count>0)
                    {
                        if (Status.CurrentEnergyOrb < Data.MaxEnergyOrb)
                        {
                            if (Attack.Drain)
                            {
                                int Value= Mathf.RoundToInt(Attack.Damage / 5.0f);
                                if (Value > Data.MaxHP - Status.CurrentHP)
                                {
                                    Value = Data.MaxHP - Status.CurrentHP;
                                }
                                Status.CurrentHP += Value;
                                Drained += Value;
                            }
                                                            
                            Status.CurrentEnergyOrb++;
                            Status.EnergyOrbs.transform.GetChild(Status.CurrentEnergyOrb - 1).GetComponent<Image>().sprite = Status.EnergyOrbFilledSprite;
                        }
                        Count--;
                    }
                    if (Attack.Drain)
                    {
                        GameObject DamageText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/DamageText"), Entity.transform.position, Quaternion.Euler(0, 0, 0));
                        DamageText.GetComponent<DamageText>().TravelVector = Vector2.up;
                        DamageText.GetComponent<Text>().color = Color.green;
                        DamageText.transform.parent = Status.Canvas.transform;
                        DamageText.GetComponent<Text>().text = Drained.ToString();
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

    protected void CheckDrainInput()
    {
        var Status = Entity.GetComponent<StatusManager_Character>();
        if (Utility.InputDrain()&&Status.CurrentEnergyOrb>0)
        {
            Status.EnergyOrbs.transform.GetChild(Status.CurrentEnergyOrb - 1).GetComponent<Image>().sprite = Status.EnergyOrbEmptySprite;
            Status.CurrentEnergyOrb--;
            Status.Drain = true;
            Status.DrainMark.GetComponent<Image>().enabled = true;
        }
    }

    protected CharacterAttackInfo GetHeavyAttackInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        var Status = Entity.GetComponent<StatusManager_Character>();
        int damage = Data.HeavyAttackBaseDamage;
        if (Status.CurrentEnergy == Data.MaxEnergy)
        {
            damage = Data.HeavyAttackMaxDamage + Data.HeavyAttackMaxDamageBonus;
        }
        else
        {
            damage = Mathf.RoundToInt((Data.HeavyAttackMaxDamage - Data.HeavyAttackBaseDamage) * (float)Status.CurrentEnergy / Data.MaxEnergy) + Data.HeavyAttackBaseDamage;
        }

        if (Status.Drain)
        {
            damage += Data.DrainBonus;
            Status.DrainMark.GetComponent<Image>().enabled = false;
            
        }

        CharacterAttackInfo Attack= new CharacterAttackInfo(Entity, CharacterAttackType.Heavy, Entity.transform.right.x > 0, damage, Data.HeavyAttackOffset, Data.HeavyAttackHitBoxSize, Status.Drain);


        Status.Drain = false;

        return Attack;
    }

    protected CharacterAttackInfo GetLightAttackInfo()
    {
        var Data = Entity.GetComponent<CharacterData>();
        return new CharacterAttackInfo(Entity, CharacterAttackType.Light, Entity.transform.right.x > 0, Data.LightAttackDamage, Data.LightAttackOffset, Data.LightAttackHitBoxSize,false);
    }

    protected void GetLightAttackTime()
    {
        var Data = Entity.GetComponent<CharacterData>();
        AnticipationTime = Data.LightAttackAnticipation;
        AttackTime = Data.LightAttackTime;
        RecoveryTime = Data.LightAttackRecovery;
    }

    protected void GetHeavyAttackTime()
    {
        var Data = Entity.GetComponent<CharacterData>();

        if (Entity.GetComponent<StatusManager_Character>().CurrentEnergy < Data.MaxEnergy)
        {
            AnticipationTime = Data.HeavyAttackLongAnticipation;
            AttackTime = Data.HeavyAttackTime;
            RecoveryTime = Data.HeavyAttackLongRecovery;
        }
        else
        {
            AnticipationTime = Data.HeavyAttackShortAnticipation;
            AttackTime = Data.HeavyAttackTime;
            RecoveryTime = Data.HeavyAttackShortRecovery;
        }
    }

    protected void GenerateSlashImage(CharacterAttackType Type)
    {
        float EulerAngle = 0;

        if (!Attack.Right)
        {
            EulerAngle = 180;
        }

        if (Type == CharacterAttackType.Light)
        {
            Vector2 Offset = Entity.GetComponent<CharacterData>().LightAttackOffset;
            if (!Attack.Right)
            {
                Offset.x = -Offset.x;
            }

            SlashImage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/LightSlash"), (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        }
        else
        {
            Vector2 Offset = Entity.GetComponent<CharacterData>().HeavyAttackOffset;
            if (!Attack.Right)
            {
                Offset.x = -Offset.x;
            }

            SlashImage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/HeavySlash"), (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        }
    }

    protected void CheckAttackTimeCount<EndStayState,EndMoveState>(float AnticipationTime,float AttackTime,float RecoveryTime) where EndStayState : CharacterActionState where EndMoveState : CharacterActionState
    {
        AttackTimeCount += Time.deltaTime;
        switch (CurrentAttackState)
        {
            case AttackState.Anticipation:
                if (AttackTimeCount > AnticipationTime)
                {
                    CurrentAttackState = AttackState.Attack;
                    AttackTimeCount = 0;

                    GenerateSlashImage(Attack.Type);

                    if (Attack.Type == CharacterAttackType.Light)
                    {

                        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
                        CurrentSpriteSeries = SpriteData.LightRecoverySeries;

                        Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[0];
                        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightRecoveryOffset, SpriteData.LightRecoverySize);
                    }
                    else
                    {
                        
                        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
                        CurrentSpriteSeries = SpriteData.HeavyRecoverySeries;

                        Entity.GetComponent<SpriteRenderer>().sprite = CurrentSpriteSeries[0];
                        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyRecoveryOffset, SpriteData.HeavyRecoverySize);
                    }

                    SlashImage.transform.parent = Entity.transform;

                    
                }
                break;
            case AttackState.Attack:
                if (AttackTimeCount > AttackTime)
                {
                    CurrentAttackState = AttackState.Recovery;
                    AttackTimeCount = 0;
                    GameObject.Destroy(SlashImage);
                }
                break;
            case AttackState.Recovery:
                if (AttackTimeCount > RecoveryTime)
                {
                    if (CheckCharacterMove<EndStayState>(true, false))
                    {
                        TransitionTo<EndMoveState>();
                    }
                    else
                    {
                        TransitionTo<EndStayState>();
                    }
                }
                break;
        }

    }

    protected bool CheckGrounded<GroundState>() where GroundState: CharacterActionState 
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (SpeedManager.HitGround && SpeedManager.SelfSpeed.y+SpeedManager.ForcedSpeed.y<=0)
        {
            TransitionTo<GroundState>();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool CheckAir<AirState>() where AirState : CharacterActionState
    {
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (SpeedManager.HitGround && SpeedManager.SelfSpeed.y + SpeedManager.ForcedSpeed.y <= 0)
        {
            return false;
        }
        else
        {
            TransitionTo<AirState>();
            return true;
        }
    }

    protected bool CheckCharacterLightAttack<AttackState>() where AttackState : CharacterActionState
    {
        if (Utility.InputLightAttack())
        {
            TransitionTo<AttackState>();
            return true;
        }

        return false;
    }

    protected bool CheckCharacterHeavyAttack<AttackState>() where AttackState : CharacterActionState
    {
        if (Utility.InputHeavyAttack())
        {
            TransitionTo<AttackState>();
            return true;
        }

        return false;
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
        WallHitted = false;

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
        if (CheckGetHit())
        {
            return;
        }

        if (CheckCharacterJump<JumpHoldingStay>())
        {
            return;
        }

        if (CheckCharacterLightAttack<GroundLightAttack>())
        {
            return;
        }

        if (CheckCharacterHeavyAttack<GroundHeavyAttack>())
        {
            return;
        }

        if (CheckAir<AirStay>())
        {
            return;
        }

        if (CheckCharacterMove<Stand>(true,false))
        {
            TransitionTo<GroundMove>();
        }
    }
}

public class GroundMove : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;
        WallHitted = false;

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
        if (CheckGetHit())
        {
            return;
        }

        if (CheckCharacterJump<JumpHoldingMove>())
        {
            return;
        }

        if (CheckCharacterLightAttack<GroundLightAttack>())
        {
            return;
        }

        if (CheckCharacterHeavyAttack<GroundHeavyAttack>())
        {
            return;
        }

        if (CheckAir<AirMove>())
        {
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
        if (CheckGetHit())
        {
            return;
        }

        if (CheckHolding())
        {
            return;
        }

        if (CheckGrounded<Stand>())
        {
            return;
        }

        if (CheckCharacterLightAttack<AirLightAttack>())
        {
            return;
        }

        if (CheckCharacterHeavyAttack<AirHeavyAttack>())
        {
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
        if (CheckGetHit())
        {
            return;
        }

        if (CheckWallHitting())
        {
            return;
        }

        if (CheckHolding())
        {
            return;
        }
        if (CheckGrounded<GroundMove>())
        {
            return;
        }


        if (CheckCharacterLightAttack<AirLightAttack>())
        {
            return;
        }

        if (CheckCharacterHeavyAttack<AirHeavyAttack>())
        {
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
        if (CheckGetHit())
        {
            return;
        }

        if (CheckGrounded<Stand>())
        {
            return;
        }


        if (CheckCharacterLightAttack<AirLightAttack>())
        {
            return;
        }

        if (CheckCharacterHeavyAttack<AirHeavyAttack>())
        {
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
        if (CheckGetHit())
        {
            return;
        }

        if (CheckWallHitting())
        {
            return;
        }

        if (CheckGrounded<GroundMove>())
        {
            return;
        }


        if (CheckCharacterLightAttack<AirLightAttack>())
        {
            return;
        }

        if (CheckCharacterHeavyAttack<AirHeavyAttack>())
        {
            return;
        }

        CheckCharacterMove<AirStay>(false,true);
    }
}

public class WallHitting : CharacterActionState
{
    private float HittingTimeCount;

    public override void OnEnter()
    {
        base.OnEnter();
        HittingTimeCount = 0;
        CurrentGravity = 0;
        Entity.GetComponent<SpeedManager>().SelfSpeed.y = 0;
        WallHitted = true;
    }

    public override void Update()
    {
        base.Update();
        Attached();
    }

    private void Attached()
    {
        HittingTimeCount += Time.deltaTime;
        if (HittingTimeCount >= Entity.GetComponent<CharacterData>().WallHittingPause)
        {
            TransitionTo<AirStay>();
        }
    }
}

public class GroundLightAttack : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        AttackTimeCount = 0;
        AttackHit = false;
        CurrentAttackState = AttackState.Anticipation;

        Attack = GetLightAttackInfo();
        GetLightAttackTime();

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentAttackState!=AttackState.Attack && CheckGetHit())
        {
            return;
        }
        CheckCharacterMove<Stand>(true, false);
        CheckAttackTimeCount<Stand, GroundMove>(AnticipationTime, AttackTime, RecoveryTime);
        CheckHit();
    }

    private void CheckHit()
    {
        if (AttackHit)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else if (CurrentAttackState == AttackState.Attack)
        {
            if (AttackHitEnemy(Attack))
            {
                AttackHit = true;
            }
        }
    }
}

public class AirLightAttack : CharacterActionState
{
    public override void OnEnter()
    {
        base.OnEnter();
        AttackTimeCount = 0;
        AttackHit = false;
        CurrentAttackState = AttackState.Anticipation;
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;

        Attack = GetLightAttackInfo();
        GetLightAttackTime();

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.LightAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.LightAnticipationOffset, SpriteData.LightAnticipationSize);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentAttackState != AttackState.Attack && CheckGetHit())
        {
            return;
        }
        CheckCharacterMove<AirStay>(true, false);
        CheckAttackTimeCount<AirStay, AirMove>(AnticipationTime, AttackTime, RecoveryTime);
        CheckHit();
    }

    private void CheckHit()
    {
        if (AttackHit)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else if (CurrentAttackState == AttackState.Attack)
        {
            if (AttackHitEnemy(Attack))
            {
                AttackHit = true;
            }
        }
    }
}


public class GroundHeavyAttack : CharacterActionState
{
    private int EnergyStored;

    public override void OnEnter()
    {
        base.OnEnter();
        AttackTimeCount = 0;
        AttackHit = false;
        CurrentAttackState = AttackState.Anticipation;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        EnergyStored = Entity.GetComponent<StatusManager_Character>().CurrentEnergy;

        Attack = GetHeavyAttackInfo();
        GetHeavyAttackTime();

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HeavyAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyAnticipationOffset, SpriteData.HeavyAnticipationSize);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentAttackState != AttackState.Attack && CheckGetHit())
        {
            return;
        }
        CheckAttackTimeCount<Stand, GroundMove>(AnticipationTime, AttackTime, RecoveryTime);
        CheckHit();
        CostEnergy();
    }

    private void CostEnergy()
    {
        if (CurrentAttackState == AttackState.Anticipation)
        {
            Entity.GetComponent<StatusManager_Character>().CurrentEnergy = Mathf.RoundToInt(EnergyStored * (1 - AttackTimeCount / AnticipationTime));
        }
        else
        {
            Entity.GetComponent<StatusManager_Character>().CurrentEnergy = 0;
        }
    }

    private void CheckHit()
    {
        if(!AttackHit && CurrentAttackState == AttackState.Attack)
        {
            if (AttackHitEnemy(Attack))
            {
                AttackHit = true;
            }
        }
    }
}

public class AirHeavyAttack : CharacterActionState
{
    private int EnergyStored;

    public override void OnEnter()
    {
        base.OnEnter();
        AttackTimeCount = 0;
        AttackHit = false;
        CurrentAttackState= AttackState.Anticipation;
        Entity.GetComponent<SpeedManager>().SelfSpeed = Vector2.zero;
        CurrentGravity = 0;
        EnergyStored = Entity.GetComponent<StatusManager_Character>().CurrentEnergy;

        Attack = GetHeavyAttackInfo();
        GetHeavyAttackTime();

        var SpriteData = Entity.GetComponent<CharacterSpriteData>();
        CurrentSpriteSeries = SpriteData.HeavyAnticipationSeries;

        SetCharacterSprite();
        Entity.GetComponent<SpeedManager>().SetBodyInfo(SpriteData.HeavyAnticipationOffset, SpriteData.HeavyAnticipationSize);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentAttackState != AttackState.Attack && CheckGetHit())
        {
            return;
        }
        CheckAttackTimeCount<AirStay, AirMove>(AnticipationTime, AttackTime, RecoveryTime);
        CheckHit();
        CostEnergy();
    }

    private void CheckHit()
    {
        if (!AttackHit && CurrentAttackState == AttackState.Attack)
        {
            if (AttackHitEnemy(Attack))
            {
                AttackHit = true;
            }
        }
    }

    private void CostEnergy()
    {
        if (CurrentAttackState == AttackState.Anticipation)
        {
            Entity.GetComponent<StatusManager_Character>().CurrentEnergy = Mathf.RoundToInt(EnergyStored * (1 - AttackTimeCount / AnticipationTime));
            
        }
        else
        {
            Entity.GetComponent<StatusManager_Character>().CurrentEnergy = 0;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        CurrentGravity = Entity.GetComponent<CharacterData>().NormalGravity;
    }
}

public class GetHit : CharacterActionState
{

    private float GetHitTimeCount;
    private bool FinishOnGround;

    public override void OnEnter()
    {
        base.OnEnter();
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
            if (CheckGrounded<Stand>())
            {
                FinishOnGround = true;
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
        Entity.GetComponent<IHittable>().hit = false;
    }
}

