using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;
    public int CurrentAdvancedEnergy;
    public EnemyAttackInfo CurrentTakenAttack;

    public GameObject Canvas;
    public GameObject HPFill;
    public GameObject EnergyFill;
    public GameObject AdvancedEnergyFill;
    public GameObject CriticalEyeMark;

    public GameObject ParryShieldMark;
    public GameObject ParriedEffectPrefab;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

    private bool InRollInvulnerability;

    private bool InParryInvulnerability;

    private bool InCriticalEye;

    private GameObject DamageText;


    private PowerSlash PowerSlashBattleArt;
    private int PowerSlashEnhancementCount;

    private HarmonySlash HarmonySlashBattleArt;
    private int HarmonySlashSlashEnhancementCount;

    private SpiritBolt SpiritBoltBattleArt;

    private SpiritFall SpiritFallBattleArt;

    private SpiritShadow SpiritShadowBattleArt;

    private SlashArt SlashArtPassiveAbility;

    private AssassinHeart AssassinHeartPassiveAbility;

    private SpellStrike SpellStrikePassiveAbility;

    private OneMind OneMindPassiveAbility;
    private int OneMindEnhancementCount;

    private Dancer DancerPassiveAbility;
    private CharacterAttackInfo DancerAttack;
    private List<GameObject> DancerHitEnemies = new List<GameObject>();

    private StepMaster StepMasterPassiveAbility;

    private Insanity InsanityPassiveAbility;

    // Start is called before the first frame update
    void Start()
    {

        Init();

        EventManager.instance.AddHandler<PlayerStartAttackAnticipation>(OnPlayerStartAttackAnticipation);
        EventManager.instance.AddHandler<PlayerEndAttackAnticipation>(OnPlayerEndAttackAnticipation);
        EventManager.instance.AddHandler<PlayerStartAttackStrike>(OnPlayerStartAttackStrike);
        EventManager.instance.AddHandler<PlayerEndAttackStrike>(OnPlayerEndAttackStrike);
        EventManager.instance.AddHandler<PlayerStartAttackRecovery>(OnPlayerStartAttackRecovery);
        EventManager.instance.AddHandler<PlayerEndAttackRecovery>(OnPlayerEndAttackRecovery);
        EventManager.instance.AddHandler<PlayerStartRoll>(OnPlayerStartRoll);
        EventManager.instance.AddHandler<PlayerEndRoll>(OnPlayerEndRoll);
        EventManager.instance.AddHandler<PlayerHitEnemy>(OnPlayerHitEnemy);
        EventManager.instance.AddHandler<PlayerBreakEnemyShield>(OnPlayerBreakEnemyShield);
        EventManager.instance.AddHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.AddHandler<PlayerGetHit>(OnPlayerGetHit);

        EventManager.instance.AddHandler<PlayerEquipBattleArt>(OnEquipBattleArt);
        EventManager.instance.AddHandler<PlayerEquipPassiveAbility>(OnEquipPassiveAbility);
        EventManager.instance.AddHandler<PlayerUnequipBattleArt>(OnUnequipBattleArt);
        EventManager.instance.AddHandler<PlayerUnequipPassiveAbility>(OnUnequipPassiveAbility);

    }

    private void OnDestroy()
    {
        EventManager.instance.RemoveHandler<PlayerStartAttackAnticipation>(OnPlayerStartAttackAnticipation);
        EventManager.instance.RemoveHandler<PlayerEndAttackAnticipation>(OnPlayerEndAttackAnticipation);
        EventManager.instance.RemoveHandler<PlayerStartAttackStrike>(OnPlayerStartAttackStrike);
        EventManager.instance.RemoveHandler<PlayerEndAttackStrike>(OnPlayerEndAttackStrike);
        EventManager.instance.RemoveHandler<PlayerStartAttackRecovery>(OnPlayerStartAttackRecovery);
        EventManager.instance.RemoveHandler<PlayerEndAttackRecovery>(OnPlayerEndAttackRecovery);
        EventManager.instance.RemoveHandler<PlayerStartRoll>(OnPlayerStartRoll);
        EventManager.instance.RemoveHandler<PlayerEndRoll>(OnPlayerEndRoll);
        EventManager.instance.RemoveHandler<PlayerHitEnemy>(OnPlayerHitEnemy);
        EventManager.instance.RemoveHandler<PlayerBreakEnemyShield>(OnPlayerBreakEnemyShield);
        EventManager.instance.RemoveHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.RemoveHandler<PlayerGetHit>(OnPlayerGetHit);

        EventManager.instance.RemoveHandler<PlayerEquipBattleArt>(OnEquipBattleArt);
        EventManager.instance.RemoveHandler<PlayerEquipPassiveAbility>(OnEquipPassiveAbility);
        EventManager.instance.RemoveHandler<PlayerUnequipBattleArt>(OnUnequipBattleArt);
        EventManager.instance.RemoveHandler<PlayerUnequipPassiveAbility>(OnUnequipPassiveAbility);

    }

    // Update is called once per frame
    void Update()
    {
        SetFill();

        var Action = GetComponent<CharacterAction>();

        if (DancerPassiveAbility != null && InRollInvulnerability)
        {
            CheckDancerHitEnemy();
        }

    }

    public void SetParryInvulnerability(bool value)
    {
        InParryInvulnerability = value;
        ParryShieldMark.GetComponent<SpriteRenderer>().enabled = value;
        ParryShieldMark.transform.position = GetComponent<SpeedManager>().GetTruePos();
    }

    public void SetRollInvulnerability(bool value)
    {
        InRollInvulnerability = value;
    }

    public bool GetCriticalEye()
    {
        return InCriticalEye;
    }

    private void SetCriticalEye (bool value)
    {
        InCriticalEye = value;
        CriticalEyeMark.GetComponent<SpriteRenderer>().enabled = value;
    }

    private void Init()
    {
        var Data = GetComponent<CharacterData>();
        CurrentHP = Data.MaxHP;
        CurrentEnergy = 0;

    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;

        EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / Data.MaxEnergy;

        AdvancedEnergyFill.GetComponent<Image>().fillAmount = (float)CurrentAdvancedEnergy / Data.MaxEnergy;

    }




    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        CurrentTakenAttack = (EnemyAttackInfo)Attack;


        bool IsInvulnerable = Invulnerable();

        EventManager.instance.Fire(new PlayerGetHit(CurrentTakenAttack,InRollInvulnerability));

        if (!IsInvulnerable)
        {
            var Data = GetComponent<CharacterData>();

            if(PowerSlashBattleArt != null)
            {
                PowerSlashLoseEnhancement();
            }

            if(HarmonySlashBattleArt != null)
            {
                HarmonySlashLoseSlashEnhancement();
            }

            if (OneMindPassiveAbility != null)
            {
                OneMindLoseEnhancement();
            }

            GainLoseAdvancedEnergy(-Data.HitAdvancedEnergyLost);
            GainLoseEnergy(-Data.HitEnergyLost);
            if(CurrentEnergy < CurrentAdvancedEnergy)
            {
                CurrentEnergy = CurrentAdvancedEnergy;
            }

            SetCriticalEye(false);

            DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

            int Damage = CurrentTakenAttack.Damage;

            Interrupted = true;


            if (CurrentTakenAttack.Right)
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 1);
            }
            else
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 1);
            }
            DamageText.GetComponent<Text>().text = Damage.ToString();
            DamageText.transform.parent = Canvas.transform;

            DamageText.GetComponent<Text>().color = Color.white;


            CurrentHP -= Damage;

            if (CurrentHP <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private bool Invulnerable()
    {
        return InRollInvulnerability || InParryInvulnerability;
    }

    private void OnPlayerStartAttackAnticipation(PlayerStartAttackAnticipation e)
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if(e.Attack.Type == CharacterAttackType.Slash)
        {
            if(OneMindPassiveAbility != null)
            {
                OneMindEnhanceSlash(e.Attack);
            }

            if (HarmonySlashBattleArt != null)
            {
                HarmonySlashEnhanceSlash(e.Attack);
            }
            if (SlashArtPassiveAbility != null)
            {
                SlashArtEffect(e.Attack);
            }
        }
        else if (e.Attack.ThisBattleArt != null)
        {
            CurrentEnergy = CurrentAdvancedEnergy;
            if (CurrentAdvancedEnergy < Data.MaxEnergy)
            {
                SetCriticalEye(false);
            }

            switch (e.Attack.ThisBattleArt.Type)
            {
                case BattleArtType.PowerSlash:
                    PowerSlashEnhanceDamage(e.Attack);
                    break;
                case BattleArtType.HarmonySlash:
                    HarmonySlashFullEnergyBonus(e.Attack);
                    break;
            }
        }

    }

    private void OnPlayerEndAttackAnticipation(PlayerEndAttackAnticipation e)
    {

    }

    private void OnPlayerStartAttackStrike(PlayerStartAttackStrike e)
    {

    }

    private void OnPlayerEndAttackStrike(PlayerEndAttackStrike e)
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();
        var Action = GetComponent<CharacterAction>();

        if (e.HitEnemies.Count > 0)
        {
            switch (e.Attack.DamType)
            {
                case DamageType.Normal:
                    break;
                case DamageType.Spell:
                    break;
                case DamageType.Strike:
                    PowerSlashIncreaseEnhancement();
                    break;
            }


            if (e.Attack.Type == CharacterAttackType.Slash)
            {
                GainLoseEnergy(AbilityData.SlashEnergyGain);
                if (InsanityPassiveAbility != null)
                {
                    InsanityGetAdvancedEnergy();
                }
            }
            else if(e.Attack.ThisBattleArt != null)
            {
                switch (Action.EquipedBattleArt.Type)
                {
                    case BattleArtType.PowerSlash:
                        GainLoseAdvancedEnergy(AbilityData.PowerSlashAdvancedEnergyGain);
                        break;
                    
                    case BattleArtType.HarmonySlash:
                        HarmonySlashIncreaseSlashEnhancement();
                        GainLoseAdvancedEnergy(AbilityData.HarmonySlashAdvancedEnergyGain);
                        break;
                }

                CurrentEnergy = CurrentAdvancedEnergy;
            }
        }
    }

    private void OnPlayerStartAttackRecovery(PlayerStartAttackRecovery e)
    {

    }

    private void OnPlayerEndAttackRecovery(PlayerEndAttackRecovery e)
    {

    }

    private void OnPlayerStartRoll(PlayerStartRoll e)
    {
        InRollInvulnerability = true;
        if(DancerPassiveAbility != null)
        {
            SetUpDancer();
        }
    }

    private void OnPlayerEndRoll(PlayerEndRoll e)
    {
        InRollInvulnerability = false;
    }


    private void OnPlayerGetHit(PlayerGetHit e)
    {
        var Action = GetComponent<CharacterAction>();
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if (InParryInvulnerability)
        {
            GameObject.Instantiate(ParriedEffectPrefab, ParryShieldMark.transform.position, Quaternion.Euler(0, 0, 0));
            GainLoseEnergy(AbilityData.ParryEnergyGain);
        }


    }

    private void OnPlayerKillEnemy(PlayerKillEnemy e)
    {

    }

    private void OnPlayerHitEnemy(PlayerHitEnemy e)
    {
        var Action = GetComponent<CharacterAction>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if (OneMindPassiveAbility != null)
        {
            OneMindIncreaseEnhancement();
        }

        if (e.OriginalAttack.ThisBattleArt != null)
        {
            switch (Action.EquipedBattleArt.Type)
            {
                case BattleArtType.PowerSlash:
                    PowerSlashExecutionBonus(e.UpdatedAttack,e.Enemy);
                    break;

                case BattleArtType.HarmonySlash:

                    break;
            }
        }
    }

    private void OnPlayerBreakEnemyShield(PlayerBreakEnemyShield e)
    {

    }


    protected void Heal(int amount)
    {
        var Data = GetComponent<CharacterData>();
        if (amount > Data.MaxHP - CurrentHP)
        {
            amount = Data.MaxHP - CurrentHP;
        }

        if (amount > 0)
        {
            GameObject DamageText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/DamageText"), transform.position, Quaternion.Euler(0, 0, 0));
            DamageText.GetComponent<DamageText>().TravelVector = Vector2.up;
            DamageText.GetComponent<Text>().color = Color.green;
            DamageText.transform.parent = Canvas.transform;
            DamageText.GetComponent<Text>().text = amount.ToString();
            CurrentHP += amount;
        }
    }

    private void GainLoseEnergy(int amount)
    {
        var Data = GetComponent<CharacterData>();
        if(amount >= Data.MaxEnergy - CurrentEnergy)
        {
            CurrentEnergy = Data.MaxEnergy;
            SetCriticalEye(true);
            return;
        }
        else if(amount <= -CurrentEnergy)
        {
            CurrentEnergy = 0;
            return;
        }

        CurrentEnergy += amount;
    }

    private void GainLoseAdvancedEnergy(int amount)
    {
        var Data = GetComponent<CharacterData>();

        if (amount >= Data.MaxEnergy - CurrentAdvancedEnergy)
        {
            CurrentAdvancedEnergy = Data.MaxEnergy;
            SetCriticalEye(true);
            return;
        }
        else if(amount <= -CurrentAdvancedEnergy)
        {
            CurrentAdvancedEnergy = 0;
            return;
        }

        CurrentAdvancedEnergy += amount;
    }

    private void OnEquipBattleArt(PlayerEquipBattleArt e)
    {
        var Action = GetComponent<CharacterAction>();

        switch (e.ThisBattleArt.Type)
        {
            case BattleArtType.HarmonySlash:
                HarmonySlashBattleArt = (HarmonySlash)e.ThisBattleArt;
                break;
            case BattleArtType.PowerSlash:
                PowerSlashBattleArt = (PowerSlash)e.ThisBattleArt;
                break;
            case BattleArtType.SpiritBolt:
                SpiritBoltBattleArt = (SpiritBolt)e.ThisBattleArt;
                break;
            case BattleArtType.SpiritFall:
                SpiritFallBattleArt = (SpiritFall)e.ThisBattleArt;
                break;
            case BattleArtType.SpiritShadow:
                SpiritShadowBattleArt = (SpiritShadow)e.ThisBattleArt;
                break;
        }

        Action.EquipedBattleArt = e.ThisBattleArt;
    }

    private void OnUnequipBattleArt(PlayerUnequipBattleArt e)
    {
        var Action = GetComponent<CharacterAction>();
        switch (e.ThisBattleArt.Type)
        {
            case BattleArtType.HarmonySlash:
                HarmonySlashLoseSlashEnhancement();
                HarmonySlashBattleArt = null;
                break;
            case BattleArtType.PowerSlash:
                PowerSlashLoseEnhancement();
                PowerSlashBattleArt = null;
                break;
            case BattleArtType.SpiritBolt:
                SpiritBoltBattleArt = null;
                break;
            case BattleArtType.SpiritFall:
                SpiritFallBattleArt = null;
                break;
            case BattleArtType.SpiritShadow:
                SpiritShadowBattleArt = null;
                break;
        }

        Action.EquipedBattleArt = null;
    }

    private void OnEquipPassiveAbility(PlayerEquipPassiveAbility e)
    {
        var Action = GetComponent<CharacterAction>();

        switch (e.ThisPassiveAbility.Type)
        {
            case PassiveAbilityType.SlashArt:
                SlashArtPassiveAbility = (SlashArt)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.AssassinHeart:
                AssassinHeartPassiveAbility = (AssassinHeart)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.SpellStrike:
                SpellStrikePassiveAbility = (SpellStrike)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.OneMind:
                OneMindPassiveAbility = (OneMind)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.Dancer:
                DancerPassiveAbility = (Dancer)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.StepMaster:
                StepMasterPassiveAbility = (StepMaster)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.Insanity:
                InsanityPassiveAbility = (Insanity)e.ThisPassiveAbility;
                break;
        }

        Action.EquipedPassiveAbility[e.Index] = e.ThisPassiveAbility;
    }

    private void OnUnequipPassiveAbility(PlayerUnequipPassiveAbility e)
    {
        var Action = GetComponent<CharacterAction>();

        switch (e.ThisPassiveAbility.Type)
        {
            case PassiveAbilityType.SlashArt:
                SlashArtPassiveAbility = null;
                break;
            case PassiveAbilityType.AssassinHeart:
                AssassinHeartPassiveAbility = null;
                break;
            case PassiveAbilityType.SpellStrike:
                SpellStrikePassiveAbility = null;
                break;
            case PassiveAbilityType.OneMind:
                OneMindLoseEnhancement();
                OneMindPassiveAbility = null;
                break;
            case PassiveAbilityType.Dancer:
                DancerPassiveAbility = null;
                break;
            case PassiveAbilityType.StepMaster:
                StepMasterPassiveAbility = null;
                break;
            case PassiveAbilityType.Insanity:
                InsanityPassiveAbility = null;
                break;
        }

        Action.EquipedPassiveAbility[e.Index] = null;
    }

    //Power Slash

    private void PowerSlashIncreaseEnhancement()
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        if (PowerSlashBattleArt.Level >= 2)
        {
            PowerSlashEnhancementCount++;
            if(PowerSlashEnhancementCount > AbilityData.PowerSlashMaxEnhancementTime)
            {
                PowerSlashEnhancementCount = AbilityData.PowerSlashMaxEnhancementTime;
            }
        }
    }

    private void PowerSlashEnhanceDamage(CharacterAttackInfo Attack)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Damage += Mathf.RoundToInt(AbilityData.PowerSlashDamageEnhancementFactor * PowerSlashEnhancementCount*AbilityData.BaseDamage);
    }

    private void PowerSlashExecutionBonus(CharacterAttackInfo Attack, GameObject Enemy)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();
        float Proportion = Enemy.GetComponent<IHittable>().CurrentHP / (float)Enemy.GetComponent<IHittable>().MaxHP;

        if(Proportion <= AbilityData.PowerSlashExecutionHPCondition && PowerSlashEnhancementCount == AbilityData.PowerSlashMaxEnhancementTime)
        {
            Attack.Damage += Mathf.RoundToInt(AbilityData.PowerSlashExecutionDamageFactor * AbilityData.BaseDamage);
        }
    }

    private void PowerSlashLoseEnhancement()
    {
        PowerSlashEnhancementCount = 0;
    }

    //Harmony Slash

    private void HarmonySlashFullEnergyBonus(CharacterAttackInfo Attack)
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if(HarmonySlashBattleArt.Level >= 2 && CurrentAdvancedEnergy == Data.MaxEnergy)
        {
            Attack.Damage += Mathf.RoundToInt(AbilityData.BaseDamage * AbilityData.HarmonySlashFullAdvancedEnergyDamageBonus);
        }
    }

    private void HarmonySlashIncreaseSlashEnhancement()
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if(HarmonySlashBattleArt.Level >= 3 && CurrentAdvancedEnergy == Data.MaxEnergy)
        {
            HarmonySlashSlashEnhancementCount++;
            if(HarmonySlashSlashEnhancementCount > AbilityData.HarmonySlashMaxSlashDamageBonusTime)
            {
                HarmonySlashSlashEnhancementCount = AbilityData.HarmonySlashMaxSlashDamageBonusTime;
            }
        }
    }

    private void HarmonySlashEnhanceSlash(CharacterAttackInfo Attack)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Damage += Mathf.RoundToInt(HarmonySlashSlashEnhancementCount * AbilityData.HarmonySlashSlashDamageBonusFactor*AbilityData.BaseDamage);
    }

    private void HarmonySlashLoseSlashEnhancement()
    {
        HarmonySlashSlashEnhancementCount = 0;
    }

    //Slash Art

    private void SlashArtEffect(CharacterAttackInfo Attack)
    {
        if (CurrentEnergy == GetComponent<CharacterData>().MaxEnergy)
        {
            Attack.DamType = DamageType.Strike;
        }
    }

    //Assassin's Heart

    private void AssassinHeartEnhancement(CharacterAttackInfo Attack)
    {

    }

    //One Mind

    private void OneMindIncreaseEnhancement()
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        OneMindEnhancementCount++;
        if(OneMindEnhancementCount > AbilityData.OneMindMaxDamageEnhancementTime)
        {
            OneMindEnhancementCount = AbilityData.OneMindMaxDamageEnhancementTime;
        }
    }

    private void OneMindEnhanceSlash(CharacterAttackInfo Attack)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Damage += Mathf.RoundToInt(AbilityData.OneMindDamageEnhancementFactor * OneMindEnhancementCount * AbilityData.BaseDamage);
    }

    private void OneMindLoseEnhancement()
    {
        OneMindEnhancementCount = 0;
    }

    //Dancer

    private void CheckDancerHitEnemy()
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();
        var SpeedManager = GetComponent<SpeedManager>();

        Vector2 Dir = Vector2.right;
        if(DancerAttack.Dir == Direction.Left)
        {
            Dir = Vector2.left;
        }


        RaycastHit2D[] AllHits = Physics2D.BoxCastAll(SpeedManager.GetTruePos(), AbilityData.DancerHitBoxSize, 0, Dir, 0, Data.EnemyLayer);
        if (AllHits.Length > 0)
        {
            for (int i = 0; i < AllHits.Length; i++)
            {
                GameObject Enemy = AllHits[i].collider.gameObject;

                if (!DancerHitEnemies.Contains(Enemy))
                {
                    CharacterAttackInfo UpdatedAttack = new CharacterAttackInfo(DancerAttack);

                    EventManager.instance.Fire(new PlayerHitEnemy(DancerAttack, UpdatedAttack, Enemy));

                    Enemy.GetComponent<IHittable>().OnHit(UpdatedAttack);
                    DancerHitEnemies.Add(Enemy);
                }
            }
        }
    }

    private void SetUpDancer()
    {
        var AbilityData = GetComponent<CharacterAbilityData>();
        var SpeedManager = GetComponent<SpeedManager>();

        DancerHitEnemies.Clear();

        int DancerDamage = Mathf.RoundToInt(AbilityData.BaseDamage * AbilityData.DancerDamageFactor);

        Direction dir;
        if (transform.right.x > 0)
        {
            dir = Direction.Right;
        }
        else
        {
            dir = Direction.Left;
        }

        DancerAttack = new CharacterAttackInfo(gameObject, CharacterAttackType.Dancer, DamageType.Normal, dir, DancerDamage, DancerDamage, DancerDamage, SpeedManager.OriPos, SpeedManager.OriPos, AbilityData.DancerHitBoxSize, AbilityData.DancerHitBoxSize);
    }

    //Insanity

    private void InsanityGetAdvancedEnergy()
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        GainLoseAdvancedEnergy(AbilityData.InsanityAdvancedEnergyGain);
        if(CurrentEnergy < CurrentAdvancedEnergy)
        {
            CurrentEnergy = CurrentAdvancedEnergy;
        }
    }

    private void InsanityEnergyLost()
    {
        CurrentAdvancedEnergy = 0;
        CurrentEnergy = 0;
    }
}
