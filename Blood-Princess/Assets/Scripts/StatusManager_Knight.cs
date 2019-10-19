using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Knight : StatusManagerBase , IHittable
{

    public GameObject Canvas;
    public GameObject SharedCanvas;
    public GameObject HPFill;
    public GameObject ShockEffect;

    public Color NormalColor;
    public Color RageColor;

    private GameObject DamageText;

    private float ShockEffectTimeCount;
    private float ShockEffectTime = 0.2f;
    private bool ShockEffectActivate;
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
        //CheckShockEffect();
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);
        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

        CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;

        var Data = GetComponent<KnightData>();

        CurrentHP -= HitAttack.Damage;

        if (HitAttack.Type != CharacterAttackType.NormalSlash)
        {
            Interrupted = true;
        }
        else
        {
            Interrupted = false;
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
        if (HitAttack.Type==CharacterAttackType.NormalSlash)
        {
            DamageText.GetComponent<Text>().color = Color.red;
        }
        else
        {
            DamageText.GetComponent<Text>().color = Color.white;
        }
        DamageText.GetComponent<Text>().color = Color.white;

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

    private void CheckShockEffect()
    {
        if (!ShockEffectActivate)
        {
            var Data = GetComponent<KnightData>();
            var SpeedManager = GetComponent<SpeedManager>();
            if (SpeedManager.Left && SpeedManager.Left.CompareTag("Player") && SpeedManager.HitLeft)
            {
                ShockPlayer(false, SpeedManager.Left);
            }
            else if (SpeedManager.Right && SpeedManager.Right.CompareTag("Player") && SpeedManager.HitRight)
            {
                ShockPlayer(true, SpeedManager.Right);
            }
            else if (SpeedManager.Top && SpeedManager.Top.CompareTag("Player") && SpeedManager.HitTop)
            {
                float XDiff = SpeedManager.Top.GetComponent<SpeedManager>().GetTruePos().x - SpeedManager.GetTruePos().x;
                ShockPlayer(XDiff > 0, SpeedManager.Top);
            }
        }
        else
        {
            ShockEffectTimeCount += Time.deltaTime;
            if(ShockEffectTimeCount >= ShockEffectTime)
            {
                ShockEffectTimeCount = 0;
                ShockEffectActivate = false;
                ShockEffect.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void ShockPlayer(bool Right,GameObject Player)
    {
        var Data = GetComponent<KnightData>();

        EnemyAttackInfo Attack = new EnemyAttackInfo(gameObject, EnemyAttackType.Shock, Right, Data.ShockDamage, Vector2.zero, Vector2.zero);
        ShockEffect.GetComponent<SpriteRenderer>().enabled = true;
        ShockEffectActivate = true;
        Player.GetComponent<IHittable>().OnHit(Attack);
    }

}
