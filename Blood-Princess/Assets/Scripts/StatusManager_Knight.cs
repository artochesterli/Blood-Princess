using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Knight : StatusManagerBase , IHittable, IRage
{
    public bool Rage { get; set; }
    public int RageCount { get; set; }

    public GameObject Canvas;
    public GameObject SharedCanvas;
    public GameObject HPFill;

    public Color NormalColor;
    public Color RageColor;

    private GameObject DamageText;
    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<KnightData>();
        CurrentHP = Data.MaxHP;
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
        DamageText.transform.parent = Canvas.transform;

        bool Knocked = true;
        if (Rage)
        {
            if (HitAttack.Type == CharacterAttackType.Light)
            {
                Knocked = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = NormalColor;
                Rage = false;
            }
        }

        var Data = GetComponent<KnightData>();
        if(CurrentHP<=Data.RageHP && RageCount < Data.RageNumber)
        {
            Rage = true;
            RageCount++;
            GetComponent<SpriteRenderer>().color = RageColor;
        }

        hit = Knocked;
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

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<KnightData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;
    }
}
