using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Knight : StatusManagerBase, IHittable, IShield
{

	public GameObject Canvas;
	public GameObject SharedCanvas;
	public GameObject HPFill;
    public GameObject ShieldFill;

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
        CurrentShield = Data.MaxShield;
		if (SharedCanvas == null)
			SharedCanvas = GameObject.Find("SharedCanvas");
	}

	// Update is called once per frame
	void Update()
	{
        Canvas.transform.eulerAngles = new Vector3(0, 0, 0);
	}

	public override bool OnHit(AttackInfo Attack)
	{
		base.OnHit(Attack);
		DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

		CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;

		GetComponent<KnightAI>().Player = HitAttack.Source;

		var Data = GetComponent<KnightData>();

		CurrentHP -= HitAttack.Damage;


        if (!Interrupted)
        {
            CurrentShield -= HitAttack.ShieldBreak;
            if (CurrentShield <= 0)
            {
                CurrentShield = 0;
                Interrupted = true;
            }
            else
            {
                Interrupted = false;
            }
            SetShieldFill((float)CurrentShield / Data.MaxShield);
        }


        SetHPFill((float)CurrentHP / Data.MaxHP);


		/*if (HitAttack.InterruptLevel >= Data.ShieldLevel)
		{
			Interrupted = true;
		}
		else
		{
			Interrupted = false;
		}*/


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
		if (HitAttack.Type == CharacterAttackType.NormalSlash)
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


	public void SetHPFill(float value)
	{
        HPFill.GetComponent<Image>().fillAmount = value;
	}

    public void SetShieldFill(float value)
    {
        ShieldFill.GetComponent<Image>().fillAmount = value;
    }

}
