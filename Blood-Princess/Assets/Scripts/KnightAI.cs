using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAI : MonoBehaviour
{
    public GameObject Player;
    public Vector2 InitPosition;

    private FSM<KnightAI> KnightAIFSM;
    // Start is called before the first frame update
    void Start()
    {
        InitPosition = transform.position;
        KnightAIFSM = new FSM<KnightAI>(this);
        KnightAIFSM.TransitionTo<KnightPatron>();
    }

    // Update is called once per frame
    void Update()
    {
        KnightAIFSM.Update();
    }
}

public abstract class KnightBehavior : FSM<KnightAI>.State
{
    protected GameObject Entity;
    protected float AttackTimeCount;
    protected AttackState CurrentAttackState;
    protected bool AttackHit;
    protected float AnticipationTime;
    protected float AttackTime;
    protected float RecoveryTime;
    protected bool PatronRight;
    protected GameObject SlashImage;

    protected Vector2 OriPos;

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

    protected void SetBackToPatron()
    {
        Context.Player = null;

        var Data = Entity.GetComponent<PatronData>();
        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        if (TruePos.x > Data.PatronRange.y + OriPos.x)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            PatronRight = false;
        }
        else if(TruePos.x < Data.PatronRange.x + OriPos.x)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            PatronRight = true;
        }
        else
        {
            if (Entity.transform.right.x > 0)
            {
                PatronRight = true;
            }
            else
            {
                PatronRight = false;
            }
        }

    }

    protected bool DetectPlayer()
    {
        int Mask = 1 << LayerMask.NameToLayer("Player");

        var Data = Entity.GetComponent<PatronData>();

        Vector2 Init = Quaternion.Euler(0, 0, -Data.DetectAngle / 2) * Entity.transform.right;

        Vector2 Dir = Init;
        float Angle = 0;

        while (Angle < Data.DetectAngle)
        {
            Dir = Quaternion.Euler(0, 0, Angle) * Init;

            RaycastHit2D hit = Physics2D.Raycast(Entity.GetComponent<SpeedManager>().GetTruePos(), Dir, (Data.AlertXRange.y - Data.AlertXRange.x)/2, Mask);
            if (hit)
            {
                if (CheckInRange(hit.collider.gameObject, OriPos.x + Data.AlertXRange.x, OriPos.x + Data.AlertXRange.y))
                {
                    Context.Player = hit.collider.gameObject;
                    return true;
                }
                else
                {
                    break;
                }
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

    protected bool CheckPlayerInHitRange(Vector2 Offset, float Height, Vector2 Dir, float Dis)
    {
        int Mask = 1 << LayerMask.NameToLayer("Player");

        RaycastHit2D hit = Physics2D.BoxCast(Entity.transform.position + Offset.y*Vector3.up, new Vector2(0.01f, Height), 0, Dir,Dis + Mathf.Abs(Offset.x), Mask);

        return hit;
    }

    protected bool CheckGetHit()
    {
        if (Entity.GetComponent<IHittable>().hit)
        {
            TransitionTo<KnightGetHit>();
            return true;
        }
        else
        {
            return false;
        }
    }

    
}

public class KnightPatron : KnightBehavior
{
    private float TimeCount;
    private bool Moving;

    public override void OnEnter()
    {
        base.OnEnter();

        var KnightData = Entity.GetComponent<KnightData>();

        Moving = true;
        if (Entity.transform.right.x > 0)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = KnightData.MoveSpeed;
            PatronRight = true;
            
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -KnightData.MoveSpeed;
            PatronRight = false;
        }

        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);

        OriPos = Context.InitPosition + Entity.GetComponent<SpeedManager>().OriPos;
    }

    public override void Update()
    {
        base.Update();

        if (CheckGetHit())
        {
            return;
        }
        Patron();
        if (DetectPlayer())
        {
            TransitionTo<KnightChase>();
        }
    }

    private void Patron()
    {
        if (Moving)
        {
            CheckSelfInRange();
        }
        else
        {
            var KnightData = Entity.GetComponent<KnightData>();
            TimeCount += Time.deltaTime;
            if (TimeCount > Entity.GetComponent<PatronData>().PatronStayTime)
            {
                TimeCount = 0;
                Moving = true;
                if (PatronRight)
                {
                    Entity.transform.rotation = Quaternion.Euler(0, 0, 0);
                    Entity.GetComponent<SpeedManager>().SelfSpeed.x = KnightData.MoveSpeed;
                }
                else
                {
                    Entity.transform.rotation = Quaternion.Euler(0, 180, 0);
                    Entity.GetComponent<SpeedManager>().SelfSpeed.x = -KnightData.MoveSpeed;
                }
            }
        }
    }

    private void CheckSelfInRange()
    {
        var Data = Entity.GetComponent<PatronData>();
        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        if (PatronRight && TruePos.x > Data.PatronRange.y + OriPos.x)
        {
            PatronRight = false;
            Moving = false;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else if (!PatronRight && TruePos.x < Data.PatronRange.x + OriPos.x)
        {
            PatronRight = true;
            Moving = false;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    
}

public class KnightAttack : KnightBehavior
{
    private EnemyAttackInfo Attack;
    private float MaxDashDis;
    private float DashTraveled;

    public override void OnEnter()
    {
        base.OnEnter();
        CurrentAttackState = AttackState.Anticipation;
        AttackTimeCount = 0;
        DashTraveled = 0;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        AttackHit = false;

        GetAttack();

        var KnightData = Entity.GetComponent<KnightData>();
        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Anticipation, KnightSpriteData.AnticipationOffset, KnightSpriteData.AnticipationSize);

        OriPos = Context.InitPosition + Entity.GetComponent<SpeedManager>().OriPos;

        if (Entity.GetComponent<IRage>().Rage)
        {
            MaxDashDis = KnightData.RageDashDistance;
        }
        else
        {
            MaxDashDis = KnightData.NormalDashDistance;
        }
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetHit())
        {
            return;
        }
        CheckAttackTimeCount(AnticipationTime, AttackTime, RecoveryTime);
        CheckHit();
    }

    private void GetAttack()
    {
        var KnightData = Entity.GetComponent<KnightData>();
        AnticipationTime = KnightData.AttackAnticipation;
        AttackTime = KnightData.AttackTime;
        RecoveryTime = KnightData.AttackRecovery;

        Attack = new EnemyAttackInfo(Entity, Entity.transform.right.x > 0, KnightData.Damage, KnightData.AttackOffset, KnightData.AttackHitBoxSize);
    }

    private void CheckHit()
    {
        if (AttackHit)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else if (CurrentAttackState == AttackState.Attack)
        {
            if (AttackHitPlayer(Attack))
            {
                AttackHit = true;
            }
        }
    }

    private bool AttackHitPlayer(EnemyAttackInfo Attack)
    {
        int Mask = 1 << LayerMask.NameToLayer("Player");

        Vector2 Offset = Attack.HitBoxOffset;
        Offset.x = Entity.transform.right.x * Offset.x;

        RaycastHit2D[] AllHits = Physics2D.BoxCastAll(Entity.transform.position + (Vector3)Offset, Attack.HitBoxSize, 0, Entity.transform.right, 0, Mask);
        if (AllHits.Length > 0)
        {
            for (int i = 0; i < AllHits.Length; i++)
            {
                if (!AllHits[i].collider.gameObject.GetComponent<IHittable>().hit)
                {
                    AllHits[i].collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
                }
            }
            return true;
        }
        else
        {
            return false;
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
    }

    private void TransferToAttack()
    {
        GenerateSlashImage();
        CurrentAttackState = AttackState.Attack;
        AttackTimeCount = 0;
        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Recovery, KnightSpriteData.RecoveryOffset, KnightSpriteData.RecoverySize);

        OriPos = Context.InitPosition + Entity.GetComponent<SpeedManager>().OriPos;
    }

    private void CheckAttackTimeCount(float AnticipationTime, float AttackTime, float RecoveryTime)
    {
        AttackTimeCount += Time.deltaTime;
        switch (CurrentAttackState)
        {
            case AttackState.Anticipation:
                if (AttackTimeCount > AnticipationTime)
                {
                    CurrentAttackState = AttackState.Charge;
                    AttackTimeCount = 0;
                }
                break;
            case AttackState.Charge:
                float PlayerX = Context.Player.GetComponent<SpeedManager>().GetTruePos().x;
                float SelfX = Entity.GetComponent<SpeedManager>().GetTruePos().x;

                SelfX += Entity.transform.right.x * Entity.GetComponent<KnightData>().AttackOffset.x / 2;
                if (Attack.Right)
                {
                    PlayerX -= Context.Player.GetComponent<SpeedManager>().BodyWidth / 2;
                    if (PlayerX - SelfX > 0 && DashTraveled<MaxDashDis)
                    {
                        float DashSpeed = Entity.GetComponent<KnightData>().DashSpeed;
                        if (Entity.GetComponent<IRage>().Rage)
                        {
                            DashSpeed = Entity.GetComponent<KnightData>().RageDashSpeed;
                        }
                        

                        float DashDis =  DashSpeed* Time.deltaTime;

                        bool Transfer = false;

                        if(DashDis > PlayerX - SelfX)
                        {
                            DashSpeed = (PlayerX - SelfX) / Time.deltaTime;
                            Transfer = true;
                        }

                        if (DashDis + DashTraveled > MaxDashDis)
                        {
                            DashDis = MaxDashDis - DashTraveled;
                            Transfer = true;
                        }

                        Entity.GetComponent<SpeedManager>().SelfSpeed.x = DashSpeed;
                        DashTraveled += DashSpeed * Time.deltaTime;

                        if (Transfer)
                        {
                            TransferToAttack();
                        }
                    }
                    else
                    {
                        TransferToAttack();
                    }
                }
                else
                {
                    PlayerX += Context.Player.GetComponent<SpeedManager>().BodyWidth / 2;
                    if (SelfX - PlayerX > 0 && DashTraveled < MaxDashDis)
                    {
                        float DashSpeed = Entity.GetComponent<KnightData>().DashSpeed;
                        if (Entity.GetComponent<IRage>().Rage)
                        {
                            DashSpeed = Entity.GetComponent<KnightData>().RageDashSpeed;
                        }

                        float DashDis = DashSpeed * Time.deltaTime;

                        bool Transfer = false;

                        if (DashDis > SelfX-PlayerX)
                        {
                            DashSpeed = (SelfX-PlayerX) / Time.deltaTime;
                            Transfer = true;
                        }

                        if (DashDis + DashTraveled > MaxDashDis)
                        {
                            DashDis = MaxDashDis - DashTraveled;
                            Transfer = true;
                        }

                        Entity.GetComponent<SpeedManager>().SelfSpeed.x = -DashSpeed;
                        DashTraveled += DashSpeed * Time.deltaTime;

                        if (Transfer)
                        {
                            TransferToAttack();
                        }
                    }
                    else
                    {
                        TransferToAttack();
                    }
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
                    TransitionTo<KnightAttackCoolDown>();
                }
                break;
        }

    }
}

public class KnightAttackCoolDown : KnightBehavior
{
    private float TimeCount;
    private float CoolDown;

    public override void OnEnter()
    {
        base.OnEnter();
        TimeCount = 0;

        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);

        OriPos = Context.InitPosition + Entity.GetComponent<SpeedManager>().OriPos;

        if (Entity.GetComponent<IRage>().Rage)
        {
            CoolDown = Entity.GetComponent<KnightData>().RageAttackCoolDown;
        }
        else
        {
            CoolDown = Entity.GetComponent<KnightData>().AttackCoolDown;
        }
    }

    public override void Update()
    {
        base.Update();

        if (CheckGetHit())
        {
            return;
        }
        CheckCoolDown();
    }

    private void CheckCoolDown()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > CoolDown)
        {
            TransitionTo<KnightChase>();
        }
    }
}

public class KnightChase : KnightBehavior
{
    private float DetectDis;
    private Vector2 Offset;
    private float Height;
    private Vector2 Dir;

    public override void OnEnter()
    {
        base.OnEnter();
        var KnightData = Entity.GetComponent<KnightData>();
        
        if (Entity.GetComponent<IRage>().Rage)
        {
            DetectDis = KnightData.RageDashDistance;
        }
        else
        {
            DetectDis = KnightData.NormalDashDistance;
        }

        Offset = KnightData.AttackOffset;
        Height = KnightData.AttackHitBoxSize.y;

        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Idle, KnightSpriteData.IdleOffset, KnightSpriteData.IdleSize);

        OriPos = Context.InitPosition + Entity.GetComponent<SpeedManager>().OriPos;
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetHit())
        {
            return;
        }
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        var PatronData = Entity.GetComponent<PatronData>();

        if (!CheckInRange(Context.Player, OriPos.x + PatronData.AlertXRange.x, OriPos.x + PatronData.AlertXRange.y))
        {
            SetBackToPatron();
            TransitionTo<KnightPatron>();
            return;
        }

        if (CheckPlayerInHitRange(Offset, Height, Entity.transform.right, DetectDis))
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
            TransitionTo<KnightAttack>();
        }
        else
        {
            float PlayerX = Context.Player.GetComponent<SpeedManager>().GetTruePos().x;
            float SelfX = Entity.transform.position.x;

            SelfX += Entity.transform.right.x * Entity.GetComponent<KnightData>().AttackOffset.x / 2;

            if (PlayerX > SelfX)
            {

                Entity.transform.eulerAngles = new Vector3(0, 0, 0);
                PlayerX -= Context.Player.GetComponent<SpeedManager>().BodyWidth / 2;
                if (PlayerX - SelfX > DetectDis)
                {
                    Entity.GetComponent<SpeedManager>().SelfSpeed.x = Entity.GetComponent<KnightData>().MoveSpeed;
                }
                else
                {
                    Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
                }
            }
            else
            {
                Entity.transform.eulerAngles = new Vector3(0, 180, 0);
                PlayerX += Context.Player.GetComponent<SpeedManager>().BodyWidth / 2;

                if (SelfX - PlayerX > DetectDis)
                {
                    Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Entity.GetComponent<KnightData>().MoveSpeed;
                }
                else
                {
                    Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
                }
            }

        }
    }
}

public class KnightGetHit : KnightBehavior
{
    private float GetHitTimeCount;
    private GameObject Source;

    public override void OnEnter()
    {
        base.OnEnter();
        CharacterAttackInfo Temp = (CharacterAttackInfo)Entity.GetComponent<IHittable>().HitAttack;

        if (Temp.Right)
        {
            Entity.GetComponent<SpeedManager>().ForcedSpeed.x = Entity.GetComponent<KnightData>().GetHitSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().ForcedSpeed.x = -Entity.GetComponent<KnightData>().GetHitSpeed;
        }
        
        GetHitTimeCount = 0;

        var KnightSpriteData = Entity.GetComponent<KnightSpriteData>();
        SetKnight(KnightSpriteData.Hit, KnightSpriteData.HitOffset, KnightSpriteData.HitSize);

        OriPos = Context.InitPosition + Entity.GetComponent<SpeedManager>().OriPos;

        Source = Temp.Source;

        GameObject.Destroy(SlashImage);
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetHit())
        {
            return;
        }
        CheckTimeCount();
    }

    private void CheckTimeCount()
    {
        GetHitTimeCount += Time.deltaTime;
        if (GetHitTimeCount >= Entity.GetComponent<KnightData>().GetHitTime)
        {
            Context.Player = Source;
            TransitionTo<KnightChase>();
        }
        else if (GetHitTimeCount >= Entity.GetComponent<KnightData>().GetHitMoveTime)
        {
            Entity.GetComponent<SpeedManager>().ForcedSpeed.x = 0;
        }
    }


    public override void OnExit()
    {
        base.OnExit();
        Entity.GetComponent<SpeedManager>().ForcedSpeed.x = 0;
        Entity.GetComponent<IHittable>().hit = false;
    }
}


