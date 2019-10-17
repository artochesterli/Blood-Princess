using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KnightAttackMode
{
    Single,
    DoubleFirst,
    DoubleSecond
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
    public int CurrentKeepDistance;

    private FSM<KnightAI> KnightAIFSM;
    // Start is called before the first frame update
    void Start()
    {
        GetPatronInfo();
        CurrentStamina = GetComponent<KnightData>().MaxStamina;

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
        //Debug.Log(this.GetType().Name);
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

        RaycastHit2D hit = Physics2D.BoxCast(Entity.transform.position + Offset.y*Vector3.up, new Vector2(BoxThickness, Height), 0, Dir,AttackStepForwardDis + Mathf.Abs(Offset.x)-BoxThickness/2, Context.PlayerLayer);

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

        if (Context.CurrentStamina > 0)
        {
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
                TransitionTo<KnightKeepDistance>();
            }
        }
        
    }

    protected void MakeTacticalDecision()
    {
        var Data = Entity.GetComponent<KnightData>();

        bool PlayerInShortDis = PlayerInHitRange(Data.AttackOffset, Data.AttackHitBoxSize.y, Entity.transform.right, Data.AttackStepForwardSpeed * Data.AttackTime + Data.ChaseForAttackDistance);
        bool PlayerInLongDis = PlayerInHitRange(Data.AttackOffset, Data.AttackHitBoxSize.y, Entity.transform.right, 2 * Data.AttackStepForwardSpeed * Data.AttackTime + Data.ChaseForAttackDistance);

        if (Context.CurrentStamina > 0)
        {
            if (Context.CurrentKeepDistance >= Data.MaxKeepDistanceDecision)
            {
                MakeAttackDecision();
            }
            else
            {
                float DecisionNumber = Random.Range(0.0f, 1.0f);

                if (DecisionNumber < Data.ShortDisAttackDecisionChance)
                {
                    MakeAttackDecision();
                }
                else
                {
                    TransitionTo<KnightKeepDistance>();
                }
            }
        }
        else
        {
            TransitionTo<KnightKeepDistance>();
        }
    }

    protected EnemyAttackInfo GetAttack()
    {
        var KnightData = Entity.GetComponent<KnightData>();

        return new EnemyAttackInfo(Entity, EnemyAttackType.Strike, Entity.transform.right.x > 0, KnightData.Damage, KnightData.AttackOffset, KnightData.AttackHitBoxSize);
    }

}

public class KnightPatron : KnightBehavior
{
    private float TimeCount;
    private bool Moving;
    private bool MovingRight;
    

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
        if (PlayerInDetectRange())
        {
            TransitionTo<KnightMoveClose>();
            return;
        }
        Patron();
    }

    private void SetUp()
    {
        var KnightData = Entity.GetComponent<KnightData>();

        Context.Player = null;
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
        var KnightData = Entity.GetComponent<KnightData>();
        TimeCount += Time.deltaTime;
        if (TimeCount >= Entity.GetComponent<PatronData>().PatronStayTime)
        {
            TimeCount = 0;
            Moving = true;
            if (MovingRight)
            {
                Entity.transform.rotation = Quaternion.Euler(0, 0, 0);
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = KnightData.NormalMoveSpeed;
            }
            else
            {
                Entity.transform.rotation = Quaternion.Euler(0, 180, 0);
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -KnightData.NormalMoveSpeed;
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

}

public class KnightMoveClose : KnightBehavior
{

    public override void OnEnter()
    {
        base.OnEnter();

        SetSpeed();
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

        SetSpeed();
        MoveClose();
    }

    private void SetSpeed()
    {
        if(GetXDiff() > 0)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.GetComponent<KnightData>().NormalMoveSpeed;
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Entity.GetComponent<KnightData>().NormalMoveSpeed;
        }
    }

    private void SetAppearance()
    {
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);
    }

    private void MoveClose()
    {
        var Data = Entity.GetComponent<KnightData>();

        if (!CheckInRange(Context.Player,Context.DetectLeftX,Context.DetectRightX))
        {
            TransitionTo<KnightPatron>();
            return;
        }

        if (Mathf.Abs(GetXDiff()) < Data.TacticDistance)
        {
            MakeTacticalDecision();
            return;
        }
    }
}

public class KnightKeepDistance : KnightBehavior
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
        if (PlayerInRecovery())
        {
            MakeAttackDecision();
        }
        KeepDis();
        Decision();
    }

    private void SetUp()
    {
        TimeCount = 0;
        if(Context.CurrentStamina < Entity.GetComponent<KnightData>().MaxStamina)
        {
            Context.CurrentStamina++;
        }
        Context.CurrentKeepDistance++;
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
            TransitionTo<KnightMoveClose>();
            return;
        }
        else if (Mathf.Abs(XDiff) > Data.DangerDistance)
        {

            if (XDiff > 0)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.KeepDisMoveSpeed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.KeepDisMoveSpeed;
            }
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else
        {
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

    private void Decision()
    {
        var Data = Entity.GetComponent<KnightData>();
        TimeCount += Time.deltaTime;

        if(TimeCount > Data.TacticDecisionInterval)
        {
            MakeTacticalDecision();
            return;
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

        Context.CurrentKeepDistance = 0;
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
            TransitionTo<KnightKeepDistance>();
            return;
        }
    }
}

public class KnightAttackAnticipation : KnightBehavior
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
        }
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

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

        if (Context.CurrentAttackMode != KnightAttackMode.DoubleSecond)
        {
            Context.CurrentStamina--;
        }
    }

    private void GenerateSlashImage()
    {
        float EulerAngle = 0;

        if (!Attack.Right)
        {
            EulerAngle = 180;
        }

        Vector2 Offset = Entity.GetComponent<KnightData>().AttackOffset;
        if (!Attack.Right)
        {
            Offset.x = -Offset.x;
        }

        SlashImage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/KnightSlash"), (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
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
            if (Hit.collider.gameObject.GetComponent<StatusManager_Character>().Invulnerable)
            {
                return false;
            }
            else
            {
                Hit.collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
                return true;
            }
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
        StateTime = Data.AttackRecoveryTime;
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

}

public class KnightGetInterrupted : KnightBehavior
{
    private float TimeCount;
    private float MoveTime;
    private float TotalTime;
    private GameObject Source;

    public override void OnEnter()
    {
        base.OnEnter();
        Interrupted();
    }

    private void Interrupted()
    {
        CharacterAttackInfo Temp = (CharacterAttackInfo)Entity.GetComponent<IHittable>().HitAttack;

        var KnightData = Entity.GetComponent<KnightData>();

        float Speed;

        Speed = KnightData.InterruptedSpeed;

        if (Temp.Right)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Speed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Speed;
        }

        TimeCount = 0;

        TotalTime = KnightData.InterruptedTime;
        MoveTime = KnightData.InterruptedMoveTime;


        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();

        SetKnight(KnightSpriteData.Hit, KnightSpriteData.HitOffset, KnightSpriteData.HitSize);

        Source = Temp.Source;

        Entity.GetComponent<IHittable>().Interrupted = false;

        if(Context.CurrentStamina < KnightData.MaxStamina)
        {
            Context.CurrentStamina++;
        }

        GameObject.Destroy(SlashImage);
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            Interrupted();
            return;
        }
        CheckTime();
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= TotalTime)
        {
            Context.Player = Source;
            MakeAttackDecision();

            return;
        }
        else if (TimeCount >= MoveTime)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }


    public override void OnExit()
    {
        base.OnExit();
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }
}


