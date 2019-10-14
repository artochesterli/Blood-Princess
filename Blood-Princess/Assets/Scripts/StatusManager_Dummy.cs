using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Dummy : StatusManagerBase, IHittable, IRage
{
    public bool Rage { get; set; }
    public int RageCount { get; set; }
    public GameObject HPFill;

    public Color NormalColor;
    public Color RageColor;
    public Color DeadColor;

    private float RecoveryTimeCount;
    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<DummyData>();
        CurrentHP = Data.MaxHP;
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
        if (CurrentHP > 0)
        {
            CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;
            CurrentHP -= HitAttack.Damage;

            if (HitAttack.Right)
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 1);
            }
            else
            {
                DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 1);
            }
            DamageText.GetComponent<Text>().text = HitAttack.Damage.ToString();
            DamageText.transform.parent = HPFill.transform.parent;

            var Data = GetComponent<DummyData>();
            if (CurrentHP <= Data.RageHP&&RageCount<Data.RageNumber)
            {
                Rage = true;
                RageCount++;
                GetComponent<SpriteRenderer>().color = RageColor;
            }

            if (Rage)
            {
                if( HitAttack.Type==CharacterAttackType.Heavy)
                {
                    GetComponent<SpriteRenderer>().color = NormalColor;
                    Rage = false;
                }
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
                CurrentHP = Data.MaxHP;
                RecoveryTimeCount = 0;
                Rage = false;
                RageCount = 0;
                GetComponent<SpriteRenderer>().color = NormalColor;
            }
        }
    }

    private void SetFill()
    {
        var Data = GetComponent<DummyData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;
    }
}
