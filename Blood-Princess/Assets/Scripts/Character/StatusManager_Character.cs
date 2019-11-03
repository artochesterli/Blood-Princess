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

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;


    private BattleArtEnhancement CriticalEyeEnhancement;
    private bool HaveCriticalEyeBuff;
    private int CurrentCriticalEyeBonusNumber;
    private int MaxCriticalEyeBonusNumber;

    private BattleArtEnhancement HarmonyEnhancement;
    private bool HarmonyAvailable;
    private float HarmonyHealUnit;





    private GameObject DamageText;

    private float TimeCount;

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
        EventManager.instance.AddHandler<PlayerHitEnemy>(OnPlayerHitEnemy);

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
        EventManager.instance.RemoveHandler<PlayerHitEnemy>(OnPlayerHitEnemy);

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
    }

    private void Init()
    {
        var Data = GetComponent<CharacterData>();
        CurrentHP = Data.MaxHP;
        CurrentEnergy = 0;

        CriticalEyeEnhancement = GetNullBattleArtEnhancement();
        HarmonyEnhancement = GetNullBattleArtEnhancement();
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


        LoseCriticalEye();



        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

        EnemyAttackInfo HitAttack = (EnemyAttackInfo)Attack;

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

        if(CurrentHP <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnPlayerStartAttackAnticipation(PlayerStartAttackAnticipation e)
    {
        CurrentEnergy -= e.Attack.Cost;

        CheckHarmonyAvailable(e.Attack);
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


    }

    private void OnPlayerStartAttackRecovery(PlayerStartAttackRecovery e)
    {

    }

    private void OnPlayerEndAttackRecovery(PlayerEndAttackRecovery e)
    {

    }

    private void OnPlayerHitEnemy(PlayerHitEnemy e)
    {
        var Data = GetComponent<CharacterData>();

        if (e.Attack.Type == CharacterAttackType.NormalSlash)
        {
            CurrentEnergy++;
            if(CurrentEnergy > Data.MaxEnergy)
            {
                CurrentEnergy = Data.MaxEnergy;
            }
        }
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
        }
    }

    private void OnPlayerEquipPassiveAbility(PlayerEquipPassiveAbility e)
    {

    }

    private void OnPlayerUnequipPassiveAbility(PlayerUnequipPassiveAbility e)
    {

    }

    private void OnPlayerUpgradePassiveAbility(PlayerUpgradePassiveAbility e)
    {

    }

    private void OnPlayerDowngradePassiveAbility(PlayerDowngradePassiveAbility e)
    {

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

        if(HarmonyEnhancement.Type == BattleArtEnhancementType.Null && HarmonyEnhancement.EnhancementAttackType != Attack.Type)
        {
            return;
        }

        var Data = GetComponent<CharacterData>();
        var AbilityData = GetComponent<CharacterAbilityData>();

        int HealNumber = Mathf.RoundToInt(Attack.Damage * HitNumber * HarmonyHealUnit);


        if (CurrentHP == Data.MaxHP && HarmonyEnhancement.Level == 3)
        {
            CurrentEnergy+=AbilityData.HarmonyEnergyRecovery;
            if(CurrentEnergy > Data.MaxEnergy)
            {
                CurrentEnergy = Data.MaxEnergy;
            }
        }
        else
        {
            Heal(HealNumber);
        }
    }


}
