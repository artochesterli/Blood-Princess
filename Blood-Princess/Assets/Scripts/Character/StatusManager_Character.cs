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
    public GameObject EnergyFill;
    public GameObject EnergyMarks;
    public GameObject CriticalEyeMark;
    public GameObject SpiritSlashInvulnerableMark;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

    private bool InRollInvulnerability;

    private bool InSpiritSlashInvulnerability;

    private bool InCriticalEye;


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

    }

    public void SetSpiritSlashInvulnerability(bool value)
    {
        InSpiritSlashInvulnerability = value;
        if (InSpiritSlashInvulnerability)
        {
            SpiritSlashInvulnerableMark.GetComponent<SpriteRenderer>().enabled = true;
            SpiritSlashInvulnerableMark.transform.position = GetComponent<SpeedManager>().GetTruePos();
        }
        else
        {
            SpiritSlashInvulnerableMark.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public bool GetCriticalEye()
    {
        return InCriticalEye;
    }

    private void SetCriticalEye (bool value)
    {
        InCriticalEye = value;
        if (InCriticalEye)
        {
            CriticalEyeMark.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            CriticalEyeMark.GetComponent<SpriteRenderer>().enabled = false;
        }
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

        /*int count = 0;

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
        }*/

    }




    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        EnemyAttackInfo HitAttack = (EnemyAttackInfo)Attack;

        //GetHitAvailable = true;

        EventManager.instance.Fire(new PlayerGetHit(HitAttack));

        if (!Invulnerable())
        {
            if (InCriticalEye)
            {
                CurrentEnergy = 0;
                SetCriticalEye(false);
            }


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

    private bool Invulnerable()
    {
        return InRollInvulnerability || InSpiritSlashInvulnerability;
    }

    private void OnPlayerStartAttackAnticipation(PlayerStartAttackAnticipation e)
    {


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

        if (e.HitEnemies.Count > 0)
        {
            if (e.Attack.Type == CharacterAttackType.NormalSlash)
            {
                if (InCriticalEye)
                {
                    GainEnergy(AbilityData.NormalSlashEnergyGain + AbilityData.NormalSlashCriticalEyeEnergyGainBonus);
                    SetCriticalEye(false);
                }
                else
                {
                    GainEnergy(AbilityData.NormalSlashEnergyGain);
                }

                if(CurrentEnergy == Data.MaxEnergy)
                {
                    SetCriticalEye(true);
                }

            }
            else if (e.Attack.Type == CharacterAttackType.SpiritSlash)
            {
                if (CurrentEnergy == Data.MaxEnergy)
                {
                    CurrentEnergy = 0;
                    SetCriticalEye(false);
                }
                else
                {
                    SetCriticalEye(true);
                }
            }
        }
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
        InRollInvulnerability = true;

    }

    private void OnPlayerEndRoll(PlayerEndRoll e)
    {
        InRollInvulnerability = false;
    }


    private void OnPlayerGetHit(PlayerGetHit e)
    {
        
    }

    private void OnPlayerKillEnemy(PlayerKillEnemy e)
    {

    }

    private void OnPlayerHitEnemy(PlayerHitEnemy e)
    {

    }

    private void OnPlayerBreakEnemyShield(PlayerBreakEnemyShield e)
    {

    }

    private void OnPlayerEquipEnhancement(PlayerEquipEnhancement e)
    {

    }

    private void OnPlayerUnequipEnhancement(PlayerUnequipEnhancement e)
    {

    }

    private void OnPlayerUpgradeEnhancement(PlayerUpgradeEnhancement e)
    {

    }


    private void OnPlayerDowngradeEnhancement(PlayerDowngradeEnhancement e)
    {

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

    private void GainEnergy(int amount)
    {
        var Data = GetComponent<CharacterData>();
        if(amount > Data.MaxEnergy - CurrentEnergy)
        {
            amount = Data.MaxEnergy - CurrentEnergy;
        }

        CurrentEnergy += amount;
    }


}
