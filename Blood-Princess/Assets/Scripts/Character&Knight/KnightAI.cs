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

    public int CurrentStamina;
    public int CurrentPatience;

    private FSM<KnightAI> KnightAIFSM;
    // Start is called before the first frame update
    void Start()
    {
        GetPatronInfo();

        var Data = GetComponent<KnightData>();
        CurrentStamina = Data.MaxStamina;
        CurrentPatience = Data.MaxPatience;

        KnightAIFSM = new FSM<KnightAI>(this);
        KnightAIFSM.TransitionTo<KnightPatron>();
    }

    // Update is called once per frame
    void Update()
    {
        KnightAIFSM.Update();
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
        Debug.Log(this.GetType().Name);
    }

    protected void SetKnight(Sprite S,Vector2 Offset,Vector2 Size)
    {
        Entity.GetComponent<SpriteRenderer>().sprite = S;
        Entity.GetComponent<SpeedManager>().SetBodyInfo(Offset, Size);
    }

    protected bool PlayerInRecovery()
    {
        return Context.Player.GetComponent<CharacterAction>().InRecovery;
    }

    protected bool PlayerInDetectRange()
    {
        if (Context.Player)
        {
            return true;
        }

        var Data = Entity.GetComponent<PatronData>();

        Vector2 Init = Quaternion.Euler(0, 0, -Data.DetectAngle / 2) * Entity.transform.right;

        Vector2 Dir = Init;

        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();
        float Dis;
        if (Entity.transform.right.x > 0)
        {
            Dis = Context.DetectRightX - TruePos.x;
        }
        else
        {
            Dis = TruePos.x - Context.DetectLeftX;
        }

        float Angle = 0;

        while (Angle < Data.DetectAngle)
        {
            Dir = Quaternion.Euler(0, 0, Angle) * Init;

            RaycastHit2D hit = Physics2D.Raycast(TruePos, Dir, Dis , Context.PlayerLayer);
            if (hit)
            {
                Context.Player = hit.collider.gameObject;
                return true;
            }
            Angle += Data.DetectInterval;
        }

        Context.Player = null;
        return false;
    }

    protected bool CheckInRange(GameObject obj,float Left, float Right)
    {
        Vector2 TruePos = obj.GetComponent<SpeedManager>().GetTruePos();
        return TruePos.x >= Left && TruePos.x <= Right;
    }

    protected bool PlayerInHitRange(Vector2 Offset, float Height, Vector2 Dir, float AttackStepForwardDis)
    {
        float BoxThickness = 0.01f;

        RaycastHit2D hit = Physics2D.BoxCast(Entity.transform.position + (Offset.y+ Context.Player.transform.position.y - Entity.transform.position.y) *Vector3.up, new Vector2(BoxThickness, Height), 0, Dir,AttackStepForwardDis + Mathf.Abs(Offset.x)-BoxThickness/2, Context.PlayerLayer);

        return hit;
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

        bool PlayerInShortDis = PlayerInHitRange(Data.AttackOffset, Data.AttackHitBoxSize.y, Entity.transform.right, Data.AttackStepForwardSpeed * Data.AttackTime + Data.ChaseForAttackDistance);
        bool PlayerInLongDis = PlayerInHitRange(Data.AttackOffset, Data.AttackHitBoxSize.y, Entity.transform.right, 2 * Data.AttackStepForwardSpeed * Data.AttackTime + Data.ChaseForAttackDistance);

        if (PlayerInShortDis)
        {
            if (AttackChoiceNumber < Data.ShortDisSingleAttackChance)
            {
                Context.CurrentAttackMode = KnightAttackMode.Single;
            }
            else
            {
                Context.CurrentAttackMode = KnightAttackMode.DoubleFirst;
            }
            TransitionTo<KnightChaseForAttack>();

        }
        else if (PlayerInLongDis)
        {
            if (AttackChoiceNumber < Data.LongDisSingleAttackChance)
            {
                Context.CurrentAttackMode = KnightAttackMode.Single;
            }
            else
            {
                Context.CurrentAttackMode = KnightAttackMode.DoubleFirst;
            }
            TransitionTo<KnightChaseForAttack>();
        }
        else
        {
            Context.CurrentAttackMode = KnightAttackMode.Chase;
            TransitionTo<KnightAttackAnticipation>();
        }
    }

    protected void MakeTacticalDecision()
    {
        var Data = Entity.GetComponent<KnightData>();

        if (Context.CurrentStamina > 0)
        {
            MakeAttackDecision();
        }
        else
        {
            TransitionTo<KnightKeepDistance>();
        }
    }

    protected EnemyAttackInfo GetAttack()
    {
        var KnightData = Entity.GetComponent<KnightData>();

        return new EnemyAttackInfo(Entity, Entity.transform.right.x > 0, KnightData.Damage, KnightData.AttackOffset, KnightData.AttackHitBoxSize);
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
            TransitionTo<KnightGetInterrupted>();
            return;
        }
        if (PlayerInDetectRange())
        {
            if (GetXDiff() > 0)
            {
                Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            }

            TransitionTo<KnightInitDecision>();
            return;
        }
        Patron();
    }

    private void SetUp()
    {
        var KnightData = Entity.GetComponent<KnightData>();

        Context.Player = null;
        Context.CurrentStamina = KnightData.MaxStamina;
        Context.CurrentPatience = KnightData.MaxPatience;

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

public class KnightInitDecision : KnightBehavior
{

    public override void Update()
    {
        base.Update();
        if (Mathf.Abs(GetXDiff()) >= Entity.GetComponent<KnightData>().ChaseAttackTriggerDistance)
        {
            Context.CurrentAttackMode = KnightAttackMode.Chase;
            TransitionTo<KnightAttackAnticipation>();
            return;
        }

        MakeTacticalDecision();
    }
}

public class KnightKeepDistance : KnightBehavior
{
    private float TimeCount;

    private float StepTimeCount;
    private bool InStep;
    private float CurrentStepTime;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        if (!CheckInRange(Context.Player, Context.DetectLeftX, Context.DetectRightX))
        {
            TransitionTo<KnightPatron>();
            return;
        }
        if (CheckGetInterrupted())
        {
            TransitionTo<KnightGetInterrupted>();
            return;
        }

        KeepDis();
        if (Decision())
        {
            return;
        }

        if (Mathf.Abs(GetXDiff()) >= Entity.GetComponent<KnightData>().ChaseAttackTriggerDistance && Context.CurrentStamina > 0)
        {
            Context.CurrentAttackMode = KnightAttackMode.Chase;
            TransitionTo<KnightAttackAnticipation>();
            return;
        }

        if (PlayerInRecovery() && Context.CurrentStamina > 0)
        {
            MakeAttackDecision();
        }

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void SetUp()
    {
        TimeCount = 0;
        StepTimeCount = 0;
        InStep = false;
        if (Context.CurrentStamina < Entity.GetComponent<KnightData>().MaxStamina)
        {
            Context.CurrentStamina++;
        }
        Context.CurrentPatience--;
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void KeepDis()
    {
        var Data = Entity.GetComponent<KnightData>();

        float XDiff = GetXDiff();

        if (XDiff > 0)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);

        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Mathf.Abs(XDiff) > Data.TacticDistance)
        {
            InStep = false;
            StepTimeCount = 0;
            if (XDiff > 0)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.GetComponent<KnightData>().NormalMoveSpeed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Entity.GetComponent<KnightData>().NormalMoveSpeed;
            }
        }
        else if (Mathf.Abs(XDiff) > Data.DangerDistance)
        {
            StepBackAndForth(XDiff);
        }
        else
        {
            InStep = false;
            StepTimeCount = 0;

            if (XDiff > 0)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.KeepDisMoveSpeed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.KeepDisMoveSpeed;
            }
        }
    }

    private void StepBackAndForth(float XDiff)
    {
        var Data = Entity.GetComponent<KnightData>();
        var SpeedManager = Entity.GetComponent<SpeedManager>();

        if (!InStep)
        {
            InStep = true;
            StepTimeCount = 0;
            CurrentStepTime = Random.Range(Data.StepBackAndForthMinTime, Data.StepBackAndForthMaxTime);

            float BackDis = Data.TacticDistance - Mathf.Abs(XDiff);
            float ForthDis = Mathf.Abs(XDiff) - Data.DangerDistance;

            if(BackDis < CurrentStepTime * Data.KeepDisMoveSpeed)
            {
                if (XDiff > 0)
                {
                    SpeedManager.SelfSpeed.x = Data.KeepDisMoveSpeed;
                }
                else
                {
                    SpeedManager.SelfSpeed.x = -Data.KeepDisMoveSpeed;
                }
            }
            else if(ForthDis < CurrentStepTime * Data.KeepDisMoveSpeed)
            {
                if (XDiff > 0)
                {
                    SpeedManager.SelfSpeed.x = -Data.KeepDisMoveSpeed;
                }
                else
                {
                    SpeedManager.SelfSpeed.x = Data.KeepDisMoveSpeed;
                }
            }
            else
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    if(XDiff > 0)
                    {
                        SpeedManager.SelfSpeed.x = Data.KeepDisMoveSpeed;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x = -Data.KeepDisMoveSpeed;
                    }
                }
                else
                {
                    if (XDiff > 0)
                    {
                        SpeedManager.SelfSpeed.x = -Data.KeepDisMoveSpeed;
                    }
                    else
                    {
                        SpeedManager.SelfSpeed.x = Data.KeepDisMoveSpeed;
                    }
                }
            }
        }
        else
        {
            StepTimeCount += Time.deltaTime;
            if (StepTimeCount >= CurrentStepTime)
            {
                InStep = false;
            }
        }
    }

    private bool Decision()
    {
        var Data = Entity.GetComponent<KnightData>();
        TimeCount += Time.deltaTime;

        if(TimeCount >= Data.TacticDecisionInterval)
        {
            TimeCount = 0;
            MakeTacticalDecision();
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class KnightChaseForAttack : KnightBehavior
{
    private float DistanceTraveled;

    public override void OnEnter()
    {
        base.OnEnter();
        SetUp();
        SetAppearance();
        CheckStop();
    }

    public override void Update()
    {
        base.Update();
        CheckStop();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void SetUp()
    {
        DistanceTraveled = 0;

        var Data = Entity.GetComponent<KnightData>();

        if (Entity.transform.right.x > 0)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.NormalMoveSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.NormalMoveSpeed;
        }
    }

    private void CheckStop()
    {
        var Data = Entity.GetComponent<KnightData>();
        DistanceTraveled += Data.NormalMoveSpeed * Time.deltaTime;

        float Dis;
        if (Context.CurrentAttackMode == KnightAttackMode.DoubleFirst)
        {
            Dis = 2 * Data.AttackStepForwardSpeed * Data.AttackTime;
        }
        else
        {
            Dis = Data.AttackStepForwardSpeed * Data.AttackTime;
        }

        if (PlayerInHitRange(Data.AttackOffset, Data.AttackHitBoxSize.y, Entity.transform.right, Dis))
        {
            TransitionTo<KnightAttackAnticipation>();
            return;
        }
        else if(DistanceTraveled >= Data.ChaseForAttackDistance)
        {
            TransitionTo<KnightAttackAnticipation>();
            return;
        }
    }
}

public class KnightAttackAnticipation : KnightBehavior
{
    private float TimeCount;
    private float StateTime;

    private float DistanceTraveled;

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
            TransitionTo<KnightGetInterrupted>();
            return;
        }
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
                Context.CurrentStamina--;
                StateTime = Data.SingleAttackAnticipationTime;
                break;
            case KnightAttackMode.DoubleFirst:
                Context.CurrentStamina--;
                StateTime = Data.DoubleAttackFirstAnticipationTime;
                Context.DoubleAttackMark.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case KnightAttackMode.DoubleSecond:
                StateTime = Data.DoubleAttackSecondAnticipationTime;
                break;
            case KnightAttackMode.Chase:
                StateTime = Data.ChaseAttackAnticipationTime;
                Context.CurrentStamina--;
                break;
        }

        Context.CurrentPatience = Data.MaxPatience;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        DistanceTraveled = 0;


    }

    private void ChasePlayer()
    {
        TimeCount += Time.deltaTime;
        if(TimeCount >= StateTime)
        {
            var Data = Entity.GetComponent<KnightData>();

            float Speed = Data.ChaseAttackSpeed;
            bool TraveledEnough = false;

            if (DistanceTraveled+ Data.ChaseAttackSpeed * Time.deltaTime >= Data.MaxChaseAttackDistance)
            {
                TraveledEnough = true;
                Speed = (Data.MaxChaseAttackDistance - DistanceTraveled) / Time.deltaTime;
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

            if (!CheckInRange(Context.Player, Context.DetectLeftX, Context.DetectRightX))
            {
                TransitionTo<KnightPatron>();
            }
            else if (TraveledEnough || XDiff > 0 && Entity.transform.right.x < 0 || XDiff < 0 && Entity.transform.right.x > 0 || PlayerInHitRange(Data.AttackOffset, Data.AttackHitBoxSize.y, Entity.transform.right, Data.AttackStepForwardSpeed * Data.AttackTime))
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
        CheckHitPlayer();
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        GameObject.Destroy(SlashImage);
        AttackHit = false;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<KnightData>();

        TimeCount = 0;
        StateTime = Data.AttackTime;
        Attack = GetAttack();
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
            TransitionTo<KnightGetInterrupted>();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
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
                MakeTacticalDecision();
                //TransitionTo<KnightKeepDistance>();
            }
        }
    }

    private void RectifyDirection()
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

}

public class KnightGetInterrupted : KnightBehavior
{
    private float TimeCount;
    private float MoveTime;
    private float TotalTime;
    private GameObject Source;
    private bool Counter;

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
            SetUp();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= TotalTime)
        {
            if (Counter)
            {
                Context.Player = Source;
                if (Context.CurrentStamina > 0)
                {
                    Context.CurrentPatience = Entity.GetComponent<KnightData>().MaxPatience;
                    MakeAttackDecision();
                }
                else
                {
                    //
                    TransitionTo<KnightKeepDistance>();
                }
            }
            else
            {
                MakeTacticalDecision();
            }

            return;
        }
        else if (TimeCount >= MoveTime)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void SetUp()
    {
        CharacterAttackInfo Temp = (CharacterAttackInfo)Entity.GetComponent<IHittable>().HitAttack;

        var KnightData = Entity.GetComponent<KnightData>();

        float InterruptedSpeed = KnightData.InterruptedSpeed;

        if (Temp.Right)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = InterruptedSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -InterruptedSpeed;
        }

        TimeCount = 0;

        TotalTime = KnightData.InterruptedTime;
        MoveTime = KnightData.InterruptedMoveTime;

        Source = Temp.Source;

        Counter = true;

        Entity.GetComponent<IHittable>().Interrupted = false;
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();

        SetKnight(KnightSpriteData.Hit, KnightSpriteData.HitOffset, KnightSpriteData.HitSize);
    }



}


