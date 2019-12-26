using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;
    public int CurrentMaxEnergy;

    public int CurrentNormalMaxEnergy;
    public int CurrentAwakenMaxEnergy;

    public int CurrentPower;

    public int CurrentOrb;
    public int CurrentMaxOrb;


    public int CoinAmount;
    public GameObject CoinText;

    public EnemyAttackInfo CurrentTakenAttack;

    public GameObject Canvas;
    public GameObject HPFill;
    public GameObject EnergyFill;
    public GameObject EnergyFullMark;

    public GameObject Marks_4;
    public GameObject Marks_3;

    public GameObject Orbs;
    public Sprite OrbFill;
    public Sprite OrbEmpty;

    private bool InRollInvulnerability;

    private bool EnergyFull;
    private bool Awaken;

    private GameObject CurrentNormalMarks;
    private GameObject CurrentAwakenMarks;

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
        EventManager.instance.AddHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.AddHandler<PlayerGetHit>(OnPlayerGetHit);
        EventManager.instance.AddHandler<PlayerGetMoney>(OnPlayerGetMoney);

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
        EventManager.instance.RemoveHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.RemoveHandler<PlayerGetHit>(OnPlayerGetHit);
        EventManager.instance.RemoveHandler<PlayerGetMoney>(OnPlayerGetMoney);

        EventManager.instance.RemoveHandler<PlayerEquipBattleArt>(OnEquipBattleArt);
        EventManager.instance.RemoveHandler<PlayerEquipPassiveAbility>(OnEquipPassiveAbility);
        EventManager.instance.RemoveHandler<PlayerUnequipBattleArt>(OnUnequipBattleArt);
        EventManager.instance.RemoveHandler<PlayerUnequipPassiveAbility>(OnUnequipPassiveAbility);

    }

    // Update is called once per frame
    void Update()
    {
        SetFill();

        if (DamageText != null)
        {
            Utility.ObjectFollow(gameObject, DamageText, Vector2.zero);
        }

        var Action = GetComponent<CharacterAction>();

        if (InRollInvulnerability)
        {
            CheckDancerHitEnemy();
        }
    }


    public void SetRollInvulnerability(bool value)
    {
        InRollInvulnerability = value;
    }

    public bool IsEnergyFull()
    {
        return EnergyFull;
    }

    public bool IsAwaken()
    {
        return Awaken;
    }

    private void SetEnergyFull(bool value)
    {
        EnergyFull = value;
        EnergyFullMark.GetComponent<SpriteRenderer>().enabled = value;
    }

    private void SetAwaken(bool value)
    {
        Awaken = value;

        SetEnergyPresent();
    }

    private void SetEnergyPresent()
    {
        var Data = GetComponent<CharacterData>();

        if (Awaken)
        {
            CurrentNormalMarks.SetActive(false);
            CurrentAwakenMarks.SetActive(true);

            CurrentMaxEnergy = CurrentAwakenMaxEnergy;
        }
        else
        {
            CurrentNormalMarks.SetActive(true);
            CurrentAwakenMarks.SetActive(false);

            CurrentMaxEnergy = CurrentNormalMaxEnergy;
        }
    }

    private void Init()
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        CurrentMaxHP = Data.MaxHP;
        CurrentHP = Data.MaxHP;
        CurrentMaxEnergy = Data.MaxEnergy;

        CurrentOrb = 0;
        CurrentMaxOrb = Data.MaxOrb;

        CurrentNormalMaxEnergy = Data.MaxEnergy;
        CurrentAwakenMaxEnergy = Data.AwakenMaxEnergy;
        CurrentNormalMarks = Marks_4;
        CurrentAwakenMarks = Marks_3;

        CurrentPower = AbilityData.BasePower;
        CurrentEnergy = 0;

        CoinAmount = 0;
        SetCoinText();
    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / CurrentMaxHP;

        EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / CurrentMaxEnergy;

    }

    

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        CurrentTakenAttack = (EnemyAttackInfo)Attack;


        bool IsInvulnerable = Invulnerable();

        EventManager.instance.Fire(new PlayerGetHit(CurrentTakenAttack, InRollInvulnerability));

        if (!IsInvulnerable)
        {
            var Data = GetComponent<CharacterData>();

            PowerSlashLoseIncrement();

            SpiritMasterLoseOrb();
            UltimateAwakenLoseOrb();
            OneMindLoseIncrement();

            CurrentEnergy = 0;

            GainLoseOrb(-1);

            SetEnergyFull(false);
            SetAwaken(false);

            Interrupted = true;

            int Damage = CurrentTakenAttack.Damage;

            if (DamageText == null)
            {
                DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));
            }

            DamageText.GetComponent<DamageText>().ActivateSelf(Damage);

            DamageText.transform.SetParent(Canvas.transform);

            CurrentHP -= Damage;

            if (CurrentHP <= 0)
            {
                EventManager.instance.Fire(new PlayerDied());

                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        return InRollInvulnerability;
    }

    private void OnPlayerStartAttackAnticipation(PlayerStartAttackAnticipation e)
    {
        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        if (e.Attack.Type == CharacterAttackType.Slash)
        {
            OneMindIncrementSlashPotency(e.Attack);
            UltimateAwakeningBonus(e.Attack);
        }
        else if (e.Attack.ThisBattleArt != null)
        {
            CurrentEnergy = 0;
            SetEnergyFull(false);
            GainLoseOrb(1);

            switch (e.Attack.ThisBattleArt.Type)
            {
                case BattleArtType.PowerSlash:
                    PowerSlashAwakenBonus(e.Attack);
                    break;
                case BattleArtType.CrossSlash:

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
                GainLoseEnergy(AbilityData.SlashEnergyGain);

            }
            else if (e.Attack.ThisBattleArt != null)
            {
                switch (Action.EquipedBattleArt.Type)
                {
                    case BattleArtType.PowerSlash:
                        PowerSlashGainEnergy();
                        break;
                    case BattleArtType.CrossSlash:
                        CrossSlashCountHit();
                        break;
                }

                HarmonyHeal();

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

    }

    private void OnPlayerGetMoney(PlayerGetMoney e)
    {
        CoinAmount += e.Value;
        if (CoinAmount < 0)
        {
            CoinAmount = 0;
        }
        SetCoinText();
    }

    private void SetCoinText()
    {
        CoinText.GetComponent<TextMeshProUGUI>().text = "Coins: " + CoinAmount.ToString();
    }

    private void OnPlayerKillEnemy(PlayerKillEnemy e)
    {

    }

    private void OnPlayerHitEnemy(PlayerHitEnemy e)
    {
        var Action = GetComponent<CharacterAction>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        GenerateHitEffect(e);


        OneMindGainIncrement();

    }

    private void GenerateHitEffect(PlayerHitEnemy e)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();
        var Data = GetComponent<CharacterData>();

        Vector2 AttackOrigin;
        Vector2 Dir;
        bool EffectRight;

        if (e.UpdatedAttack.Dir == Direction.Right)
        {
            AttackOrigin = transform.position + e.UpdatedAttack.BaseHitBoxOffset.x * Vector3.right + e.UpdatedAttack.BaseHitBoxSize.x / 2 * Vector3.left;
            Dir = Vector2.right;
            EffectRight = false;
        }
        else
        {
            AttackOrigin = transform.position + e.UpdatedAttack.BaseHitBoxOffset.x * Vector3.left + e.UpdatedAttack.BaseHitBoxSize.x / 2 * Vector3.right;
            Dir = Vector2.left;
            EffectRight = true;
        }


        RaycastHit2D hit = Physics2D.Raycast(AttackOrigin, Dir, e.UpdatedAttack.BaseHitBoxSize.x, Data.EnemyLayer | Data.DoorLayer);
        RaycastHit2D TopHit = Physics2D.Raycast(AttackOrigin + Vector2.up * e.UpdatedAttack.HitBoxSize.y / 2, Dir, e.UpdatedAttack.BaseHitBoxSize.x, Data.EnemyLayer | Data.DoorLayer);
        RaycastHit2D DownHit = Physics2D.Raycast(AttackOrigin + Vector2.down * e.UpdatedAttack.HitBoxSize.y / 2, Dir, e.UpdatedAttack.BaseHitBoxSize.x, Data.EnemyLayer | Data.DoorLayer);

        GameObject Effect = null;

        switch (e.UpdatedAttack.Type)
        {
            case CharacterAttackType.Slash:
                Effect = AbilityData.SlashHitEffect;
                break;
            case CharacterAttackType.PowerSlash:
                Effect = AbilityData.PowerSlashHitEffect;
                break;
            case CharacterAttackType.CrossSlash:
                Effect = AbilityData.CrossSlashHitEffect;
                break;
            case CharacterAttackType.Dancer:
                Effect = AbilityData.DancerHitEffect;
                break;
        }

        if (hit)
        {
            GameObject HitEffect = GameObject.Instantiate(Effect, hit.point, Quaternion.Euler(0, 0, 0));
            e.Enemy.GetComponent<HitEffectManager>().AllInfo.Add(new HitEffectInfo(HitEffect, EffectRight));
        }
        else if (TopHit)
        {
            GameObject HitEffect = GameObject.Instantiate(Effect, TopHit.point, Quaternion.Euler(0, 0, 0));
            e.Enemy.GetComponent<HitEffectManager>().AllInfo.Add(new HitEffectInfo(HitEffect, EffectRight));
        }
        else if (DownHit)
        {
            GameObject HitEffect = GameObject.Instantiate(Effect, DownHit.point, Quaternion.Euler(0, 0, 0));
            e.Enemy.GetComponent<HitEffectManager>().AllInfo.Add(new HitEffectInfo(HitEffect, EffectRight));
        }
    }


    public void Heal(int amount)
    {
        var Data = GetComponent<CharacterData>();
        if (amount > Data.MaxHP - CurrentHP)
        {
            amount = Data.MaxHP - CurrentHP;
        }

        if (amount > 0)
        {
            /*GameObject DamageText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/DamageText"), transform.position, Quaternion.Euler(0, 0, 0));
            DamageText.GetComponent<DamageText>().TravelVector = Vector2.up;
            DamageText.GetComponent<Text>().color = Color.green;
            DamageText.transform.parent = Canvas.transform;
            DamageText.GetComponent<Text>().text = amount.ToString();*/
            CurrentHP += amount;
        }
    }

    private void GainLoseEnergy(int amount)
    {
        var Data = GetComponent<CharacterData>();
        if (amount >= CurrentMaxEnergy - CurrentEnergy)
        {
            CurrentEnergy = CurrentMaxEnergy;
            SetEnergyFull(true);
            return;
        }
        else if (amount <= -CurrentEnergy)
        {
            CurrentEnergy = 0;
            return;
        }

        CurrentEnergy += amount;
    }

    private void GainLoseOrb(int amount)
    {
        var Data = GetComponent<CharacterData>();

        CurrentOrb += amount;
        if(CurrentOrb > CurrentMaxOrb)
        {
            CurrentOrb = CurrentMaxOrb;
        }
        else if(CurrentOrb < 0)
        {
            CurrentOrb = 0;
        }

        for (int i = 0; i < CurrentMaxOrb; i++)
        {
            if (i < CurrentOrb)
            {
                Orbs.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = OrbFill;
            }
            else
            {
                Orbs.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = OrbEmpty;
            }
        }

        if(CurrentOrb == CurrentMaxOrb)
        {
            SetAwaken(true);
        }
        else
        {
            SetAwaken(false);
        }
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
                SpiritMasterChangeMarks(true);
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
                SpiritMasterChangeMarks(false);
                break;
            case PassiveAbilityType.UltimateAwakening:
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

    private void PowerSlashAwakenBonus(CharacterAttackInfo Attack)
    {
        if (PowerSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (Awaken)
        {
            if (PowerSlashBattleArt.Level == 2)
            {
                Attack.Potency += AbilityData.PowerSlashAwakenPotencyBonus_Normal;
            }
            else if (PowerSlashBattleArt.Level == 3)
            {
                Attack.Potency += AbilityData.PowerSlashAwakenPotencyBonus_Upgraded;
            }
        }
    }


    private void PowerSlashGainEnergy()
    {
        if (PowerSlashBattleArt == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (PowerSlashBattleArt.Level == 3 && Awaken)
        {
            GainLoseEnergy(AbilityData.PowerSlashEnergyGain);
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


    public void SetCrossSlashParameter()
    {
        if (CrossSlashBattleArt == null || CrossSlashBattleArt.StrikeCount > 1)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (CrossSlashBattleArt.Level >= 2)
        {
            if (Awaken)
            {
                CrossSlashBattleArt.CurrentStrikeNumber = AbilityData.CrossSlashStrikeNumber_Awaken;
            }
            else
            {
                CrossSlashBattleArt.CurrentStrikeNumber = AbilityData.CrossSlashStrikeNumber_Normal;
            }
        }
        else
        {
            CrossSlashBattleArt.CurrentStrikeNumber = AbilityData.CrossSlashStrikeNumber_Normal;
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

    private void SpiritMasterChangeMarks(bool Equip)
    {
        var AbilityData = GetComponent<CharacterAbilityData>();
        var Data = GetComponent<CharacterData>();

        CurrentNormalMarks.SetActive(false);
        CurrentAwakenMarks.SetActive(false);

        if (Equip)
        {
            CurrentNormalMarks = Marks_3;
            CurrentNormalMaxEnergy = AbilityData.SpiritMasterNormalMaxEnergy;
        }
        else
        {
            CurrentNormalMarks = Marks_4;

            CurrentNormalMaxEnergy = Data.MaxEnergy;
        }

        SetEnergyPresent();

        if(CurrentEnergy > CurrentMaxEnergy)
        {
            CurrentEnergy = CurrentMaxEnergy;
        }

    }

    private void SpiritMasterLoseOrb()
    {
        if (SpiritMasterPassiveAbility == null)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        GainLoseOrb(-CurrentMaxOrb);
    }

    //UltimateAwakening

    private void UltimateAwakeningBonus(CharacterAttackInfo Attack)
    {
        if (UltimateAwakeningPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        if (Awaken)
        {
            Attack.Potency += AbilityData.UltimateAwakeningExtraPotency;
        }
    }

    private void UltimateAwakenLoseOrb()
    {
        if (UltimateAwakeningPassiveAbility == null)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        GainLoseOrb(-CurrentMaxOrb);
    }


    //CriticalEye

    private void CriticalEyeBonus(CharacterAttackInfo Attack)
    {
        if (CriticalEyePassiveAbility == null)
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
        if (BattleArtMasterPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();
        Attack.Potency += AbilityData.BattleArtMasterExtraPotency;
    }

    //One Mind

    private void OneMindGainIncrement()
    {
        if (OneMindPassiveAbility == null)
        {
            return;
        }

        var AbilityData = GetComponent<CharacterAbilityData>();

        OneMindPassiveAbility.IncrementCount++;
        if (OneMindPassiveAbility.IncrementCount > AbilityData.OneMindMaxIncrement)
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
        if (DancerPassiveAbility.DancerAttack.Dir == Direction.Left)
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
        if (DancerPassiveAbility == null)
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

        DancerPassiveAbility.DancerAttack = new CharacterAttackInfo(gameObject, CharacterAttackType.Dancer, dir, CurrentPower, CurrentPower, AbilityData.DancerPotency, AbilityData.DancerPotency, AbilityData.DancerInterruptLevel, AbilityData.DancerInterruptLevel, SpeedManager.OriPos, SpeedManager.OriPos, AbilityData.DancerHitBoxSize, AbilityData.DancerHitBoxSize);
    }
}
