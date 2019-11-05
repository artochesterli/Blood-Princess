using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;

    public GameObject Canvas;
    public GameObject HPFill;
    public GameObject EnergyMarks;
    public GameObject CriticalEyeMark;
    public GameObject ShieldBreakerShield;
    public GameObject DancerInvulnerableMark;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

    private BattleArtEnhancement CriticalEyeEnhancement;
    private bool HaveCriticalEyeBuff;
    private int CurrentCriticalEyeBonusNumber;
    private int MaxCriticalEyeBonusNumber;

    private BattleArtEnhancement HarmonyEnhancement;
    private bool HarmonyAvailable;
    private float HarmonyHealUnit;

    private BattleArtEnhancement ShieldBreakerEnhancement;
    private bool InShieldBreakerShield;
    private bool ShieldBreakerAbsorbAttack;
    private int ShieldBreakerBonus;
    private float ShieldBreakerShieldTimeCount;

    private CharacterPassiveAbility DancerPassiveAbility;
    private bool InDancerInvulnerability;
    private int DancerShieldBreak;
    private List<GameObject> DancerHitEnemies;

    private CharacterPassiveAbility ExecutionerAbility;
    private bool ExecutionerShortenAnticipationActivated;

    private CharacterPassiveAbility AccurateBladeAbility;

    private CharacterPassiveAbility CursedBladeAbility;


    private GameObject DamageText;

    private float TimeCount;

    private bool GetHitAvailable;

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
        EventManager.instance.AddHandler<PlayerStartRollAnticipation>(OnPlayerStartRollAnticipation);
        EventManager.instance.AddHandler<PlayerEndRollAnticipation>(OnPlayerEndRollAnticipation);
        EventManager.instance.AddHandler<PlayerStartRoll>(OnPlayerStartRoll);
        EventManager.instance.AddHandler<PlayerEndRoll>(OnPlayerEndRoll);
        EventManager.instance.AddHandler<PlayerHitEnemy>(OnPlayerHitEnemy);
        EventManager.instance.AddHandler<PlayerBreakEnemyShield>(OnPlayerBreakEnemyShield);
        EventManager.instance.AddHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.AddHandler<PlayerGetHit>(OnPlayerGetHit);

        EventManager.instance.AddHandler<PlayerEquipEnhancement>(OnPlayerEquipEnhancement);
        EventManager.instance.AddHandler<PlayerUnequipEnhancement>(OnPlayerUnequipEnhancement);
        EventManager.instance.AddHandler<PlayerEquipPassiveAbility>(OnPlayerEquipPassiveAbility);
        EventManager.instance.AddHandler<PlayerUnequipPassiveAbility>(OnPlayerUnequipPassiveAbility);
        EventManager.instance.AddHandler<PlayerUpgradeEnhancement>(OnPlayerUpgradeEnhancement);
        EventManager.instance.AddHandler<PlayerDowngradeEnhancement>(OnPlayerDowngradeEnhancement);
        EventManager.instance.AddHandler<PlayerUpgradePassiveAbility>(OnPlayerUpgradePassiveAbility);
        EventManager.instance.AddHandler<PlayerDowngradePassiveAbility>(OnPlayerDowngradePassiveAbility);
    }

    private void OnDestroy()
    {
        EventManager.instance.RemoveHandler<PlayerStartAttackAnticipation>(OnPlayerStartAttackAnticipation);
        EventManager.instance.RemoveHandler<PlayerEndAttackAnticipation>(OnPlayerEndAttackAnticipation);
        EventManager.instance.RemoveHandler<PlayerStartAttackStrike>(OnPlayerStartAttackStrike);
        EventManager.instance.RemoveHandler<PlayerEndAttackStrike>(OnPlayerEndAttackStrike);
        EventManager.instance.RemoveHandler<PlayerStartAttackRecovery>(OnPlayerStartAttackRecovery);
        EventManager.instance.RemoveHandler<PlayerEndAttackRecovery>(OnPlayerEndAttackRecovery);
        EventManager.instance.RemoveHandler<PlayerStartRollAnticipation>(OnPlayerStartRollAnticipation);
        EventManager.instance.RemoveHandler<PlayerEndRollAnticipation>(OnPlayerEndRollAnticipation);
        EventManager.instance.RemoveHandler<PlayerStartRoll>(OnPlayerStartRoll);
        EventManager.instance.RemoveHandler<PlayerEndRoll>(OnPlayerEndRoll);
        EventManager.instance.RemoveHandler<PlayerHitEnemy>(OnPlayerHitEnemy);
        EventManager.instance.RemoveHandler<PlayerBreakEnemyShield>(OnPlayerBreakEnemyShield);
        EventManager.instance.RemoveHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.RemoveHandler<PlayerGetHit>(OnPlayerGetHit);

        EventManager.instance.RemoveHandler<PlayerEquipEnhancement>(OnPlayerEquipEnhancement);
        EventManager.instance.RemoveHandler<PlayerUnequipEnhancement>(OnPlayerUnequipEnhancement);
        EventManager.instance.RemoveHandler<PlayerEquipPassiveAbility>(OnPlayerEquipPassiveAbility);
        EventManager.instance.RemoveHandler<PlayerUnequipPassiveAbility>(OnPlayerUnequipPassiveAbility);
        EventManager.instance.RemoveHandler<PlayerUpgradeEnhancement>(OnPlayerUpgradeEnhancement);
        EventManager.instance.RemoveHandler<PlayerDowngradeEnhancement>(OnPlayerDowngradeEnhancement);
        EventManager.instance.RemoveHandler<PlayerUpgradePassiveAbility>(OnPlayerUpgradePassiveAbility);
        EventManager.instance.RemoveHandler<PlayerDowngradePassiveAbility>(OnPlayerDowngradePassiveAbility);
    }

    // Update is called once per frame
    void Update()
    {
        SetFill();

        var Action = GetComponent<CharacterAction>();

        CountShieldTime();

        DancerHitEnemy();
    }

    private void Init()
    {
        var Data = GetComponent<CharacterData>();
        CurrentHP = Data.MaxHP;
        CurrentEnergy = 0;

        CriticalEyeEnhancement = GetNullBattleArtEnhancement();
        HarmonyEnhancement = GetNullBattleArtEnhancement();
        ShieldBreakerEnhancement = GetNullBattleArtEnhancement();

        DancerPassiveAbility = GetNullPassiveAbility();
        DancerHitEnemies = new List<GameObject>();
        ExecutionerAbility = GetNullPassiveAbility();
        AccurateBladeAbility = GetNullPassiveAbility();
        CursedBladeAbility = GetNullPassiveAbility();
    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;

        int count = 0;

        foreach(Transform child in EnergyMarks.transform)
        {
            if (CurrentEnergy > count)
            {
                child.GetComponent<Image>().enabled = true;
            }
            else
            {
                child.GetComponent<Image>().enabled = false;
            }
            count++;
        }

        //EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / Data.MaxEnergy;
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        EnemyAttackInfo HitAttack = (EnemyAttackInfo)Attack;

        GetHitAvailable = true;

        EventManager.instance.Fire(new PlayerGetHit(HitAttack));

        if (GetHitAvailable)
        {
            DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));


            int Damage = HitAttack.Damage;

            CurrentEnergy = 0;

            Interrupted = true;


            if (HitAttack.Right)
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 1);
            }
            else
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 1);
            }
            DamageText.GetComponent<Text>().text = Damage.ToString();
            DamageText.transform.parent = Canvas.transform;


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

    private void OnPlayerStartAttackAnticipation(PlayerStartAttackAnticipation e)
    {
        CurrentEnergy -= e.Attack.Cost;

        CursedBladeDecreaseBattleArtBaseDamage(e.Attack);
        CursedBladeIncreaseNormalSlashBaseDamage(e.Attack);

        CheckHarmonyAvailable(e.Attack);

        ActivateShieldBreakerShield(e.Attack);

        ExecutionerShortenAnticipation(e.Attack);


    }

    private void OnPlayerEndAttackAnticipation(PlayerEndAttackAnticipation e)
    {

    }

    private void OnPlayerStartAttackStrike(PlayerStartAttackStrike e)
    {
        CriticalEyeBonusToAttack(e.Attack);

    }

    private void OnPlayerEndAttackStrike(PlayerEndAttackStrike e)
    {
        LoseCriticalEye(e.Attack, e.HitEnemies.Count);
        AddCriticalEyeBuff(e.Attack,e.HitEnemies.Count);
        ImproveCriticalEyeBuff(e.Attack, e.HitEnemies.Count);


        HarmonyHeal(e.Attack,e.HitEnemies.Count);

        AccurateBladeHeal(e.Attack,e.HitEnemies.Count);

    }

    private void OnPlayerStartAttackRecovery(PlayerStartAttackRecovery e)
    {

    }

    private void OnPlayerEndAttackRecovery(PlayerEndAttackRecovery e)
    {

    }

    private void OnPlayerStartRollAnticipation(PlayerStartRollAnticipation e)
    {

    }

    private void OnPlayerEndRollAnticipation(PlayerEndRollAnticipation e)
    {

    }

    private void OnPlayerStartRoll(PlayerStartRoll e)
    {
        ActivateDancerInvulnerability();
    }

    private void OnPlayerEndRoll(PlayerEndRoll e)
    {
        DeactivateDancerInvulnerability();
    }


    private void OnPlayerGetHit(PlayerGetHit e)
    {
        var Action = GetComponent<CharacterAction>();

        ShieldBreakerGetHit(Action.CurrentAttack);

        DancerGetHit();

        if (GetHitAvailable)
        {
            LoseCriticalEye();
            CursedBladeIncreaseRecievedDamage(e.EnemyAttack);
        }
    }

    private void OnPlayerKillEnemy(PlayerKillEnemy e)
    {

    }

    private void OnPlayerHitEnemy(PlayerHitEnemy e)
    {
        var Data = GetComponent<CharacterData>();

        if (e.UpdatedAttack.Type == CharacterAttackType.NormalSlash)
        {
            GainEnergy(1);
        }

        AccurateBladeBonusDamage(e.UpdatedAttack, e.Enemy);

    }

    private void OnPlayerBreakEnemyShield(PlayerBreakEnemyShield e)
    {
        ShieldBreakerDamageBonus(e.Attack);

        ActivateExecutionerShorten();
        ExecutionerRecoverEnergy();
    }

    private void OnPlayerEquipEnhancement(PlayerEquipEnhancement e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.Enhancement.Type)
        {
            case BattleArtEnhancementType.CriticleEye:
                CriticalEyeEnhancement = e.Enhancement;
                if(CriticalEyeEnhancement.Level >= 2)
                {
                    MaxCriticalEyeBonusNumber = AbilityData.CriticalEyeMaxBonusNumberLv2;
                }
                else
                {
                    MaxCriticalEyeBonusNumber = AbilityData.CriticalEyeMaxBonusNumberLv1;
                }
                break;

            case BattleArtEnhancementType.Harmony:
                HarmonyEnhancement = e.Enhancement;
                if(HarmonyEnhancement.Level >= 2)
                {
                    HarmonyHealUnit = AbilityData.HarmonyHealLv2;
                }
                else
                {
                    HarmonyHealUnit = AbilityData.HarmonyHealLv1;
                }
                break;
            case BattleArtEnhancementType.ShieldBreaker:
                ShieldBreakerEnhancement = e.Enhancement;
                if(ShieldBreakerEnhancement.Level >= 2)
                {
                    ShieldBreakerBonus = AbilityData.ShieldBreakerBonusLv2;
                }
                else
                {
                    ShieldBreakerBonus = AbilityData.ShieldBreakerBonusLv1;
                }
                break;
        }
    }

    private void OnPlayerUnequipEnhancement(PlayerUnequipEnhancement e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.Enhancement.Type)
        {
            case BattleArtEnhancementType.CriticleEye:
                CriticalEyeEnhancement = GetNullBattleArtEnhancement();
                LoseCriticalEye();
                break;

            case BattleArtEnhancementType.Harmony:
                HarmonyEnhancement = GetNullBattleArtEnhancement();
                HarmonyAvailable = false;
                break;
            case BattleArtEnhancementType.ShieldBreaker:
                ShieldBreakerEnhancement = GetNullBattleArtEnhancement();
                DeactivateShieldBreakerShield();
                break;
        }
    }

    private void OnPlayerUpgradeEnhancement(PlayerUpgradeEnhancement e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.Enhancement.Type)
        {
            case BattleArtEnhancementType.CriticleEye:
                if(CriticalEyeEnhancement.Level >= 2)
                {
                    MaxCriticalEyeBonusNumber = AbilityData.CriticalEyeMaxBonusNumberLv2;
                }
                break;

            case BattleArtEnhancementType.Harmony:
                if (HarmonyEnhancement.Level >= 2)
                {
                    HarmonyHealUnit = AbilityData.HarmonyHealLv2;
                }
                break;
            case BattleArtEnhancementType.ShieldBreaker:
                if(ShieldBreakerEnhancement.Level >= 2)
                {
                    ShieldBreakerBonus = AbilityData.ShieldBreakerBonusLv2;
                }
                break;
        }
    }


    private void OnPlayerDowngradeEnhancement(PlayerDowngradeEnhancement e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.Enhancement.Type)
        {
            case BattleArtEnhancementType.CriticleEye:
                if (CriticalEyeEnhancement.Level < 2)
                {
                    MaxCriticalEyeBonusNumber = AbilityData.CriticalEyeMaxBonusNumberLv1;
                    if(CurrentCriticalEyeBonusNumber > MaxCriticalEyeBonusNumber)
                    {
                        CurrentCriticalEyeBonusNumber = MaxCriticalEyeBonusNumber;
                    }
                }
                break;

            case BattleArtEnhancementType.Harmony:
                if (HarmonyEnhancement.Level < 2)
                {
                    HarmonyHealUnit = AbilityData.HarmonyHealLv1;
                }
                break;
            case BattleArtEnhancementType.ShieldBreaker:
                if(ShieldBreakerEnhancement.Level < 2)
                {
                    ShieldBreakerBonus = AbilityData.ShieldBreakerBonusLv1;
                }
                break;
        }
    }

    private void OnPlayerEquipPassiveAbility(PlayerEquipPassiveAbility e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.PassiveAbility.Type)
        {
            case CharacterPassiveAbilityType.Dancer:
                DancerPassiveAbility = e.PassiveAbility;
                if(DancerPassiveAbility.Level == 3)
                {
                    DancerShieldBreak = AbilityData.DancerShieldBreak;
                }
                else
                {
                    DancerShieldBreak = 0;
                }
                break;
            case CharacterPassiveAbilityType.Executioner:
                ExecutionerAbility = e.PassiveAbility;
                break;
            case CharacterPassiveAbilityType.AccurateBlade:
                AccurateBladeAbility = e.PassiveAbility;
                break;
        }
    }

    private void OnPlayerUnequipPassiveAbility(PlayerUnequipPassiveAbility e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.PassiveAbility.Type)
        {
            case CharacterPassiveAbilityType.Dancer:
                DancerPassiveAbility = GetNullPassiveAbility();
                DeactivateDancerInvulnerability();
                break;
            case CharacterPassiveAbilityType.Executioner:
                ExecutionerAbility = GetNullPassiveAbility();
                DeactivateExecutionerShorten();
                break;
            case CharacterPassiveAbilityType.AccurateBlade:
                AccurateBladeAbility = GetNullPassiveAbility();
                break;
        }
    }

    private void OnPlayerUpgradePassiveAbility(PlayerUpgradePassiveAbility e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.PassiveAbility.Type)
        {
            case CharacterPassiveAbilityType.Dancer:
                if (DancerPassiveAbility.Level == 3)
                {
                    DancerShieldBreak = AbilityData.DancerShieldBreak;
                }
                break;
            case CharacterPassiveAbilityType.Executioner:
                break;
            case CharacterPassiveAbilityType.AccurateBlade:
                break;
        }
    }

    private void OnPlayerDowngradePassiveAbility(PlayerDowngradePassiveAbility e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();

        switch (e.PassiveAbility.Type)
        {
            case CharacterPassiveAbilityType.Dancer:
                if (DancerPassiveAbility.Level < 3)
                {
                    DancerShieldBreak = 0;
                }
                break;
            case CharacterPassiveAbilityType.Executioner:
                break;
            case CharacterPassiveAbilityType.AccurateBlade:
                break;
        }
    }

    private BattleArtEnhancement GetNullBattleArtEnhancement()
    {
        return new BattleArtEnhancement("", CharacterAttackType.Null, BattleArtEnhancementType.Null, 0);
    }

    private CharacterPassiveAbility GetNullPassiveAbility()
    {
        return new CharacterPassiveAbility("", CharacterPassiveAbilityType.Null, 0);
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

    private void GainEnergy(int amount)
    {
        var Data = GetComponent<CharacterData>();
        if(amount > Data.MaxEnergy - CurrentEnergy)
        {
            amount = Data.MaxEnergy - CurrentEnergy;
        }

        CurrentEnergy += amount;
    }

    /*private BattleArtEnhancement GetTargetEnhancement(BattleArtEnhancementType Type, CharacterAttackType ArtType)
    {
        var Action = GetComponent<CharacterAction>();

        if(ArtType == CharacterAttackType.BloodSlash)
        {
            for(int i=0;i< Action.BloodSlashEnhancements.Count; i++)
            {
                if(Action.BloodSlashEnhancements[i].Type == Type)
                {
                    return Action.BloodSlashEnhancements[i];
                }
            } 
        }
        else
        {
            for (int i = 0; i < Action.DeadSlashEnhancements.Count; i++)
            {
                if(Action.DeadSlashEnhancements[i].Type == Type)
                {
                    return Action.DeadSlashEnhancements[i];
                }
            }
        }

        return new BattleArtEnhancement("", CharacterAttackType.Null, BattleArtEnhancementType.Null, 0);
    }

    private CharacterPassiveAbility GetTargetPassiveAbility(CharacterPassiveAbilityType Type)
    {
        var Action = GetComponent<CharacterAction>();

        for(int i = 0; i < Action.EquipedPassiveAbilities.Count; i++)
        {
            if (Action.EquipedPassiveAbilities[i].Type == Type)
            {
                return Action.EquipedPassiveAbilities[i];
            }
        }

        return new CharacterPassiveAbility("", CharacterPassiveAbilityType.Null, 0);
    }*/





    //CriticalEyeFunctions



    private void AddCriticalEyeBuff(CharacterAttackInfo Attack, int HitNumber)
    {

        if (CriticalEyeEnhancement.Type == BattleArtEnhancementType.Null || CriticalEyeEnhancement.EnhancementAttackType != Attack.Type)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (HitNumber > 0 && !HaveCriticalEyeBuff)
        {
            CriticalEyeMark.GetComponent<SpriteRenderer>().enabled = true;
            HaveCriticalEyeBuff = true;
            CurrentCriticalEyeBonusNumber = 0;

            if (CriticalEyeEnhancement.Level >= 2)
            {
                MaxCriticalEyeBonusNumber = AbilityData.CriticalEyeMaxBonusNumberLv2;
            }
            else
            {
                MaxCriticalEyeBonusNumber = AbilityData.CriticalEyeMaxBonusNumberLv1;
            }

        }
    }

    private void CriticalEyeBonusToAttack(CharacterAttackInfo Attack)
    {
        if (CriticalEyeEnhancement.Type == BattleArtEnhancementType.Null || Attack.Type == CharacterAttackType.NormalSlash)
        {
            return;
        }

        if(Attack.Type != CriticalEyeEnhancement.EnhancementAttackType &&(CriticalEyeEnhancement.Level < 3 || CurrentCriticalEyeBonusNumber < MaxCriticalEyeBonusNumber))
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Damage += Mathf.RoundToInt(Attack.BaseDamage * AbilityData.CriticalEyeBonusUnit * CurrentCriticalEyeBonusNumber);
    }

    private void ImproveCriticalEyeBuff(CharacterAttackInfo Attack, int HitNumber)
    {
        if (CriticalEyeEnhancement.Type == BattleArtEnhancementType.Null || CriticalEyeEnhancement.EnhancementAttackType != Attack.Type)
        {
            return;
        }

        if (HaveCriticalEyeBuff)
        {
            CurrentCriticalEyeBonusNumber += HitNumber;
            if (CurrentCriticalEyeBonusNumber > MaxCriticalEyeBonusNumber)
            {
                CurrentCriticalEyeBonusNumber = MaxCriticalEyeBonusNumber;
            }
        }
    }

    private void LoseCriticalEye(CharacterAttackInfo Attack, int HitNumber)
    {
        if (CriticalEyeEnhancement.Type == BattleArtEnhancementType.Null || CriticalEyeEnhancement.EnhancementAttackType != Attack.Type)
        {
            return;
        }

        if (HitNumber == 0)
        {
            CriticalEyeMark.GetComponent<SpriteRenderer>().enabled = false;
            HaveCriticalEyeBuff = false;
            CurrentCriticalEyeBonusNumber = 0;
        }
    }

    private void LoseCriticalEye()
    {

        CriticalEyeMark.GetComponent<SpriteRenderer>().enabled = false;
        HaveCriticalEyeBuff = false;
        CurrentCriticalEyeBonusNumber = 0;
    }

    //HarmonyFunctions

    private void CheckHarmonyAvailable(CharacterAttackInfo Attack)
    {
        if(HarmonyEnhancement.Type != BattleArtEnhancementType.Null && HarmonyEnhancement.EnhancementAttackType == Attack.Type && CurrentEnergy == 0)
        {
            HarmonyAvailable = true;
        }
        else
        {
            HarmonyAvailable = false;
        }
    }

    private void HarmonyHeal(CharacterAttackInfo Attack, int HitNumber)
    {
        if (HarmonyAvailable)
        {
            HarmonyAvailable = false;
        }
        else
        {
            return;
        }

        if(HarmonyEnhancement.Type == BattleArtEnhancementType.Null || HarmonyEnhancement.EnhancementAttackType != Attack.Type || HitNumber==0)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        int HealNumber = Mathf.RoundToInt(Attack.Damage * HitNumber * HarmonyHealUnit);


        if (CurrentHP == Data.MaxHP && HarmonyEnhancement.Level == 3)
        {
            GainEnergy(AbilityData.HarmonyEnergyRecovery);
        }
        else
        {
            Heal(HealNumber);
        }
    }

    //ShieldBreaker Functions

    private void CountShieldTime()
    {
        if (!InShieldBreakerShield)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        ShieldBreakerShieldTimeCount += Time.deltaTime;
        if (ShieldBreakerShieldTimeCount >= AbilityData.ShieldBreakerShieldTime)
        {
            ShieldBreakerAbsorbAttack = false;
            DeactivateShieldBreakerShield();
        }
    }

    private void ActivateShieldBreakerShield(CharacterAttackInfo Attack)
    {
        if(ShieldBreakerEnhancement.Type==BattleArtEnhancementType.Null || ShieldBreakerEnhancement.EnhancementAttackType != Attack.Type)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        ShieldBreakerAbsorbAttack = false;
        InShieldBreakerShield = true;
        ShieldBreakerShieldTimeCount = 0;
        ShieldBreakerShield.GetComponent<SpriteRenderer>().enabled = true;
        ShieldBreakerShield.transform.position = GetComponent<SpeedManager>().GetTruePos();

        Attack.AnticipationTime += AbilityData.ShieldBreakerShieldTime;
    }

    private void DeactivateShieldBreakerShield()
    {

        InShieldBreakerShield = false;
        ShieldBreakerShield.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void ShieldBreakerGetHit(CharacterAttackInfo Attack)
    {
        if (ShieldBreakerEnhancement.Type == BattleArtEnhancementType.Null || ShieldBreakerEnhancement.EnhancementAttackType != Attack.Type || !InShieldBreakerShield)
        {
            return;
        }

        ShieldBreakerAbsorbAttack = true;
        GetHitAvailable = false;

        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.AnticipationTime = 0;

        Attack.ShieldBreak += ShieldBreakerBonus;

        DeactivateShieldBreakerShield();
    }

    private void ShieldBreakerDamageBonus(CharacterAttackInfo Attack)
    {
        if (ShieldBreakerEnhancement.Type == BattleArtEnhancementType.Null || ShieldBreakerEnhancement.EnhancementAttackType != Attack.Type || !ShieldBreakerAbsorbAttack || ShieldBreakerEnhancement.Level <3)
        {
            return;
        }


        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Damage += Mathf.RoundToInt(Attack.BaseDamage * AbilityData.ShieldBreakerDamageBonus);
    }

    //DancerFunctions

    private void ActivateDancerInvulnerability()
    {
        var Data = GetComponent<CharacterData>();

        if(DancerPassiveAbility.Type == CharacterPassiveAbilityType.Null || CurrentEnergy < Data.MaxEnergy)
        {
            return;
        }

        InDancerInvulnerability = true;
        DancerInvulnerableMark.GetComponent<SpriteRenderer>().enabled = true;
        DancerInvulnerableMark.transform.position = GetComponent<SpeedManager>().GetTruePos();
        DancerHitEnemies = new List<GameObject>();
    }

    private void DeactivateDancerInvulnerability()
    {
        InDancerInvulnerability = false;
        DancerInvulnerableMark.GetComponent<SpriteRenderer>().enabled = false;
        DancerHitEnemies.Clear();
    }

    private void DancerHitEnemy()
    {
        if (DancerPassiveAbility.Type == CharacterPassiveAbilityType.Null || !InDancerInvulnerability || DancerPassiveAbility.Level < 2)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        var Data = GetComponent<CharacterData>();
        var SpeedManager = GetComponent<SpeedManager>();

        Vector2 Offset = SpeedManager.GetTruePos() - (Vector2)transform.position;

        CharacterAttackInfo DancerAttack = new CharacterAttackInfo(gameObject, CharacterAttackType.Dancer, gameObject.transform.right.x > 0, AbilityData.DancerDamage, AbilityData.DancerDamage, AbilityData.DancerDamage, DancerShieldBreak, DancerShieldBreak , 0, 0, Offset, Offset, AbilityData.DancerHitBoxSize, AbilityData.DancerHitBoxSize);


        RaycastHit2D[] Allhits = Physics2D.BoxCastAll(SpeedManager.GetTruePos(), AbilityData.DancerHitBoxSize, 0, Vector2.zero, 0, Data.EnemyLayer);

        for(int i = 0; i < Allhits.Length; i++)
        {
            GameObject Enemy = Allhits[i].collider.gameObject;
            if (!DancerHitEnemies.Contains(Enemy))
            {
                EventManager.instance.Fire(new PlayerHitEnemy(DancerAttack,new CharacterAttackInfo(DancerAttack), Enemy));
                DancerHitEnemies.Add(Enemy);
                Enemy.GetComponent<IHittable>().OnHit(DancerAttack);
            }
        }

    }

    private void DancerGetHit()
    {
        if (!InDancerInvulnerability)
        {
            return;
        }

        GetHitAvailable = false;
    }

    //Executioner Functions

    private void ExecutionerShortenAnticipation(CharacterAttackInfo Attack)
    {
        if (ExecutionerAbility.Type == CharacterPassiveAbilityType.Null || !ExecutionerShortenAnticipationActivated)
        {
            return;
        }

        DeactivateExecutionerShorten();

        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.AnticipationTime -= AbilityData.ExecutionerAnticipationCut * Attack.BaseAnticipationTime;
    }

    private void ActivateExecutionerShorten()
    {
        if (ExecutionerAbility.Type == CharacterPassiveAbilityType.Null || ExecutionerAbility.Level < 2)
        {
            return;
        }

        ExecutionerShortenAnticipationActivated = true;
    }

    private void DeactivateExecutionerShorten()
    {
        ExecutionerShortenAnticipationActivated = false;
    }

    private void ExecutionerRecoverEnergy()
    {
        if (ExecutionerAbility.Type == CharacterPassiveAbilityType.Null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        var Data = GetComponent<CharacterData>();

        if(ExecutionerAbility.Level < 3)
        {
            GainEnergy(AbilityData.ExecutionerEnergyRecovery);
        }
        else
        {
            CurrentEnergy = Data.MaxEnergy;
        }
    }

    //Accurate Blade Functions

    private void AccurateBladeBonusDamage(CharacterAttackInfo Attack,GameObject Enemy)
    {
        if (AccurateBladeAbility.Type == CharacterPassiveAbilityType.Null || Attack.Type!=CharacterAttackType.NormalSlash)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        int BonusNumber = Enemy.GetComponent<IShield>().MaxShield - Enemy.GetComponent<IShield>().CurrentShield;
        if (BonusNumber > AbilityData.AccurateBladeMaxBonusNumber)
        {
            BonusNumber = AbilityData.AccurateBladeMaxBonusNumber;
        }

        if (BonusNumber > 0)
        {
            if (AccurateBladeAbility.Level < 2)
            {
                Attack.Damage += Mathf.RoundToInt(Attack.BaseDamage * AbilityData.AccurateBladeBonusUnit);
            }
            else
            {
                Attack.Damage += Mathf.RoundToInt(Attack.BaseDamage * AbilityData.AccurateBladeBonusUnit * BonusNumber);
            }
        }
    }

    private void AccurateBladeHeal(CharacterAttackInfo Attack, int HitNumber)
    {
        if(AccurateBladeAbility.Type == CharacterPassiveAbilityType.Null || AccurateBladeAbility.Level < 3 || Attack.Type !=CharacterAttackType.NormalSlash)
        {
            return;
        }


        var AbilityData = GetComponent<CharacterAbilityData>();

        Heal(Mathf.RoundToInt(Attack.Damage * HitNumber * AbilityData.AccurateBladeHeal));
    }

    //Cursed Blade Functions

    private void CursedBladeIncreaseRecievedDamage(EnemyAttackInfo EnemyAttack)
    {
        if(CursedBladeAbility.Type == CharacterPassiveAbilityType.Null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        EnemyAttack.Damage += Mathf.RoundToInt(EnemyAttack.BaseDamage * AbilityData.CursedBladeBattleArtDamageDecrease);
    }

    private void CursedBladeDecreaseBattleArtBaseDamage(CharacterAttackInfo Attack)
    {
        if (CursedBladeAbility.Type == CharacterPassiveAbilityType.Null || Attack.Type == CharacterAttackType.NormalSlash)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        if(CurrentEnergy == Data.MaxEnergy && CursedBladeAbility.Level >= 2)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.BaseDamage -= Mathf.RoundToInt(AbilityData.CursedBladeBattleArtDamageDecrease * Attack.OriginalDamage);

    }

    private void CursedBladeIncreaseNormalSlashBaseDamage(CharacterAttackInfo Attack)
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if (CursedBladeAbility.Type == CharacterPassiveAbilityType.Null || Attack.Type!=CharacterAttackType.NormalSlash || CursedBladeAbility.Level < 3 || CurrentEnergy < Data.MaxEnergy)
        {
            return;
        }

        Attack.BaseDamage += Mathf.RoundToInt(AbilityData.CursedBladeNormalSlashDamageIncrease * Attack.OriginalDamage);
    }

}
