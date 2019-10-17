using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;

    public bool Invulnerable;
    public bool Blocking;
    public GameObject InvulnerableEffect;

    public Vector2 BarDefaultSize;
    public Vector2 BarInflateSize;

    public GameObject Canvas;
    public GameObject HPFill;
    public GameObject EnergyMarks;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

    private GameObject DamageText;

    private float TimeCount;

    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<CharacterData>();
        CurrentHP = Data.MaxHP;
        CurrentEnergy = 0;
    }

    // Update is called once per frame
    void Update()
    {

        SetFill();
    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;

        int count = 0;

        foreach(Transform child in EnergyMarks.transform)
        {
            if (CurrentEnergy > count)
            {
                child.GetComponent<Image>().enabled = true;
            }
            else
            {
                child.GetComponent<Image>().enabled = false;
            }
            count++;
        }

        //EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / Data.MaxEnergy;
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);
        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

        EnemyAttackInfo HitAttack = (EnemyAttackInfo)Attack;

        int Damage = HitAttack.Damage;

        if (Invulnerable)
        {
            Damage = 0;
            Interrupted = false;
            CurrentEnergy++;
        }
        else if (Blocking)
        {
            if(HitAttack.Right && transform.right.x < 0 || !HitAttack.Right && transform.right.x > 0)
            {
                Damage = Mathf.RoundToInt((1 - GetComponent<CharacterData>().BlockDamageDeduction) * Damage);
                Interrupted = false;
            }
            else
            {
                CurrentEnergy = 0;
                Interrupted = true;
            }
        }
        else
        {
            CurrentEnergy = 0;
            Interrupted = true;
        }


        CurrentHP -= Damage;

        if (HitAttack.Right)
        {
            DamageText.GetComponent<DamageText>().TravelVector = new Vector2(1, 1);
        }
        else
        {
            DamageText.GetComponent<DamageText>().TravelVector = new Vector2(-1, 1);
        }
        DamageText.GetComponent<Text>().text = Damage.ToString();
        DamageText.transform.parent = Canvas.transform;


        if(CurrentHP <= 0)
        {
            SceneManager.LoadScene(0);
            return true;
        }
        else
        {
            return false;
        }
    }
}
