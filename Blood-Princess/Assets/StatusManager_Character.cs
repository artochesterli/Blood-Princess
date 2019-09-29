using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager_Character : StatusManagerBase, IHittable
{
    public int CurrentEnergy;
    public int CurrentEnergyOrb;

    public GameObject HPFill;
    public GameObject EnergyFill;
    public GameObject EnergyOrbs;

    public Sprite EnergyOrbEmptySprite;
    public Sprite EnergyOrbFilledSprite;

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
        SetFill();
    }

    private void SetFill()
    {
        var Data = GetComponent<CharacterData>();
        HPFill.GetComponent<Image>().fillAmount = (float)CurrentHP / Data.MaxHP;
        EnergyFill.GetComponent<Image>().fillAmount = (float)CurrentEnergy / Data.MaxEnergy;
    }

    public override bool OnHit(AttackInfo Attack)
    {
        return base.OnHit(Attack);
        
    }
}
