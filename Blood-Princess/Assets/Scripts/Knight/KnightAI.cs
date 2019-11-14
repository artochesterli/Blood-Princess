using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KnightAttackMode
{
    Single,
    DoubleFirst,
    DoubleSecond,
    Chase
}

public enum KnightState
{
    Patron,
    Engage,
    Anticipation,
    Strike,
    Recovery,
    Interrupted
}

public class KnightAI : MonoBehaviour
{
    public GameObject Player;
    public LayerMask PlayerLayer;

    public GameObject DoubleAttackMark;

    public GameObject PatronLeftMark;
    public GameObject PatronRightMark;
    public GameObject DetectLeftMark;
    public GameObject DetectRightMark;

    public float DetectLeftX;
    public float DetectRightX;
    public float PatronLeftX;
    public float PatronRightX;

    public KnightAttackMode CurrentAttackMode;
    public KnightState LastState;

    public float AttackCoolDownTimeCount;

    public bool GetAttackedInThisRound;


    private FSM<KnightAI> KnightAIFSM;
    // Start is called before the first frame update
    void Start()
    {
        GetPatronInfo();
        Player = CharacterOpenInfo.Self;

        var Data = GetComponent<KnightData>();

        KnightAIFSM = new FSM<KnightAI>(this);
        KnightAIFSM.TransitionTo<KnightPatron>();
    }

    // Update is called once per frame
    void Update()
    {
        KnightAIFSM.Update();
    }

    private void OnDestroy()
    {
        KnightAIFSM.CurrentState.CleanUp();
    }

    private void GetPatronInfo()
    {
        DetectLeftX = DetectLeftMark.transform.position.x;
        DetectRightX = DetectRightMark.transform.position.x;
        PatronLeftX = PatronLeftMark.transform.position.x;
        PatronRightX = PatronRightMark.transform.position.x;
    }
}

public abstract class KnightBehavior : FSM<KnightAI>.State
{
    protected GameObject Entity;
    protected GameObject SlashImage;

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

    protected void SetKnight(Sprite S,Vector2 Offset,Vector2 Size)
    {
        Entity.GetComponent<SpriteRenderer>().sprite = S;
        Entity.GetComponent<SpeedManager>().SetBodyInfo(Offset, Size);
    }

    protected void RectifyDirection()
    {
        if (GetXDiff() > 0)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    protected bool PlayerInDetectRange(bool CheckDirection)
    {
        var PatronData = Entity.GetComponent<PatronData>();

        Vector2 PlayerTruePos = Context.Player.GetComponent<SpeedManager>().GetTruePos();
        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        if (CheckDirection)
        {
            if (GetXDiff() > 0 && Entity.transform.right.x < 0 || GetXDiff() < 0 && Entity.transform.right.x > 0)
            {
                return false;
            }
        }

        if(PlayerTruePos.x >= Context.DetectLeftX && PlayerTruePos.x <= Context.DetectRightX && Mathf.Abs(PlayerTruePos.y - TruePos.y)<=PatronData.DetectHeight)
        {
            RaycastHit2D hit = Physics2D.Raycast(TruePos, PlayerTruePos - TruePos, (PlayerTruePos - TruePos).magnitude, PatronData.DetectLayer);
            if (hit && hit.collider.gameObject == Context.Player)
            {
                return true;
            }
        }

        return false;

    }

    protected float GetBorderDis()
    {
        var Data = Entity.GetComponent<KnightData>();

        float XDiff = GetXDiff();

        var PlayerSpeedManager = Context.Player.GetComponent<SpeedManager>();

        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();

        if (XDiff > 0)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            return (PlayerSpeedManager.GetTruePos().x - PlayerSpeedManager.BodyWidth / 2) - (SelfSpeedManager.GetTruePos().x + SelfSpeedManager.BodyWidth / 2);
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            return (SelfSpeedManager.GetTruePos().x - SelfSpeedManager.BodyWidth / 2) - (PlayerSpeedManager.GetTruePos().x + PlayerSpeedManager.BodyWidth / 2);
        }
    }

    protected float GetXDiff()
    {
        return Context.Player.GetComponent<SpeedManager>().GetTruePos().x - Entity.GetComponent<SpeedManager>().GetTruePos().x;
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

    protected void MakeAttackDecision()
    {
        var Data = Entity.GetComponent<KnightData>();
        float AttackChoiceNumber = Random.Range(0.0f, 1.0f);


        if(AttackChoiceNumber < Data.SingleAttackChance)
        {
            Context.CurrentAttackMode = KnightAttackMode.Single;
        }
        else
        {
            Context.CurrentAttackMode = KnightAttackMode.DoubleFirst;
        }

        TransitionTo<KnightAttackAnticipation>();
    }

    protected EnemyAttackInfo GetAttackInfo()
    {
        var KnightData = Entity.GetComponent<KnightData>();

        return new EnemyAttackInfo(Entity, Entity.transform.right.x > 0, KnightData.Damage, KnightData.Damage, KnightData.AttackOffset, KnightData.AttackHitBoxSize);
    }

    protected void TransitionToInterrupted()
    {
        var Data = Entity.GetComponent<KnightData>();

        if (Context.GetAttackedInThisRound)
        {
            Context.AttackCoolDownTimeCount = 0;
        }
        else
        {
            Context.AttackCoolDownTimeCount = Data.FirstGetHitAttackCoolDown;
            Context.GetAttackedInThisRound = true;
        }

        TransitionTo<KnightGetInterrupted>();
    }

}

public class KnightPatron : KnightBehavior
{
    private float TimeCount;
    private bool Moving;
    private bool MovingRight;


    private bool First;

    public override void Init()
    {
        base.Init();
        First = true;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (!First)
        {
            SetUp();
        }
        else
        {
            SetUpInit();
        }
        SetAppearance();


        First = false;
    }

    public override void Update()
    {
        base.Update();

        if (CheckGetInterrupted())
        {
            TransitionToInterrupted();
            return;
        }
        if (PlayerInDetectRange(true))
        {
            TransitionTo<KnightEngage>();
            return;
        }
        Patron();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = KnightState.Patron;
    }

    private void SetUp()
    {
        var KnightData = Entity.GetComponent<KnightData>();

        Moving = true;

        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        if(TruePos.x > Context.PatronRightX)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            MovingRight = false;
        }
        else if(TruePos.x < Context.PatronLeftX)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            MovingRight = true;
        }
        else
        {
            if (Entity.transform.right.x > 0)
            {
                MovingRight = true;
            }
            else
            {
                MovingRight = false;
            }
        }

        if (MovingRight)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = KnightData.NormalMoveSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -KnightData.NormalMoveSpeed;
        }

    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void CheckSelfPos()
    {
        var Data = Entity.GetComponent<PatronData>();
        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        TimeCount = 0;

        if(MovingRight && TruePos.x >= Context.PatronRightX)
        {
            MovingRight = false;
            Moving = false;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else if(!MovingRight && TruePos.x <= Context.PatronLeftX)
        {
            MovingRight = true;
            Moving = false;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void CheckStayTime()
    {
        var Data = Entity.GetComponent<KnightData>();
        TimeCount += Time.deltaTime;
        if (TimeCount >= Entity.GetComponent<PatronData>().PatronStayTime)
        {
            TimeCount = 0;
            Moving = true;
            if (MovingRight)
            {
                Entity.transform.rotation = Quaternion.Euler(0, 0, 0);
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.NormalMoveSpeed;
            }
            else
            {
                Entity.transform.rotation = Quaternion.Euler(0, 180, 0);
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.NormalMoveSpeed;
            }
        }

    }

    private void Patron()
    {
        if (Moving)
        {
            CheckSelfPos();
        }
        else
        {
            CheckStayTime();
        }
    }

    private void SetUpInit()
    {
        var Data = Entity.GetComponent<KnightData>();

        float StayTime = Entity.GetComponent<PatronData>().PatronStayTime;

        float SelfX = Entity.GetComponent<SpeedManager>().GetTruePos().x;

        float CycleTime = 2 * (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed + 2 * StayTime;

        float time = Random.Range(0, CycleTime);

        float PositionX = SelfX;

        if(time< (Context.PatronRightX-SelfX)/ Data.NormalMoveSpeed)
        {
            Moving = true;
            MovingRight = true;
            PositionX = Mathf.Lerp(Entity.transform.position.x, Context.PatronRightX, time / (Context.PatronRightX - SelfX) / Data.NormalMoveSpeed);
        }
        else if(time < (Context.PatronRightX - SelfX) / Data.NormalMoveSpeed + StayTime)
        {
            Moving = false;
            MovingRight = true;
            TimeCount = time - (Context.PatronRightX - SelfX) / Data.NormalMoveSpeed;
            PositionX = Context.PatronRightX;

        }
        else if(time < (Context.PatronRightX - SelfX) / Data.NormalMoveSpeed + StayTime + (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed)
        {
            Moving = true;
            MovingRight = false;
            float CutTime = time - ((Context.PatronRightX - SelfX) / Data.NormalMoveSpeed + StayTime);
            PositionX = Mathf.Lerp(Context.PatronRightX, Context.PatronLeftX, CutTime / (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed);
        }
        else if(time < (Context.PatronRightX - SelfX) / Data.NormalMoveSpeed + 2*StayTime + (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed)
        {
            Moving = false;
            MovingRight = false;
            TimeCount = time - (Context.PatronRightX - SelfX) / Data.NormalMoveSpeed - StayTime - (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed;
            PositionX = Context.PatronLeftX;
        }
        else
        {
            Moving = true;
            MovingRight = true;
            float CutTime = time - ((Context.PatronRightX - SelfX) / Data.NormalMoveSpeed + 2*StayTime+ (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed);
            PositionX = Mathf.Lerp(Context.PatronLeftX, Context.PatronRightX, CutTime / (Context.PatronRightX - Context.PatronLeftX) / Data.NormalMoveSpeed);
        }

        Entity.transform.position = new Vector2(PositionX - Entity.GetComponent<SpeedManager>().OriPos.x, Entity.transform.position.y);

        if (MovingRight)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Moving)
        {
            if (MovingRight)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.NormalMoveSpeed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.NormalMoveSpeed;
            }
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }
}

public class KnightEngage : KnightBehavior
{
    private float ShortAttackDis;
    private float ChaseAttackMinDis;
    private float ChaseAttackMaxDis;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        EventManager.instance.AddHandler<PlayerStartAttackAnticipation>(OnCharacterGoingToAttack);
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionToInterrupted();
            return;
        }
        if (!PlayerInDetectRange(false))
        {
            TransitionTo<KnightPatron>();
            return;
        }

        KeepDis();
        CheckAttackCoolDown();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = KnightState.Engage;
        EventManager.instance.RemoveHandler<PlayerStartAttackAnticipation>(OnCharacterGoingToAttack);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        EventManager.instance.RemoveHandler<PlayerStartAttackAnticipation>(OnCharacterGoingToAttack);
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();
        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();

        float AttackHitDis = Data.AttackOffset.x - Data.AttackHitBoxSize.x / 2 + Data.AttackHitBoxSize.x * Data.AttackAvailableHitBoxPercentage;

        if (Entity.transform.right.x > 0)
        {
            AttackHitDis -= (SelfSpeedManager.GetTruePos().x + SelfSpeedManager.BodyWidth / 2) - Entity.transform.position.x;
        }
        else
        {
            AttackHitDis -= Entity.transform.position.x - (SelfSpeedManager.GetTruePos().x - SelfSpeedManager.BodyWidth / 2);
        }

        ShortAttackDis = AttackHitDis + Data.AttackStepForwardSpeed * Data.AttackTime;
        ChaseAttackMinDis = AttackHitDis + Data.AttackStepForwardSpeed * Data.AttackTime + Data.MinChaseAttackChaseDistance;
        ChaseAttackMaxDis = AttackHitDis + Data.AttackStepForwardSpeed * Data.AttackTime + Data.MaxChaseAttackChaseDistance;

    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void KeepDis()
    {
        var Data = Entity.GetComponent<KnightData>();


        var PlayerSpeedManager = Context.Player.GetComponent<SpeedManager>();

        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();

        float BorderDis = GetBorderDis();

        if(BorderDis <= ShortAttackDis)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            if(Context.AttackCoolDownTimeCount <= 0)
            {
                Context.AttackCoolDownTimeCount = Data.AttackCoolDown;
                MakeAttackDecision();
            }
        }
        else if(BorderDis <= ChaseAttackMinDis)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.transform.right.x * Data.NormalMoveSpeed;
        }
        else if(BorderDis <= ChaseAttackMaxDis)
        {
            Context.AttackCoolDownTimeCount = Data.AttackCoolDown;
            Context.CurrentAttackMode = KnightAttackMode.Chase;
            TransitionTo<KnightAttackAnticipation>();
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.transform.right.x * Data.NormalMoveSpeed;
        }
    }

    private void CheckAttackCoolDown()
    {
        var Data = Entity.GetComponent<KnightData>();

        Context.AttackCoolDownTimeCount -= Time.deltaTime;
    }

    private void OnCharacterGoingToAttack(PlayerStartAttackAnticipation e)
    {
        var Data = Entity.GetComponent<KnightData>();

    }
}

public class KnightAttackAnticipation : KnightBehavior
{
    private float TimeCount;
    private float StateTime;

    private float DistanceTraveled;
    private float AttackReachRange;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        
    }

    public override void Update()
    {
        base.Update();

        if (Context.CurrentAttackMode == KnightAttackMode.Chase)
        {
            ChasePlayer();
        }
        else
        {
            CheckTime();
        }

    }

    public override void OnExit()
    {
        base.OnExit();
        var Status = Entity.GetComponent<StatusManager_Knight>();

        Status.Interrupted = false;

        Context.LastState = KnightState.Anticipation;
        Context.DoubleAttackMark.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= StateTime)
        {
            TransitionTo<KnightAttackStrike>();
            return;
        }
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Anticipation, KnightSpriteData.AnticipationOffset, KnightSpriteData.AnticipationSize);
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();

        TimeCount = 0;
        switch (Context.CurrentAttackMode)
        {
            case KnightAttackMode.Single:
                StateTime = Data.SingleAttackAnticipationTime;
                break;
            case KnightAttackMode.DoubleFirst:
                StateTime = Data.DoubleAttackFirstAnticipationTime;
                Context.DoubleAttackMark.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case KnightAttackMode.DoubleSecond:
                StateTime = Data.DoubleAttackSecondAnticipationTime;
                break;
            case KnightAttackMode.Chase:
                StateTime = Data.ChaseAttackAnticipationTime;
                break;
        }

        Context.GetAttackedInThisRound = false;
        Context.AttackCoolDownTimeCount = Data.AttackCoolDown;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        DistanceTraveled = 0;

        var SpeedManager = Entity.GetComponent<SpeedManager>();

        float AttackHitDis = Data.AttackOffset.x - Data.AttackHitBoxSize.x / 2 + Data.AttackHitBoxSize.x * Data.AttackAvailableHitBoxPercentage;

        if (Entity.transform.right.x > 0)
        {
            AttackHitDis -= (SpeedManager.GetTruePos().x + SpeedManager.BodyWidth / 2) - Entity.transform.position.x;
        }
        else
        {
            AttackHitDis -= Entity.transform.position.x - (SpeedManager.GetTruePos().x - SpeedManager.BodyWidth / 2);
        }

        AttackReachRange = AttackHitDis + Data.AttackStepForwardSpeed * Data.AttackTime;



    }

    private void ChasePlayer()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= StateTime)
        {
            var Data = Entity.GetComponent<KnightData>();

            float Speed = Data.ChaseAttackSpeed;
            bool TraveledEnough = false;

            if (DistanceTraveled+ Data.ChaseAttackSpeed * Time.deltaTime >= Data.MaxChaseAttackChaseDistance)
            {
                TraveledEnough = true;
                Speed = (Data.MaxChaseAttackChaseDistance - DistanceTraveled) / Time.deltaTime;
            }

            DistanceTraveled += Data.ChaseAttackSpeed * Time.deltaTime;

            if (Entity.transform.right.x > 0)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Speed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Speed;
            }

            float XDiff = GetXDiff();

            if (!PlayerInDetectRange(false))
            {
                TransitionTo<KnightPatron>();
            }
            else if (TraveledEnough || XDiff > 0 && Entity.transform.right.x < 0 || XDiff < 0 && Entity.transform.right.x > 0 || GetBorderDis() <= AttackReachRange)
            {
                TransitionTo<KnightAttackStrike>();
            }
        }

    }
}

public class KnightAttackStrike : KnightBehavior
{
    private float TimeCount;
    private float StateTime;
    private EnemyAttackInfo Attack;
    private bool AttackHit;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        GenerateSlashImage();
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionToInterrupted();
            return;
        }
        CheckHitPlayer();
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();

        Context.LastState = KnightState.Strike;

        GameObject.Destroy(SlashImage);
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();

        TimeCount = 0;
        StateTime = Data.AttackTime;
        Attack = GetAttackInfo();
        AttackHit = false;
        if(Entity.transform.right.x > 0)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.AttackStepForwardSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.AttackStepForwardSpeed;
        }


    }

    private void GenerateSlashImage()
    {
        float EulerAngle = 0;

        if (!Attack.Right)
        {
            EulerAngle = 180;
        }

        var Data = Entity.GetComponent<KnightData>();

        Vector2 Offset = Data.AttackOffset;
        if (!Attack.Right)
        {
            Offset.x = -Offset.x;
        }

        SlashImage = GameObject.Instantiate(Data.SlashImage, (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        SlashImage.transform.parent = Entity.transform;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount > StateTime)
        {
            TransitionTo<KnightAttackRecovery>();
            return;
        }
    }

    private void CheckHitPlayer()
    {
        if (!AttackHit && HitPlayer(Attack))
        {
            AttackHit = true;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private bool HitPlayer(EnemyAttackInfo Attack)
    {
        Vector2 Offset = Attack.HitBoxOffset;
        Offset.x = Entity.transform.right.x * Offset.x;

        RaycastHit2D Hit = Physics2D.BoxCast(Entity.transform.position + (Vector3)Offset, Attack.HitBoxSize, 0, Entity.transform.right, 0, Context.PlayerLayer);

        if (Hit)
        {
            Hit.collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class KnightAttackRecovery : KnightBehavior
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
            TransitionToInterrupted();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = KnightState.Recovery;

    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();

        TimeCount = 0;

        switch (Context.CurrentAttackMode)
        {
            case KnightAttackMode.Single:
                StateTime = Data.SingleAttackRecoveryTime;
                break;
            case KnightAttackMode.DoubleFirst:
                StateTime = Data.DoubleAttackFirstRecoveryTime;
                break;
            case KnightAttackMode.DoubleSecond:
                StateTime = Data.DoubleAttackSecondRecoveryTime;
                break;
        }

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Recovery, KnightSpriteData.RecoveryOffset, KnightSpriteData.RecoverySize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            RectifyDirection();
            if (Context.CurrentAttackMode == KnightAttackMode.DoubleFirst)
            {
                if (GetXDiff()>0)
                {
                    Entity.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    Entity.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                Context.CurrentAttackMode = KnightAttackMode.DoubleSecond;
                TransitionTo<KnightAttackAnticipation>();
            }
            else
            {
                TransitionTo<KnightEngage>();
            }
        }
    }
}

public class KnightGetInterrupted : KnightBehavior
{
    private float TimeCount;

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
            TransitionToInterrupted();
            return;
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = KnightState.Interrupted;
    }

    private void SetUp()
    {
        CharacterAttackInfo Temp = (CharacterAttackInfo)Entity.GetComponent<IHittable>().HitAttack;

        var KnightData = Entity.GetComponent<KnightData>();
        var Status = Entity.GetComponent<StatusManager_Knight>();

        Status.Interrupted = false;

        if (Temp.Right)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = KnightData.KnockedBackSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -KnightData.KnockedBackSpeed;
        }

        TimeCount = 0;
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();

        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);

        /*if (Context.LastState != KnightState.Recovery && Context.LastState != KnightState.Strike)
        {
            SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
        }
        else
        {
            SetKnight(KnightSpriteData.Hit, KnightSpriteData.HitOffset, KnightSpriteData.HitSize);
        }*/
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;

        var Data = Entity.GetComponent<KnightData>();

        if (TimeCount >= Data.KnockedBackTime)
        {
            TransitionTo<KnightEngage>();
            return;
        }

        /*if (Context.LastState == KnightState.Recovery || Context.LastState == KnightState.Strike)
        {
            if(TimeCount >= Data.KnockedBackTime + Data.RecoveryKnockedBackStunTime)
            {
                TransitionTo<KnightEngage>();
                return;
            }
            else if(TimeCount >= Data.KnockedBackTime)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            }
        }
        else
        {
            if (TimeCount >= Data.KnockedBackTime)
            {
                TransitionTo<KnightEngage>();
                return;
            }
        }*/


    }
}


