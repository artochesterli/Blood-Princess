using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;
    public int CurrentAdvancedEnergy;

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
    private bool InSuperCriticalEye;


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
        EventManager.instance.AddHandler<PlayerStartRollAnticipation>(OnPlayerStartRollAnticipation);
        EventManager.instance.AddHandler<PlayerEndRollAnticipation>(OnPlayerEndRollAnticipation);
        EventManager.instance.AddHandler<PlayerStartRoll>(OnPlayerStartRoll);
        EventManager.instance.AddHandler<PlayerEndRoll>(OnPlayerEndRoll);
        EventManager.instance.AddHandler<PlayerHitEnemy>(OnPlayerHitEnemy);
        EventManager.instance.AddHandler<PlayerBreakEnemyShield>(OnPlayerBreakEnemyShield);
        EventManager.instance.AddHandler<PlayerKillEnemy>(OnPlayerKillEnemy);
        EventManager.instance.AddHandler<PlayerGetHit>(OnPlayerGetHit);

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

    }

    // Update is called once per frame
    void Update()
    {
        SetFill();

        var Action = GetComponent<CharacterAction>();

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

        EnemyAttackInfo HitAttack = (EnemyAttackInfo)Attack;


        bool IsInvulnerable = Invulnerable();


        EventManager.instance.Fire(new PlayerGetHit(HitAttack));

        if (!IsInvulnerable)
        {
            var Data = GetComponent<CharacterData>();

            bool Nulified = false;

            if (CurrentAdvancedEnergy > 0)
            {
                Nulified = true;
                HitAttack.Damage -= Mathf.RoundToInt(Data.MaxAdvancedEnergyDamageNulification * CurrentAdvancedEnergy / Data.MaxEnergy);

                if(HitAttack.Damage < 0)
                {
                    HitAttack.Damage = 0;
                }
            }

            CurrentAdvancedEnergy = 0;

            int EnergyLost = Mathf.CeilToInt(CurrentEnergy * Data.HitEnergyLostProportion);

            if (EnergyLost < Data.MinimalEnergyLost)
            {
                EnergyLost = Data.MinimalEnergyLost;
            }

            GainLoseEnergy(-EnergyLost);

            SetCriticalEye(false);


            DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

            int Damage = HitAttack.Damage;

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

            if (Nulified)
            {
                DamageText.GetComponent<Text>().color = new Color(0.7372549f, 0.1686275f, 0.6732783f);
            }
            else
            {
                DamageText.GetComponent<Text>().color = Color.white;
            }


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

        if(e.Attack.Type == CharacterAttackType.BattleArt)
        {
            switch (e.Attack.ThisBattleArt.Type)
            {
                case BattleArtType.SpiritSlash:
                    CurrentEnergy = CurrentAdvancedEnergy;
                    if(CurrentAdvancedEnergy < Data.MaxEnergy)
                    {
                        SetCriticalEye(false);
                    }
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
            if (e.Attack.Type == CharacterAttackType.Slash)
            {
                GainLoseEnergy(AbilityData.SlashEnergyGain);
            }
            else if(e.Attack.Type == CharacterAttackType.BattleArt)
            {
                switch (Action.EquipedBattleArt.Type)
                {
                    case BattleArtType.SpiritSlash:
                        GainAdvancedEnergy(AbilityData.SpiritSlashAdvancedEnergyGain);
                        GainLoseEnergy(AbilityData.SpiritSlashAdvancedEnergyGain);
                        break;
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


    }

    private void OnPlayerEndRoll(PlayerEndRoll e)
    {

    }


    private void OnPlayerGetHit(PlayerGetHit e)
    {
        var Action = GetComponent<CharacterAction>();
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

        if (e.OriginalAttack.Type == CharacterAttackType.BattleArt)
        {
            switch (Action.EquipedBattleArt.Type)
            {
                case BattleArtType.SpiritSlash:
                    if(e.Enemy.transform.right.x >0 && e.OriginalAttack.Right || e.Enemy.transform.right.x < 0 && !e.OriginalAttack.Right)
                    {
                        e.UpdatedAttack.Damage += Mathf.RoundToInt(e.OriginalAttack.BaseDamage * AbilityData.SpiritSlashBackStabDamageBonus);
                    }
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

    private void GainAdvancedEnergy(int amount)
    {
        var Data = GetComponent<CharacterData>();

        if(amount >= Data.MaxEnergy - CurrentAdvancedEnergy)
        {
            CurrentAdvancedEnergy = Data.MaxEnergy;
            return;
        }

        CurrentAdvancedEnergy += amount;
    }


}
