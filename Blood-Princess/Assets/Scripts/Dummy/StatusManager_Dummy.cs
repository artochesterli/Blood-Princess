using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Dummy : StatusManagerBase, IHittable, IShield
{
    public int MaxShield { get; set; }
    public int CurrentShield { get; set; }

    public GameObject HPFill;
    public GameObject ShieldFill;
    public GameObject Canvas;

    public Color NormalColor;
    public Color DeadColor;

    private float RecoveryTimeCount;
    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<DummyData>();
        MaxHP = Data.MaxHP;
        CurrentHP = Data.MaxHP;
        CurrentShield = Data.MaxShield;
    }

    // Update is called once per frame
    void Update()
    {
        CheckRecovery();
        SetFill();
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);
        GameObject DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));
        DamageText.transform.SetParent (Canvas.transform);
        if (CurrentHP > 0)
        {
            CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;

            var Data = GetComponent<DummyData>();

            Interrupted = true;

            int Damage = Utility.GetEffectValue(HitAttack.Power, HitAttack.Potency);

            if (HitAttack.Type != CharacterAttackType.Slash)
            {
                CurrentHP -= Damage;
                
            }

            if (HitAttack.Dir == Direction.Right)
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 1);
            }
            else
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 1);
            }
            DamageText.GetComponent<Text>().text = Damage.ToString();
            if (Interrupted)
            {
                DamageText.GetComponent<Text>().color = Color.red;
            }
            else
            {
                DamageText.GetComponent<Text>().color = Color.white;
            }

            if (CurrentHP <= 0)
            {
                return true;
            }
        }
        else
        {
            DamageText.GetComponent<Text>().text = "0";
            DamageText.transform.parent = HPFill.transform.parent;
        }

        return false;
    }

    private void CheckRecovery()
    {
        if (CurrentHP <= 0)
        {
            GetComponent<SpriteRenderer>().color = DeadColor;
            RecoveryTimeCount += Time.deltaTime;
            var Data = GetComponent<DummyData>();
            if (RecoveryTimeCount > Data.RecoveryTime)
            {
                RecoverShield();
                CurrentHP = Data.MaxHP;
                RecoveryTimeCount = 0;
                GetComponent<SpriteRenderer>().color = NormalColor;
            }
        }
    }

    public void RecoverShield()
    {
        CurrentShield = GetComponent<DummyData>().MaxShield;
    }

    private void SetFill()
    {
        var Data = GetComponent<DummyData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;
        if (Data.MaxShield > 0)
        {
            ShieldFill.GetComponent<Image>().fillAmount = (float)CurrentShield / Data.MaxShield;
        }
        else
        {
            ShieldFill.GetComponent<Image>().enabled = false;
            ShieldFill.transform.parent.GetComponent<Image>().enabled = false;
        }
    }
}
