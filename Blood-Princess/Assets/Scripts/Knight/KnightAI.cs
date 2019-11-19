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
    BlinkPrepare,
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
    public KnightState CurrentState;

    public float AttackCoolDownTimeCount;


    private FSM<KnightAI> KnightAIFSM;
    // Start is called before the first frame update
    void Start()
    {
        GetPatronInfo();
        Player = CharacterOpenInfo.Self;

        AttackCoolDownTimeCount = GetComponent<KnightData>().AttackCoolDown;


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

        if (AttackChoiceNumber < Data.SingleAttackChance)
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

        TransitionTo<KnightGetInterrupted>();
    }

}

public class KnightPatron : KnightBehavior
{
    private float TimeCount;
    private bool Moving;
    private bool MovingRight;

    private bool Initilized;

    public override void Init()
    {
        base.Init();
        Initilized = false;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = KnightState.Patron;

        var Data = Entity.GetComponent<KnightData>();
        var PatronData = Entity.GetComponent<PatronData>();

        if (Initilized)
        {
            AIUtility.PatronSetUp(Entity, ref Moving, ref MovingRight, Context.PatronRightX, Context.PatronLeftX, Data.NormalMoveSpeed);
        }
        else
        {
            Initilized = true;
            AIUtility.RandomPatronInit(Entity, PatronData.PatronStayTime, Context.PatronRightX, Context.PatronLeftX, Data.NormalMoveSpeed, ref Moving, ref MovingRight, ref TimeCount);
        }
        SetAppearance();

    }

    public override void Update()
    {
        base.Update();

        var PatronData = Entity.GetComponent<PatronData>();

        if (CheckGetInterrupted())
        {
            TransitionToInterrupted();
            return;
        }
        if (AIUtility.PlayerInDetectRange(Entity,Context.Player,Context.DetectRightX,Context.DetectLeftX,PatronData.DetectHeight,PatronData.DetectLayer,true))
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

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void Patron()
    {
        var Data = Entity.GetComponent<KnightData>();
        var PatronData = Entity.GetComponent<PatronData>();
        if (Moving)
        {
            AIUtility.PatronCheckSelfPos(Entity, Context.PatronRightX, Context.PatronLeftX, ref Moving, ref MovingRight);
        }
        else
        {
            AIUtility.CheckPatronStayTime(Entity, ref TimeCount, PatronData.PatronStayTime, ref Moving, MovingRight, Data.NormalMoveSpeed);
        }
    }
}

public class KnightBlinkPrepare : KnightBehavior
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
        Context.LastState = KnightState.BlinkPrepare;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();

        TimeCount = 0;
        StateTime = Data.BlinkPrepareTime;
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= StateTime)
        {
            Blink();
            TransitionTo<KnightEngage>();
        }
    }

    private void Blink()
    {
        var Data = Entity.GetComponent<KnightData>();
        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();
        var PlayerSpeedManager = Context.Player.GetComponent<SpeedManager>();

        bool RightAvailable = true;
        bool LeftAvailable = true;

        float AttackHitDis = (Data.AttackHitBoxSize.x + Data.AttackStepForwardSpeed * Data.AttackTime) * Data.AttackAvailableHitBoxPercentage;

        float AttackHitBoxBorderToTruePos = Data.AttackOffset.x - Data.AttackHitBoxSize.x / 2 - (SelfSpeedManager.GetTruePos().x - Entity.transform.position.x) * Entity.transform.right.x;

        float BlinkRightPosX = PlayerSpeedManager.GetTruePos().x + PlayerSpeedManager.BodyWidth / 2 + AttackHitDis + AttackHitBoxBorderToTruePos + Data.BlinkForChaseAttackDis;

        if(BlinkRightPosX > Context.DetectRightX)
        {
            RightAvailable = false;
        }

        float BlinkLeftPosX = PlayerSpeedManager.GetTruePos().x - PlayerSpeedManager.BodyWidth / 2 - AttackHitDis - AttackHitBoxBorderToTruePos - Data.BlinkForChaseAttackDis;

        if(BlinkLeftPosX < Context.DetectLeftX)
        {
            LeftAvailable = false;
        }

        if(RightAvailable && LeftAvailable || !RightAvailable && !LeftAvailable)
        {
            float Chance = Random.Range(0.0f, 1.0f);
            if (Chance < 0.5f)
            {
                SelfSpeedManager.MoveToPoint(new Vector2(BlinkRightPosX, SelfSpeedManager.GetTruePos().y));
            }
            else
            {
                SelfSpeedManager.MoveToPoint(new Vector2(BlinkLeftPosX, SelfSpeedManager.GetTruePos().y));
            }
        }
        else if (RightAvailable)
        {
            SelfSpeedManager.MoveToPoint(new Vector2(BlinkRightPosX, SelfSpeedManager.GetTruePos().y));
        }
        else
        {
            SelfSpeedManager.MoveToPoint(new Vector2(BlinkLeftPosX, SelfSpeedManager.GetTruePos().y));
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
        Context.CurrentState = KnightState.Engage;

        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        var PatronData = Entity.GetComponent<PatronData>();

        if (CheckGetInterrupted())
        {
            TransitionToInterrupted();
            return;
        }
        if (!AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX, Context.DetectLeftX, PatronData.DetectHeight, PatronData.DetectLayer, false))
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
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();
        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();
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

        AIUtility.RectifyDirection(Context.Player, Entity);

        float AttackHitDis = (Data.AttackHitBoxSize.x + Data.AttackStepForwardSpeed * Data.AttackTime) * Data.AttackAvailableHitBoxPercentage;

        float AttackHitBoxBorderToTruePos = Data.AttackOffset.x - Data.AttackHitBoxSize.x / 2 - (SelfSpeedManager.GetTruePos().x - Entity.transform.position.x) * Entity.transform.right.x;

        ShortAttackDis = AttackHitDis + AttackHitBoxBorderToTruePos;
        ChaseAttackMinDis = AttackHitDis + AttackHitBoxBorderToTruePos + Data.MinChaseAttackChaseDistance;
        ChaseAttackMaxDis = AttackHitDis + AttackHitBoxBorderToTruePos + Data.MaxChaseAttackChaseDistance;

        float Dis = Mathf.Abs(AIUtility.GetXDiff(Context.Player, Entity)) - PlayerSpeedManager.BodyWidth/2;

        if(Dis <= ShortAttackDis)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            if(Context.AttackCoolDownTimeCount <= 0)
            {
                Context.AttackCoolDownTimeCount = Data.AttackCoolDown;
                MakeAttackDecision();
            }
        }
        else if(Dis <= ChaseAttackMinDis)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.transform.right.x * Data.NormalMoveSpeed;
        }
        else if(Dis <= ChaseAttackMaxDis)
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
        Context.AttackCoolDownTimeCount -= Time.deltaTime;
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
        Context.CurrentState = KnightState.Anticipation;
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
            var PatronData = Entity.GetComponent<PatronData>();

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

            float XDiff = AIUtility.GetXDiff(Context.Player,Entity);

            if (!AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX, Context.DetectLeftX, PatronData.DetectHeight, PatronData.DetectLayer, false))
            {
                TransitionTo<KnightPatron>();
            }
            else if (TraveledEnough || XDiff > 0 && Entity.transform.right.x < 0 || XDiff < 0 && Entity.transform.right.x > 0 || AIUtility.GetBorderDis(Context.Player,Entity) <= AttackReachRange)
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
        Context.CurrentState = KnightState.Strike;
        SetUp();
        SetAppearance();
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

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Recovery, KnightSpriteData.RecoveryOffset, KnightSpriteData.RecoverySize);
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
        if (!AttackHit && AIUtility.HitPlayer(Entity.transform.position,Attack, Context.PlayerLayer))
        {
            AttackHit = true;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
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
        Context.CurrentState = KnightState.Recovery;
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
            if (Context.CurrentAttackMode == KnightAttackMode.DoubleFirst)
            {
                AIUtility.RectifyDirection(Context.Player, Entity);
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
    private bool GetHitOnBack;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = KnightState.Interrupted;
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
        CharacterAttackInfo Temp = Entity.GetComponent<StatusManager_Knight>().CurrentTakenAttack;

        var KnightData = Entity.GetComponent<KnightData>();
        var Status = Entity.GetComponent<StatusManager_Knight>();

        Status.Interrupted = false;

        if (Temp.Right)
        {
            if (Entity.transform.right.x < 0)
            {
                GetHitOnBack = false;
                Context.AttackCoolDownTimeCount = 0;
            }
            else
            {
                GetHitOnBack = true;
            }
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = KnightData.KnockedBackSpeed;
        }
        else
        {
            if (Entity.transform.right.x > 0)
            {
                GetHitOnBack = false;
                Context.AttackCoolDownTimeCount = 0;
            }
            else
            {
                GetHitOnBack = true;
            }
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -KnightData.KnockedBackSpeed;
        }

        TimeCount = 0;
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        if (GetHitOnBack)
        {
            SetKnight(KnightSpriteData.Hit, KnightSpriteData.HitOffset, KnightSpriteData.HitSize);
        }
        else
        {
            SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
        }

    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;

        var Data = Entity.GetComponent<KnightData>();

        if (TimeCount >= Data.KnockedBackTime)
        {
            if (!GetHitOnBack)
            {
                float Chance = Random.Range(0.0f, 1.0f);
                if (Chance < Data.BlinkChance)
                {
                    TransitionTo<KnightBlinkPrepare>();
                }
                else
                {
                    TransitionTo<KnightEngage>();
                }
            }
            else
            {
                TransitionTo<KnightEngage>();
            }
            return;
        }

    }
}


