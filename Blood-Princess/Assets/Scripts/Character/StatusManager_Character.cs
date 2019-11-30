using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;
    public int CurrentMaxEnergy;
    public int CurrentSeal;
    public int CurrentPower;

    public EnemyAttackInfo CurrentTakenAttack;

    public GameObject Canvas;
    public GameObject HPFill;
    public GameObject EnergyFill;
    public GameObject EnergyFullMark;
    public GameObject SealFill;
    public GameObject SealMarks;

    public GameObject ParryShieldMark;
    public GameObject ParriedEffectPrefab;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

    private bool InRollInvulnerability;

    private bool InParryInvulnerability;

    private bool EnergyFull;
    private bool Awaken;

    private GameObject DamageText;


    private PowerSlash PowerSlashBattleArt;
    private CrossSlash CrossSlashBattleArt;

    private Harmony HarmonyPassiveAbility;
    private SpiritMaster SpiritMasterPassiveAbility;
    private UltimateAwakening UltimateAwakeningPassiveAbility;
    private CriticalEye CriticalEyePassiveAbility;
    private BattleArtMaster BattleArtMasterPassiveAbility;
    private OneMind OneMindPassiveAbility;
    private Dancer DancerPassiveAbility;


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

        if (InRollInvulnerability)
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

    public bool IsEnergyFull()
    {
        return EnergyFull;
    }

    private void SetEnergyFull(bool value)
    {
        EnergyFull = value;
        EnergyFullMark.GetComponent<SpriteRenderer>().enabled = value;
    }

    private void SetAwaken(bool value)
    {
        Awaken = value;
    }

    private void Init()
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        CurrentHP = Data.MaxHP;
        CurrentMaxEnergy = Data.MaxEnergy;
        CurrentSeal = Data.MaxSealNumber;
        CurrentPower = AbilityData.BasePower;
        CurrentEnergy = 0;

    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;

        EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / (Data.MaxEnergy + Data.MaxSealNumber*Data.SealBreakEnergyCap);

        SealFill.GetComponent<Image>().fillAmount = (float)(CurrentSeal* Data.SealBreakEnergyCap)/ (Data.MaxEnergy + Data.MaxSealNumber * Data.SealBreakEnergyCap);

        for (int i = 1; i <= CurrentSeal; i++)
        {
            SealMarks.transform.GetChild(Data.MaxSealNumber - i).gameObject.SetActive(true);
        }

        for(int i= CurrentSeal+1 ; i <= Data.MaxSealNumber; i++)
        {
            SealMarks.transform.GetChild(Data.MaxSealNumber - i).gameObject.SetActive(false);
        }

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

            PowerSlashLoseIncrement();

            SpiritMasterGainSeal();
            UltimateAwakenGainSeal();
            OneMindLoseIncrement();

            CurrentEnergy = 0;
            GainLoseSeal(1);
            SetEnergyFull(false);
            SetAwaken(false);

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
            OneMindIncrementSlashPotency(e.Attack);
        }
        else if (e.Attack.ThisBattleArt != null)
        {
            CurrentEnergy = 0;
            SetEnergyFull(false);

            switch (e.Attack.ThisBattleArt.Type)
            {
                case BattleArtType.PowerSlash:
                    PowerSlashIncrementPotency(e.Attack);
                    break;
                case BattleArtType.CrossSlash:
                    CrossSlashLastStrikeBonus(e.Attack);
                    break;
            }

            BattleArtMasterBonus(e.Attack);
        }

        CriticalEyeBonus(e.Attack);

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
            if (e.Attack.Type == CharacterAttackType.Slash)
            {
                if (Awaken)
                {
                    GainLoseEnergy(AbilityData.SlashEnergyAwakenGain);
                }
                else
                {
                    GainLoseEnergy(AbilityData.SlashEnergyGain);
                }

                SpiritMasterEnergyGainBonus();
            }
            else if(e.Attack.ThisBattleArt != null)
            {
                switch (Action.EquipedBattleArt.Type)
                {
                    case BattleArtType.PowerSlash:
                        PowerSlashGainEnergy();
                        PowerSlashSelfIncrement();
                        break;
                    case BattleArtType.CrossSlash:
                        CrossSlashCountHit();
                        CrossSlashBreakSeal();
                        break;
                }

                HarmonyHeal();

                EndStrikeBreakSeal();
            }
        }
    }

    private void EndStrikeBreakSeal()
    {
        if(CrossSlashBattleArt != null && CrossSlashBattleArt.StrikeHitCount > 1)
        {


            return;
        }

        GainLoseSeal(-1);
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

        SetUpDancer();
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

        OneMindGainIncrement();
        
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
        if(amount >= CurrentMaxEnergy - CurrentEnergy)
        {
            CurrentEnergy = CurrentMaxEnergy;
            SetEnergyFull(true);
            return;
        }
        else if(amount <= -CurrentEnergy)
        {
            CurrentEnergy = 0;
            return;
        }

        CurrentEnergy += amount;
    }

    private void GainLoseSeal(int amount)
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if(amount > 0)
        {
            CurrentSeal += amount;
            if(CurrentSeal > Data.MaxSealNumber)
            {
                CurrentSeal = Data.MaxSealNumber;
            }

            SetAwaken(false);
        }
        else if(amount < 0)
        {
            CurrentSeal += amount;
            if(CurrentSeal <= 0)
            {
                CurrentSeal = 0;
                SetAwaken(true);
            }
        }

        CurrentMaxEnergy = Data.MaxEnergy + Data.SealBreakEnergyCap * (Data.MaxSealNumber - CurrentSeal);
        CurrentPower = AbilityData.BasePower + Data.PowerIncrementWithSeal[CurrentSeal];

        UltimateAwakeningBonus();
    }

    private void OnEquipBattleArt(PlayerEquipBattleArt e)
    {
        var Action = GetComponent<CharacterAction>();

        switch (e.ThisBattleArt.Type)
        {
            case BattleArtType.PowerSlash:
                PowerSlashBattleArt = (PowerSlash)e.ThisBattleArt;
                break;
            case BattleArtType.CrossSlash:
                CrossSlashBattleArt = (CrossSlash)e.ThisBattleArt;
                break;
        }

        Action.EquipedBattleArt = e.ThisBattleArt;
    }

    private void OnUnequipBattleArt(PlayerUnequipBattleArt e)
    {
        var Action = GetComponent<CharacterAction>();
        switch (e.ThisBattleArt.Type)
        {
            case BattleArtType.PowerSlash:
                PowerSlashLoseIncrement();
                PowerSlashBattleArt = null;
                break;
            case BattleArtType.CrossSlash:
                CrossSlashBattleArt = null;
                break;

        }

        Action.EquipedBattleArt = null;
    }

    private void OnEquipPassiveAbility(PlayerEquipPassiveAbility e)
    {
        var Action = GetComponent<CharacterAction>();

        switch (e.ThisPassiveAbility.Type)
        {
            case PassiveAbilityType.Harmony:
                HarmonyPassiveAbility = (Harmony)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.SpiritMaster:
                SpiritMasterPassiveAbility = (SpiritMaster)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.UltimateAwakening:
                UltimateAwakeningPassiveAbility = (UltimateAwakening)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.CriticalEye:
                CriticalEyePassiveAbility = (CriticalEye)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.BattleArtMaster:
                BattleArtMasterPassiveAbility = (BattleArtMaster)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.OneMind:
                OneMindPassiveAbility = (OneMind)e.ThisPassiveAbility;
                break;
            case PassiveAbilityType.Dancer:
                DancerPassiveAbility = (Dancer)e.ThisPassiveAbility;
                break;
        }

        Action.EquipedPassiveAbility[e.Index] = e.ThisPassiveAbility;
    }

    private void OnUnequipPassiveAbility(PlayerUnequipPassiveAbility e)
    {
        var Action = GetComponent<CharacterAction>();

        switch (e.ThisPassiveAbility.Type)
        {
            case PassiveAbilityType.Harmony:
                HarmonyPassiveAbility = null;
                break;
            case PassiveAbilityType.SpiritMaster:
                SpiritMasterPassiveAbility = null;
                break;
            case PassiveAbilityType.UltimateAwakening:
                UltimateAwakeningLoseBonus();
                UltimateAwakeningPassiveAbility = null;
                break;
            case PassiveAbilityType.CriticalEye:
                CriticalEyePassiveAbility = null;
                break;
            case PassiveAbilityType.BattleArtMaster:
                BattleArtMasterPassiveAbility = null;
                break;
            case PassiveAbilityType.OneMind:
                OneMindLoseIncrement();
                OneMindPassiveAbility = null;
                break;
            case PassiveAbilityType.Dancer:
                DancerPassiveAbility = null;
                break;
        }

        Action.EquipedPassiveAbility[e.Index] = null;
    }

    //Power Slash

    private void PowerSlashSelfIncrement()
    {
        if (PowerSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if(PowerSlashBattleArt.Level >= 2)
        {
            PowerSlashBattleArt.IncrementCount++;

            int MaxIncrement = AbilityData.PowerSlashMaxIncrement_Normal;
            if(PowerSlashBattleArt.Level == 3)
            {
                MaxIncrement = AbilityData.PowerSlashMaxIncrement_Upgraded;
            }

            if(PowerSlashBattleArt.IncrementCount > MaxIncrement)
            {
                PowerSlashBattleArt.IncrementCount = MaxIncrement;
            }
        }
    }

    private void PowerSlashIncrementPotency(CharacterAttackInfo Attack)
    {
        if (PowerSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Potency += AbilityData.PowerSlashPotencyIncrement * PowerSlashBattleArt.IncrementCount;
    }
   
    private void PowerSlashGainEnergy()
    {
        if (PowerSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (PowerSlashBattleArt.Level == 3 && PowerSlashBattleArt.IncrementCount == AbilityData.PowerSlashMaxIncrement_Upgraded)
        {
            GainLoseEnergy(AbilityData.PowerSlashMaxIncrementEnergyGain);
        }
    }

    private void PowerSlashLoseIncrement()
    {
        if (PowerSlashBattleArt == null)
        {
            return;
        }

        PowerSlashBattleArt.IncrementCount = 0;

    }

    //Cross Slash

    private void CrossSlashBreakSeal()
    {
        if(CrossSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        if(CrossSlashBattleArt.Level == 3 && CrossSlashBattleArt.StrikeHitCount == AbilityData.CrossSlashStrikeNumber_Upgraded)
        {
            GainLoseSeal(-AbilityData.CrossSlashSealBreakBonus);
        }
    }

    private void CrossSlashLastStrikeBonus(CharacterAttackInfo Attack)
    {
        if (CrossSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        int LastStrike = AbilityData.CrossSlashStrikeNumber_Normal;
        if(CrossSlashBattleArt.Level >= 2)
        {
            LastStrike = AbilityData.CrossSlashStrikeNumber_Upgraded;
        }

        if(Awaken && CrossSlashBattleArt.StrikeCount == LastStrike)
        {
            Attack.Potency += AbilityData.CrossSlashAwakenPotencyBonus;
        }
    }

    private void CrossSlashCountHit()
    {
        if (CrossSlashBattleArt == null)
        {
            return;
        }

        CrossSlashBattleArt.StrikeHitCount++;
    }

    //Harmony

    private void HarmonyHeal()
    {
        if (HarmonyPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        Heal(Utility.GetEffectValue(CurrentPower, AbilityData.HarmonyHealPotency));
    }

    //SpiritMaster

    private void SpiritMasterEnergyGainBonus()
    {
        if(SpiritMasterPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        GainLoseEnergy(AbilityData.SpiritMasterExtraEnergyGainListWithSeal[CurrentSeal]);
    }

    private void SpiritMasterGainSeal()
    {
        if(SpiritMasterPassiveAbility == null)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        GainLoseSeal(Data.MaxSealNumber);
    }

    //UltimateAwakening

    private void UltimateAwakeningBonus()
    {
        if (UltimateAwakeningPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (Awaken)
        {
            CurrentPower += AbilityData.UltimateAwakeningExtraPower;
        }
    }

    private void UltimateAwakeningLoseBonus()
    {
        if (UltimateAwakeningPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (Awaken)
        {
            CurrentPower -= AbilityData.UltimateAwakeningExtraPower;
        }
    }

    private void UltimateAwakenGainSeal()
    {
        if (UltimateAwakeningPassiveAbility == null)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        GainLoseSeal(Data.MaxSealNumber);
    }



    //CriticalEye

    private void CriticalEyeBonus(CharacterAttackInfo Attack)
    {
        if(CriticalEyePassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        if (Attack.Potency >= AbilityData.CriticalEyePotencyRequired)
        {
            Attack.Potency += AbilityData.CriticalEyeExtraPotency;
        }
    }

    //BattleArtMaster

    private void BattleArtMasterBonus(CharacterAttackInfo Attack)
    {
        if(BattleArtMasterPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        Attack.Potency += AbilityData.BattleArtMasterExtraPotency;
    }

    //One Mind

    private void OneMindGainIncrement()
    {
        if(OneMindPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        OneMindPassiveAbility.IncrementCount++;
        if(OneMindPassiveAbility.IncrementCount > AbilityData.OneMindMaxIncrement)
        {
            OneMindPassiveAbility.IncrementCount = AbilityData.OneMindMaxIncrement;
        }
    }

    private void OneMindIncrementSlashPotency(CharacterAttackInfo Attack)
    {
        if (OneMindPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        Attack.Potency += AbilityData.OneMindSlashPotencyIncrement * OneMindPassiveAbility.IncrementCount;

    }

    private void OneMindLoseIncrement()
    {
        if (OneMindPassiveAbility == null)
        {
            return;
        }

        OneMindPassiveAbility.IncrementCount = 0;
    }

    //Dancer

    private void CheckDancerHitEnemy()
    {
        if (DancerPassiveAbility == null)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();
        var SpeedManager = GetComponent<SpeedManager>();

        Vector2 Dir = Vector2.right;
        if(DancerPassiveAbility.DancerAttack.Dir == Direction.Left)
        {
            Dir = Vector2.left;
        }

        RaycastHit2D[] AllHits = Physics2D.BoxCastAll(SpeedManager.GetTruePos(), AbilityData.DancerHitBoxSize, 0, Dir, 0, Data.EnemyLayer);
        if (AllHits.Length > 0)
        {
            for (int i = 0; i < AllHits.Length; i++)
            {
                GameObject Enemy = AllHits[i].collider.gameObject;

                if (!DancerPassiveAbility.HitEnemies.Contains(Enemy))
                {
                    CharacterAttackInfo UpdatedAttack = new CharacterAttackInfo(DancerPassiveAbility.DancerAttack);

                    EventManager.instance.Fire(new PlayerHitEnemy(DancerPassiveAbility.DancerAttack, UpdatedAttack, Enemy));

                    Enemy.GetComponent<IHittable>().OnHit(UpdatedAttack);
                    DancerPassiveAbility.HitEnemies.Add(Enemy);
                }
            }
        }
    }

    private void SetUpDancer()
    {
        if(DancerPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        var SpeedManager = GetComponent<SpeedManager>();

        DancerPassiveAbility.HitEnemies.Clear();

        Direction dir;
        if (transform.right.x > 0)
        {
            dir = Direction.Right;
        }
        else
        {
            dir = Direction.Left;
        }

        DancerPassiveAbility.DancerAttack = new CharacterAttackInfo(gameObject, CharacterAttackType.Dancer, dir, CurrentPower,CurrentPower,AbilityData.DancerPotency,AbilityData.DancerPotency,AbilityData.DancerInterruptLevel,AbilityData.DancerInterruptLevel, SpeedManager.OriPos, SpeedManager.OriPos, AbilityData.DancerHitBoxSize, AbilityData.DancerHitBoxSize);
    }
}
