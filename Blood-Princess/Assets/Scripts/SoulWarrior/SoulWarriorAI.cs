﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoulWarriorState
{
    Patron,
    Engage,
    SlashAnticipation,
    SlashStrike,
    SlashRecovery,
    MagicAnticipation,
    MagicStrike,
    MagicRecovery,
    BlinkPrepare,
    KnockedBack,
    Offbalance
}

public class SoulWarriorAI : MonoBehaviour
{
    public GameObject Player;
    public LayerMask PlayerLayer;

    public GameObject PatronLeftMark;
    public GameObject PatronRightMark;
    public GameObject DetectLeftMark;
    public GameObject DetectRightMark;

    public GameObject BlinkMark;

    public float DetectLeftX;
    public float DetectRightX;
    public float PatronLeftX;
    public float PatronRightX;

    public float AttackCoolDownTimeCount;

    public SoulWarriorState LastState;
    public SoulWarriorState CurrentState;

    public Vector2 MagicPos;

    private FSM<SoulWarriorAI> SoulWarriorAIFSM;

    // Start is called before the first frame update
    void Start()
    {
        GetPatronInfo();
        Player = CharacterOpenInfo.Self;

        AttackCoolDownTimeCount = GetComponent<SoulWarriorData>().AttackCoolDown;

        SoulWarriorAIFSM =new FSM<SoulWarriorAI>(this);
        SoulWarriorAIFSM.TransitionTo<SoulWarriorPatron>();
    }

    // Update is called once per frame
    void Update()
    {
        SoulWarriorAIFSM.Update();
    }

    private void OnDestroy()
    {
        SoulWarriorAIFSM.CurrentState.CleanUp();
    }

    private void GetPatronInfo()
    {
        DetectLeftX = DetectLeftMark.transform.position.x;
        DetectRightX = DetectRightMark.transform.position.x;
        PatronLeftX = PatronLeftMark.transform.position.x;
        PatronRightX = PatronRightMark.transform.position.x;
    }
}

public abstract class SoulWarriorBehavior : FSM<SoulWarriorAI>.State
{
    protected GameObject Entity;
    protected GameObject SlashImage;

    protected bool InKnockedBack;
    protected float KnockedBackTimeCount;

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

    protected void SetSoulWarrior(Sprite S, Vector2 Offset, Vector2 Size)
    {
        Entity.GetComponent<SpriteRenderer>().sprite = S;
        Entity.GetComponent<SpeedManager>().SetBodyInfo(Offset, Size);
    }

    protected bool CheckGetInterrupted()
    {
        return Entity.GetComponent<IHittable>().Interrupted;
    }

    protected bool CheckOffBalance()
    {
        return Entity.GetComponent<StatusManager_SoulWarrior>().OffBalance;
    }

    protected EnemyAttackInfo GetSlashInfo()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        Direction dir = Direction.Right;
        if(Entity.transform.right.x < 0)
        {
            dir = Direction.Left;
        }

        return new EnemyAttackInfo(Entity, dir, Data.SlashDamage, Data.SlashDamage, Data.SlashOffset, Data.SlashHitBoxSize);
    }

    protected EnemyAttackInfo GetMagicInfo()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        return new EnemyAttackInfo(Entity, Direction.None, Data.MagicDamage, Data.MagicDamage, Vector2.zero , Data.MagicHitBoxSize);
    }

    protected void SetKnockedBack(bool b, CharacterAttackInfo Attack = null)
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        var Status = Entity.GetComponent<StatusManager_SoulWarrior>();

        if (b)
        {
            Status.Interrupted = false;

            InKnockedBack = true;
            KnockedBackTimeCount = 0;
            if (Attack.Dir == Direction.Right)
            {
                Entity.GetComponent<SpeedManager>().ForcedSpeed.x = Data.KnockedBackSpeed;
            }
            else if (Attack.Dir == Direction.Left)
            {
                Entity.GetComponent<SpeedManager>().ForcedSpeed.x = -Data.KnockedBackSpeed;
            }
        }
        else
        {
            InKnockedBack = false;
            Entity.GetComponent<SpeedManager>().ForcedSpeed.x = 0;
        }
    }

    protected void KnockedBackProcess()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        KnockedBackTimeCount += Time.deltaTime;
        if (KnockedBackTimeCount >= Data.KnockedBackTime)
        {
            SetKnockedBack(false);
        }
    }
}

public class SoulWarriorPatron : SoulWarriorBehavior
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
        Context.CurrentState = SoulWarriorState.Patron;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        var PatronData = Entity.GetComponent<PatronData>();
        var Data = Entity.GetComponent<SoulWarriorData>();

        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorKnockedBack>();
            return;
        }

        if (AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX + Data.MagicUseableDis, Context.DetectLeftX - Data.MagicUseableDis, PatronData.DetectHeight, PatronData.DetectLayer, true))
        {
            TransitionTo<SoulWarriorEngage>();
            return;
        }
        Patron();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.Patron;
    }

    private void SetUp()
    {

        var Data = Entity.GetComponent<SoulWarriorData>();
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
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void Patron()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        var PatronData = Entity.GetComponent<PatronData>();
        if (Moving)
        {
            AIUtility.PatronCheckSelfPos(Entity, Context.PatronRightX, Context.PatronLeftX, ref Moving, ref MovingRight);
        }
        else
        {
            AIUtility.CheckPatronStayTime(Entity, ref TimeCount, PatronData.PatronStayTime, ref Moving, ref MovingRight, Data.NormalMoveSpeed);
        }
    }
}

public class SoulWarriorEngage : SoulWarriorBehavior
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.Engage;
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        var PatronData = Entity.GetComponent<PatronData>();
        var Data = Entity.GetComponent<SoulWarriorData>();

        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorKnockedBack>();
            return;
        }

        if (!AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX + Data.MagicUseableDis, Context.DetectLeftX - Data.MagicUseableDis, PatronData.DetectHeight, PatronData.DetectLayer, false))
        {
            TransitionTo<SoulWarriorPatron>();
            return;
        }

        KeepDis();
        CheckAttackCoolDown();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.Engage;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void KeepDis()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        var PlayerSpeedManager = Context.Player.GetComponent<SpeedManager>();

        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();

        AIUtility.RectifyDirection(Context.Player, Entity);

        float XDiff = AIUtility.GetXDiff(Context.Player, Entity);

        float AttackHitDis = (Data.SlashHitBoxSize.x + Data.AttackStepForwardSpeed * Data.SlashStrikeTime) * Data.SlashAvailableHitBoxPercentage;
        float AttackHitBoxBorderToTruePos = Data.SlashOffset.x - Data.SlashHitBoxSize.x / 2 - (SelfSpeedManager.GetTruePos().x - Entity.transform.position.x) * Entity.transform.right.x;

        float AttackDis = AttackHitDis + AttackHitBoxBorderToTruePos;

        if (Mathf.Abs(XDiff) > Data.MagicUseableDis)
        {
            SelfSpeedManager.SelfSpeed.x = Entity.transform.right.x * Data.NormalMoveSpeed;
        }
        else if(Mathf.Abs(XDiff) - PlayerSpeedManager.BodyWidth/2 > AttackDis)
        {
            SelfSpeedManager.SelfSpeed.x = 0;
            if (Context.AttackCoolDownTimeCount <= 0)
            {
                TransitionTo<SoulWarriorMagicAnticipation>();
            }
        }
        else
        {
            SelfSpeedManager.SelfSpeed.x = 0;
            if (Context.AttackCoolDownTimeCount <= 0)
            {
                TransitionTo<SoulWarriorSlashAnticipation>();
            }
        }
    }

    private void CheckAttackCoolDown()
    {
        Context.AttackCoolDownTimeCount -= Time.deltaTime;
    }
}

public class SoulWarriorSlashAnticipation : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.SlashAnticipation;
        SetUp();
        SetAppearance();

    }

    public override void Update()
    {
        base.Update();

        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            SetKnockedBack(true, Entity.GetComponent<StatusManager_SoulWarrior>().CurrentTakenAttack);
            return;
        }

        if (InKnockedBack)
        {
            KnockedBackProcess();
        }

        CheckTime();

    }

    public override void OnExit()
    {
        base.OnExit();
        var Status = Entity.GetComponent<StatusManager_SoulWarrior>();

        Context.LastState = SoulWarriorState.SlashAnticipation;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount = 0;
        StateTime = Data.SlashAnticipationTime;
        Context.AttackCoolDownTimeCount = Data.AttackCoolDown;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.SlashAnticipation, SpriteData.SlashAnticipationOffset, SpriteData.SlashAnticipationSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime && !InKnockedBack)
        {
            TransitionTo<SoulWarriorSlashStrike>();
            return;
        }
    }
}

public class SoulWarriorSlashStrike : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;
    private EnemyAttackInfo Attack;
    private bool AttackHit;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.SlashStrike;
        SetUp();
        SetAppearance();
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
        Context.LastState = SoulWarriorState.SlashStrike;

        GameObject.Destroy(SlashImage);
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.SlashRecovery, SpriteData.SlashRecoveryOffset, SpriteData.SlashRecoverySize);
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount = 0;
        StateTime = Data.SlashStrikeTime;
        Attack = GetSlashInfo();
        AttackHit = false;
        if (Entity.transform.right.x > 0)
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

        var Data = Entity.GetComponent<SoulWarriorData>();

        Vector2 Offset = Data.SlashOffset;
        if (Attack.Dir == Direction.Left)
        {
            Offset.x = -Offset.x;
            EulerAngle = 180;
        }

        SlashImage = GameObject.Instantiate(Data.SlashImage, (Vector2)Entity.transform.position + Offset, Quaternion.Euler(0, EulerAngle, 0));
        SlashImage.transform.parent = Entity.transform;
    }


    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount > StateTime)
        {
            TransitionTo<SoulWarriorSlashRecovery>();
            return;
        }
    }

    private void CheckHitPlayer()
    {
        if (!AttackHit && AIUtility.HitPlayer(Entity.transform.position, Attack, Context.PlayerLayer))
        {
            AttackHit = true;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }
}

public class SoulWarriorSlashRecovery : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.SlashRecovery;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorKnockedBack>();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.SlashRecovery;

    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount = 0;
        StateTime = Data.SlashRecoveryTime;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.SlashRecovery, SpriteData.SlashRecoveryOffset, SpriteData.SlashRecoverySize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<SoulWarriorEngage>();
        }
    }
}

public class SoulWarriorMagicAnticipation : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    private GameObject MagicPrepare;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.MagicAnticipation;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            SetKnockedBack(true, Entity.GetComponent<StatusManager_SoulWarrior>().CurrentTakenAttack);
            return;
        }

        if (InKnockedBack)
        {
            KnockedBackProcess();
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();

        GameObject.Destroy(MagicPrepare);

        Context.LastState = SoulWarriorState.MagicAnticipation;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        var PlayerSpeedManager = Context.Player.GetComponent<SpeedManager>();

        TimeCount = 0;
        StateTime = Data.MagicAnticipationTime;

        float OffsetDis = 0;

        if(PlayerSpeedManager.SelfSpeed.x > 0)
        {
            OffsetDis = Data.MagicPredictionDis;
        }
        else if(PlayerSpeedManager.SelfSpeed.x < 0)
        {
            OffsetDis = -Data.MagicPredictionDis;
        }

        Context.MagicPos = PlayerSpeedManager.GetTruePos() + Vector2.right * OffsetDis;

        MagicPrepare = GameObject.Instantiate(Data.MagicPrepare, Context.MagicPos , Quaternion.Euler(0, 0, 0));

        Context.AttackCoolDownTimeCount = Data.AttackCoolDown;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.MagicAnticipation, SpriteData.MagicAnticipationOffset, SpriteData.MagicAnticipationSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime && !InKnockedBack)
        {
            TransitionTo<SoulWarriorMagicStrike>();
            return;
        }
    }
}

public class SoulWarriorMagicStrike : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    private EnemyAttackInfo MagicAttack;
    private GameObject Magic;

    private bool AttackHit;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.MagicStrike;
        SetUp();
        SetAppearance();
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
        Context.LastState = SoulWarriorState.MagicStrike;
    }


    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount = 0;
        StateTime = Data.MagicStrikeTime;
        MagicAttack = GetMagicInfo();
        Magic = GameObject.Instantiate(Data.Magic, Context.MagicPos, Quaternion.Euler(0, 0, 0));

        AttackHit = false;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.MagicRecovery, SpriteData.MagicRecoveryOffset, SpriteData.MagicRecoverySize);
    }

    private void CheckTime()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount += Time.deltaTime;

        if (TimeCount >= StateTime)
        {
            TransitionTo<SoulWarriorMagicRecovery>();
            return;
        }
    }

    private void CheckHitPlayer()
    {
        if (!AttackHit)
        {
            if(AIUtility.HitPlayer(Magic.transform.position, MagicAttack, Context.PlayerLayer))
            {
                AttackHit = true;
            }
        }
    }
}
public class SoulWarriorMagicRecovery : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.MagicRecovery;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorKnockedBack>();
            return;
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.MagicRecovery;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        TimeCount = 0;
        StateTime = Data.MagicRecoveryTime;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.MagicRecovery, SpriteData.MagicRecoveryOffset, SpriteData.MagicRecoverySize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<SoulWarriorEngage>();
        }
    }
}

public class SoulWarriorBlink : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.BlinkPrepare;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            SetKnockedBack(true, Entity.GetComponent<StatusManager_SoulWarrior>().CurrentTakenAttack);
            return;
        }

        if (InKnockedBack)
        {
            KnockedBackProcess();
        }


        var PatronData = Entity.GetComponent<PatronData>();
        var Data = Entity.GetComponent<SoulWarriorData>();

        if(!AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX + Data.MagicUseableDis, Context.DetectLeftX -Data.MagicUseableDis, PatronData.DetectHeight, PatronData.DetectLayer, false))
        {
            TransitionTo<SoulWarriorPatron>();
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.BlinkMark.GetComponent<SpriteRenderer>().enabled = false;
        Context.LastState = SoulWarriorState.BlinkPrepare;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        TimeCount = 0;
        StateTime = Data.BlinkPrepareTime;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;

        Context.BlinkMark.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime && !InKnockedBack)
        {
            Blink();
            TransitionTo<SoulWarriorSlashAnticipation>();
        }
    }

    private void Blink()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();
        var PlayerSpeedManager = Context.Player.GetComponent<SpeedManager>();

        GameObject.Instantiate(Data.BlinkEffect, SelfSpeedManager.GetTruePos(), Quaternion.Euler(0, 0, 0));

        bool BlinkToRight;

        if (AIUtility.GetXDiff(Context.Player, Entity) < 0)
        {
            BlinkToRight = false;
        }
        else
        {
            BlinkToRight = true;
        }

        float AttackHitDis = (Data.SlashHitBoxSize.x + Data.AttackStepForwardSpeed * Data.SlashStrikeTime) * Data.SlashAvailableHitBoxPercentage;

        float AttackHitBoxBorderToTruePos = Data.SlashOffset.x - Data.SlashHitBoxSize.x / 2 - (SelfSpeedManager.GetTruePos().x - Entity.transform.position.x) * Entity.transform.right.x;


        float BlinkPosX;

        if (BlinkToRight)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            BlinkPosX = PlayerSpeedManager.GetTruePos().x + PlayerSpeedManager.BodyWidth/2 + AttackHitDis + AttackHitBoxBorderToTruePos;
            if (BlinkPosX > Context.DetectRightX)
            {
                BlinkPosX = Context.DetectRightX;
            }
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            BlinkPosX = PlayerSpeedManager.GetTruePos().x - PlayerSpeedManager.BodyWidth / 2 - AttackHitDis - AttackHitBoxBorderToTruePos;
            if (BlinkPosX < Context.DetectLeftX)
            {
                BlinkPosX = Context.DetectLeftX;
            }
        }

        SelfSpeedManager.MoveToPoint(new Vector2(BlinkPosX, SelfSpeedManager.GetTruePos().y));

        GameObject.Instantiate(Data.BlinkEffect, SelfSpeedManager.GetTruePos(), Quaternion.Euler(0, 0, 0));
    }
}

public class SoulWarriorKnockedBack : SoulWarriorBehavior
{
    private float TimeCount;
    private float StateTime;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.KnockedBack;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }
        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.KnockedBack;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        var Status = Entity.GetComponent<StatusManager_SoulWarrior>();

        Status.Interrupted = false;

        StateTime = Data.KnockedBackTime;
        TimeCount = 0;

        Context.AttackCoolDownTimeCount = 0;

        if (Status.CurrentTakenAttack.Dir == Direction.Right)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.KnockedBackSpeed;
        }
        else if (Status.CurrentTakenAttack.Dir == Direction.Left)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.KnockedBackSpeed;
        }
    }

    private void SetAppearance()
    {
        var SoulWarriorSpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SoulWarriorSpriteData.Idle, SoulWarriorSpriteData.IdleOffset, SoulWarriorSpriteData.IdleSize);
    }

    private void CheckTime()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
        {
            TransitionTo<SoulWarriorBlink>();
        }
    }
}

public class SoulWarriorOffBalance : SoulWarriorBehavior
{
    private float TimeCount;
    private float BackTime;
    private float StayTime;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.CurrentState = SoulWarriorState.Offbalance;
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        if (CheckOffBalance())
        {
            TransitionTo<SoulWarriorOffBalance>();
            return;
        }

        if (CheckGetInterrupted())
        {
            SetKnockedBack(true, Entity.GetComponent<StatusManager_SoulWarrior>().CurrentTakenAttack);
        }

        if (InKnockedBack)
        {
            KnockedBackProcess();
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.Offbalance;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();
        var Status = Entity.GetComponent<StatusManager_SoulWarrior>();

        Status.OffBalance = false;
        Status.Interrupted = false;

        BackTime = Data.OffBalanceBackTime;
        StayTime = Data.OffBalanceStayTime;
        TimeCount = 0;

        Context.AttackCoolDownTimeCount = 0;

        AIUtility.RectifyDirection(Context.Player, Entity);

        if (Status.CurrentTakenAttack.Dir == Direction.Right)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.OffBalanceBackSpeed;
        }
        else if (Status.CurrentTakenAttack.Dir == Direction.Left)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.OffBalanceBackSpeed;
        }


    }

    private void SetAppearance()
    {
        var SoulWarriorSpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SoulWarriorSpriteData.Hit, SoulWarriorSpriteData.HitOffset, SoulWarriorSpriteData.HitSize);
    }

    private void CheckTime()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount += Time.deltaTime;
        if (TimeCount >= BackTime + StayTime && !InKnockedBack)
        {
            TransitionTo<SoulWarriorBlink>();
        }
        else if (TimeCount >= BackTime)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }
}
