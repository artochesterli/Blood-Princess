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
    public bool OffBalance;

    private GameObject DamageText;


    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<KnightData>();
        CurrentTakenAttack = null;
        CurrentMaxHP = Data.MaxHP;
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

        if (AI.CurrentState != KnightState.Strike && CurrentTakenAttack.InterruptLevel > 0)
        {
            Interrupted = true;
            ReceivedInterruptionLevel = CurrentTakenAttack.InterruptLevel;

            if (Data.OffBalanceInterruptLevel <= CurrentTakenAttack.InterruptLevel || CurrentTakenAttack.Dir == Direction.Right && transform.right.x > 0 || CurrentTakenAttack.Dir == Direction.Left && transform.right.x < 0)
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

    public void Heal(int amount)
    {
        if (amount > CurrentMaxHP - CurrentHP)
        {
            amount = CurrentMaxHP - CurrentHP;
        }

        CurrentHP += amount;

        SetHPFill((float)CurrentHP / CurrentMaxHP);
    }
}
