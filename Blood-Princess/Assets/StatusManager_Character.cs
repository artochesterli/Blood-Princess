using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;
    public int CurrentEnergyOrb;
    public bool Drain;

    public Vector2 BarDefaultSize;
    public Vector2 BarInflateSize;

    public GameObject Canvas;
    public GameObject HPFill;
    public GameObject EnergyFill;
    public GameObject EnergyOrbs;
    public GameObject DrainMark;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

    private GameObject DamageText;
    private bool NearDeath;

    private bool BarInflating;
    private float TimeCount;

    // Start is called before the first frame update
    void Start()
    {
        var Data = GetComponent<CharacterData>();
        CurrentHP = Data.MaxHP;
        CurrentEnergy = 0;
        CurrentEnergyOrb = 0;
    }

    // Update is called once per frame
    void Update()
    {
        NearDeath = CurrentHP - CurrentEnergy <= 0;
        if (NearDeath)
        {
            TimeCount += Time.deltaTime;
            if (BarInflating)
            {
                HPFill.transform.parent.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(BarDefaultSize, BarInflateSize, TimeCount / 0.1f);
                EnergyFill.transform.parent.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(BarDefaultSize, BarInflateSize, TimeCount / 0.1f);
            }
            else
            {
                HPFill.transform.parent.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(BarInflateSize, BarDefaultSize,  TimeCount / 0.1f);
                EnergyFill.transform.parent.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(BarInflateSize, BarDefaultSize, TimeCount / 0.1f);
            }
            if (TimeCount >= 0.1f)
            {
                TimeCount = 0;
                BarInflating = !BarInflating;
            }

        }
        else
        {
            HPFill.transform.parent.GetComponent<RectTransform>().sizeDelta = BarDefaultSize;
        }

        SetFill();
    }

    private void SetFill()
    {
        Canvas.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;
        EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / Data.MaxEnergy;
    }

    public override bool OnHit(AttackInfo Attack)
    {
        base.OnHit(Attack);
        DamageText = (GameObject)Instantiate(Resources.Load("Prefabs/DamageText"), transform.localPosition, Quaternion.Euler(0, 0, 0));

        EnemyAttackInfo HitAttack = (EnemyAttackInfo)Attack;
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

        if (NearDeath)
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
