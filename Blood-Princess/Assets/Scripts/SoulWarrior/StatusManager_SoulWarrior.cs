using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_SoulWarrior : StatusManagerBase, IHittable
{
    public GameObject Canvas;
    public GameObject SharedCanvas;
    public GameObject HPFill;

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
        
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);

        var Data = GetComponent<SoulWarriorData>();

        CharacterAttackInfo HitAttack = (CharacterAttackInfo)Attack;


        Interrupted = true;


        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));


        GetComponent<SoulWarriorAI>().Player = HitAttack.Source;


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
