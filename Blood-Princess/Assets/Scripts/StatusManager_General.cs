using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// A Generalized Status Manager that Could be used by a general Enemy
/// </summary>
[RequireComponent(typeof(BehaviorTree))]
public class StatusManager_General : StatusManagerBase, IHittable
{
	public int MaxHP = 100;
	public GameObject Canvas;
	public GameObject SharedCanvas;
	public GameObject HPFill;

	private GameObject DamageText;
	private BehaviorTree m_BehaviorTree;

	private void Awake()
	{
		if (Canvas == null)
			Canvas = transform.Find("StatusCanvas").gameObject;
		if (SharedCanvas == null)
			SharedCanvas = GameObject.Find("SharedCanvas");
		if (HPFill == null)
			HPFill = Canvas.transform.GetChild(0).GetChild(0).gameObject;
		CurrentHP = MaxHP;
		m_BehaviorTree = GetComponent<BehaviorTree>();
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

        int Damage = Utility.GetEffectValue(HitAttack.Power, HitAttack.Potency);

		CurrentHP -= Damage;

		m_BehaviorTree.SendEvent("Attacked");

		if (HitAttack.Type != CharacterAttackType.Slash)
		{
			Interrupted = true;
		}
		else
		{
			Interrupted = false;
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
		DamageText.transform.parent = Canvas.transform;
		if (HitAttack.Type == CharacterAttackType.Slash)
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

	private void m_OnHitPlayer(PlayerGetHit ev)
	{
		if (!ev.PlayerInRollInvulnerability)
			m_BehaviorTree.SetVariableValue("PlayerHit", true);
	}

	private void OnEnable()
	{
		EventManager.instance.AddHandler<PlayerGetHit>(m_OnHitPlayer);
	}

	private void OnDisable()
	{
		EventManager.instance.RemoveHandler<PlayerGetHit>(m_OnHitPlayer);
	}

	private void SetFill()
	{
		Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
		HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / MaxHP;
	}
}
