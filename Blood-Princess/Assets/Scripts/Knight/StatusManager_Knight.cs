using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Knight : StatusManagerBase, IHittable
{

    public GameObject Canvas;
	public GameObject SharedCanvas;
	public GameObject HPFill;

    public CharacterAttackInfo CurrentTakenAttack;

    public Color NormalColor;

	private GameObject DamageText;


	// Start is called before the first frame update
	void Start()
	{
		var Data = GetComponent<KnightData>();
        CurrentTakenAttack = null;
        MaxHP = Data.MaxHP;
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

        CurrentTakenAttack = (CharacterAttackInfo)Attack;

        var AI = GetComponent<KnightAI>();

        if (AI.CurrentState != KnightState.Anticipation && AI.CurrentState != KnightState.Strike && AI.CurrentState != KnightState.BlinkPrepare)
        {
            Interrupted = true;
        }
        else
        {
            Interrupted = false;
        }


        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));


		CurrentHP -= CurrentTakenAttack.Damage;

        SetHPFill((float)CurrentHP / Data.MaxHP);


		if (CurrentTakenAttack.Dir == Direction.Right)
		{
			DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 0);
		}
		else
		{
			DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 0);
		}
		DamageText.GetComponent<Text>().text = CurrentTakenAttack.Damage.ToString();
		DamageText.transform.parent = Canvas.transform;

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
