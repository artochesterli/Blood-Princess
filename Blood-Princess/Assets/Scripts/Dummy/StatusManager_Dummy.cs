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
    private GameObject DamageText;
    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<DummyData>();
        CurrentMaxHP = Data.MaxHP;
        CurrentHP = Data.MaxHP;
        CurrentShield = Data.MaxShield;
    }

    // Update is called once per frame
    void Update()
    {
        if (DamageText != null)
        {
            Utility.ObjectFollow(gameObject, DamageText, Vector2.zero);
        }

        CheckRecovery();
        SetFill();
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        if (CurrentHP > 0)
        {
            CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;

            var Data = GetComponent<DummyData>();

            Interrupted = true;

            int Damage = Utility.GetEffectValue(HitAttack.Power, HitAttack.Potency);

            CurrentHP -= Damage;
                

            if (DamageText == null)
            {
                DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));
            }

            DamageText.GetComponent<DamageText>().ActivateSelf(Damage);

            DamageText.transform.SetParent(Canvas.transform);

            if (CurrentHP <= 0)
            {
                return true;
            }
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
