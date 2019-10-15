using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Knight : StatusManagerBase , IHittable, IShield
{
    public int CurrentShield { get; set; }

    public GameObject Canvas;
    public GameObject SharedCanvas;
    public GameObject HPFill;
    public GameObject ShieldFill;

    public Color NormalColor;
    public Color RageColor;

    private GameObject DamageText;
    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<KnightData>();
        CurrentHP = Data.MaxHP;
        CurrentShield = Data.MaxShield;
    }

    // Update is called once per frame
    void Update()
    {
        SetFill();
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);
        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

        CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;

        var Data = GetComponent<KnightData>();

        CurrentHP -= HitAttack.Damage;

        if (HitAttack.Type == CharacterAttackType.Heavy)
        {
            Interrupted = true;
        }
        else
        {
            Interrupted = false;
            /*if (CurrentShield > 0)
            {
                CurrentShield -= HitAttack.Damage;
                if (CurrentShield < 0)
                {
                    CurrentShield = 0;
                }
                Interrupted = false;
            }
            else
            {
                CurrentHP -= HitAttack.Damage;
                Interrupted = true;
            }*/
        }


        if (HitAttack.Right)
        {
            DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 1);
        }
        else
        {
            DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 1);
        }
        DamageText.GetComponent<Text>().text = HitAttack.Damage.ToString();
        DamageText.transform.parent = Canvas.transform;
        if (HitAttack.Type==CharacterAttackType.Heavy)
        {
            DamageText.GetComponent<Text>().color = Color.red;
        }
        else
        {
            DamageText.GetComponent<Text>().color = Color.white;
        }


        if (CurrentHP <= 0)
        {
            DamageText.transform.parent = SharedCanvas.transform;
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RecoverShield()
    {
        if(CurrentShield <= 0)
        {
            CurrentShield = GetComponent<KnightData>().MaxShield;
        }
    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<KnightData>();
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
