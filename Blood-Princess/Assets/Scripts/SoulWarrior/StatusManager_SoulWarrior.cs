using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_SoulWarrior : StatusManagerBase, IHittable
{
    public GameObject Canvas;
    public GameObject SharedCanvas;
    public GameObject HPFill;
    public bool OffBalance;

    public CharacterAttackInfo CurrentTakenAttack;

    private GameObject DamageText;

    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<SoulWarriorData>();
        CurrentHP = Data.MaxHP;
        if (SharedCanvas == null)
            SharedCanvas = GameObject.Find("SharedCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        Canvas.transform.eulerAngles = Vector3.zero;
        if (DamageText != null)
        {
            Utility.ObjectFollow(gameObject, DamageText, Vector2.zero);
        }
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        var Data = GetComponent<SoulWarriorData>();
        var AI = GetComponent<SoulWarriorAI>();

        CurrentTakenAttack = (CharacterAttackInfo)Attack;

        if (CurrentTakenAttack.InterruptLevel > 0 && GetComponent<SoulWarriorAI>().CurrentState != SoulWarriorState.SlashStrike && GetComponent<SoulWarriorAI>().CurrentState != SoulWarriorState.MagicStrike)
        {
            Interrupted = true;

            if (Data.OffBalanceInterruptLevel <= CurrentTakenAttack.InterruptLevel || AI.CurrentState != SoulWarriorState.SlashAnticipation)
            {
                OffBalance = true;
            }
            else
            {
                OffBalance = false;
            }

        }
        else
        {
            Interrupted = false;
            OffBalance = false;
        }

        int Damage = Utility.GetEffectValue(CurrentTakenAttack.Power, CurrentTakenAttack.Potency);

        CurrentHP -= Damage;

        SetHPFill((float)CurrentHP / Data.MaxHP);


        if (DamageText == null)
        {
            DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));
        }

        DamageText.GetComponent<DamageText>().ActivateSelf(Damage);

        DamageText.transform.SetParent(Canvas.transform);

        if (CurrentHP <= 0)
        {
            if (DamageText != null)
            {
                DamageText.transform.SetParent(SharedCanvas.transform);
            }
            EventManager.instance.Fire(new PlayerKillEnemy(CurrentTakenAttack, gameObject));
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
