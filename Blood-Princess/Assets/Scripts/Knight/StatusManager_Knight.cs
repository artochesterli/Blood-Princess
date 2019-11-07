using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Knight : StatusManagerBase, IHittable
{
    public int MaxShield { get; set; }
    public int CurrentShield { get; set; }

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


        var Data = GetComponent<KnightData>();

        CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;


        Interrupted = true;


        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));


		GetComponent<KnightAI>().Player = HitAttack.Source;


		CurrentHP -= HitAttack.Damage;

        SetHPFill((float)CurrentHP / Data.MaxHP);


		if (HitAttack.Right)
		{
			DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 0);
		}
		else
		{
			DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 0);
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
}
