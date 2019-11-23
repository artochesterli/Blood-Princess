using System.Collections;
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
    Interrupted
}

public class SoulWarriorAI : MonoBehaviour
{
    public GameObject Player;
    public LayerMask PlayerLayer;

    public GameObject AttackMark;
    public GameObject Magic;

    public GameObject PatronLeftMark;
    public GameObject PatronRightMark;
    public GameObject DetectLeftMark;
    public GameObject DetectRightMark;

    public float DetectLeftX;
    public float DetectRightX;
    public float PatronLeftX;
    public float PatronRightX;

    public float AttackCoolDownTimeCount;

    public SoulWarriorState LastState;

    private FSM<SoulWarriorAI> SoulWarriorAIFSM;

    // Start is called before the first frame update
    void Start()
    {
        GetPatronInfo();
        Player = CharacterOpenInfo.Self;

        SoulWarriorAIFSM=new FSM<SoulWarriorAI>(this);
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

    protected void SetSoulWarrior(Sprite S, Vector2 Offset, Vector2 Size)
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

    protected EnemyAttackInfo GetSlashInfo()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        return new EnemyAttackInfo(Entity, Entity.transform.right.x > 0, Data.SlashDamage, Data.SlashDamage, Data.SlashOffset, Data.SlashHitBoxSize);
    }

    protected void GetMagicInfo(GameObject Magic, ref EnemyAttackInfo Left,ref EnemyAttackInfo Right)
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        Left = new EnemyAttackInfo(Magic, false, Data.MagicDamage, Data.MagicDamage, Vector2.right * Data.MagicHalfHitBoxSize.x / 2 , Data.MagicHalfHitBoxSize);
        Right = new EnemyAttackInfo(Magic, true, Data.MagicDamage, Data.MagicDamage, Vector2.right * Data.MagicHalfHitBoxSize.x / 2 , Data.MagicHalfHitBoxSize);
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
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        var PatronData = Entity.GetComponent<PatronData>();

        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorGetInterrupted>();
            return;
        }
        if (AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX, Context.DetectLeftX, PatronData.DetectHeight, PatronData.DetectLayer, true))
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
            AIUtility.CheckPatronStayTime(Entity, ref TimeCount, PatronData.PatronStayTime, ref Moving, MovingRight, Data.NormalMoveSpeed);
        }
    }
}

public class SoulWarriorEngage : SoulWarriorBehavior
{
    public override void OnEnter()
    {
        base.OnEnter();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();

        var PatronData = Entity.GetComponent<PatronData>();

        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorGetInterrupted>();
            return;
        }
        if (!AIUtility.PlayerInDetectRange(Entity, Context.Player, Context.DetectRightX, Context.DetectLeftX, PatronData.DetectHeight, PatronData.DetectLayer, false))
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

        if (Mathf.Abs(XDiff) > Data.MagicUseableDis)
        {
            SelfSpeedManager.SelfSpeed.x = Entity.transform.right.x * Data.NormalMoveSpeed;
        }
        else
        {
            SelfSpeedManager.SelfSpeed.x = 0;
            if (Context.AttackCoolDownTimeCount <= 0)
            {
                TransitionTo<SoulWarriorMagicAnticipation>();
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
        var Status = Entity.GetComponent<StatusManager_SoulWarrior>();

        Status.Interrupted = false;

        Context.AttackMark.SetActive(false);

        Context.LastState = SoulWarriorState.SlashAnticipation;
    }

    private void SetUp()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount = 0;
        StateTime = Data.SlashAnticipationTime;
        Context.AttackCoolDownTimeCount = Data.AttackCoolDown;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Attack, SpriteData.AttackOffset, SpriteData.AttackSize);

        Context.AttackMark.SetActive(true);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
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
        SetUp();
        SetAppearance();
        GenerateSlashImage();
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorGetInterrupted>();
            return;
        }
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
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);

        Context.AttackMark.SetActive(false);
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

        if (!Attack.Right)
        {
            EulerAngle = 180;
        }

        var Data = Entity.GetComponent<SoulWarriorData>();

        Vector2 Offset = Data.SlashOffset;
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
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorGetInterrupted>();
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
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
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

        Entity.GetComponent<StatusManager_SoulWarrior>().Interrupted = false;

        Context.LastState = SoulWarriorState.MagicAnticipation;
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

        Context.Magic = GameObject.Instantiate(Data.MagicPrefab, PlayerSpeedManager.GetTruePos() + Vector2.right * OffsetDis, Quaternion.Euler(0, 0, 0));

        Context.AttackCoolDownTimeCount = Data.AttackCoolDown;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Attack, SpriteData.AttackOffset, SpriteData.AttackSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
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

    private EnemyAttackInfo Left;
    private EnemyAttackInfo Right;
    private bool AttackHit;

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
            TransitionTo<SoulWarriorGetInterrupted>();
            return;
        }
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
        GetMagicInfo(Context.Magic, ref Left, ref Right);
        AttackHit = false;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void CheckTime()
    {
        var Data = Entity.GetComponent<SoulWarriorData>();

        TimeCount += Time.deltaTime;
        Context.Magic.transform.localScale = Vector3.one * Mathf.Lerp(1, Data.MagicHalfHitBoxSize.x * 2, TimeCount / StateTime);
        Color color = Context.Magic.GetComponent<SpriteRenderer>().color;
        Context.Magic.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, Mathf.Lerp(1, 0, TimeCount / StateTime));
        if (TimeCount >= StateTime)
        {
            GameObject.Destroy(Context.Magic);
            TransitionTo<SoulWarriorMagicRecovery>();
            return;
        }
    }

    private void CheckHitPlayer()
    {
        if (!AttackHit)
        {
            if(AIUtility.HitPlayer(Context.Magic.transform.position, Left, Context.PlayerLayer))
            {
                AttackHit = true;
            }
            else if(AIUtility.HitPlayer(Context.Magic.transform.position, Right, Context.PlayerLayer))
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
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorGetInterrupted>();
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
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
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
        SetUp();
        SetAppearance();
    }

    public override void Update()
    {
        base.Update();
        if (CheckGetInterrupted())
        {
            TransitionTo<SoulWarriorGetInterrupted>();
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
        var Data = Entity.GetComponent<SoulWarriorData>();
        TimeCount = 0;
        StateTime = Data.BlinkPrepareTime;

        Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= StateTime)
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
    }
}

public class SoulWarriorGetInterrupted : SoulWarriorBehavior
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
            TransitionTo<SoulWarriorGetInterrupted>();
            return;
        }

        CheckTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.LastState = SoulWarriorState.Interrupted;
    }

    private void SetUp()
    {
        CharacterAttackInfo Temp = Entity.GetComponent<StatusManager_SoulWarrior>().CurrentTakenAttack;

        var Data = Entity.GetComponent<SoulWarriorData>();
        var Status = Entity.GetComponent<StatusManager_SoulWarrior>();

        Status.Interrupted = false;

        if (Temp.Dir == Direction.Right)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = Data.KnockedBackSpeed;
        }
        else if(Temp.Dir == Direction.Left)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -Data.KnockedBackSpeed;
        }

        TimeCount = 0;
        StateTime = Data.KnockedBackTime;
    }

    private void SetAppearance()
    {
        var SpriteData = Entity.GetComponent<SoulWarriorSpriteData>();
        SetSoulWarrior(SpriteData.Idle, SpriteData.IdleOffset, SpriteData.IdleSize);
    }

    private void CheckTime()
    {
        TimeCount += Time.deltaTime;

        if (TimeCount >= StateTime)
        {
            TransitionTo<SoulWarriorBlink>();
            return;
        }

    }
}
