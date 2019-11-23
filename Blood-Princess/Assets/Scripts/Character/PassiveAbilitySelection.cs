using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveAbilitySelection : MonoBehaviour
{
    public PassiveAbilityType Type;
    public PassiveAbility Ability;
    public bool Selected { get; set; }

    public Color UnselectedColor;
    public Color SelectedColor;
    // Start is called before the first frame update
    void Start()
    {
        switch (Type)
        {
            case PassiveAbilityType.SlashArt:
                Ability = new SlashArt();
                break;
            case PassiveAbilityType.AssassinHeart:
                Ability = new AssassinHeart();
                break;
            case PassiveAbilityType.Dancer:
                Ability = new Dancer();
                break;
            case PassiveAbilityType.OneMind:
                Ability = new OneMind();
                break;
            case PassiveAbilityType.Insanity:
                Ability = new Insanity();
                break;
            case PassiveAbilityType.StepMaster:
                Ability = new StepMaster();
                break;
            case PassiveAbilityType.SpellStrike:
                Ability = new SpellStrike();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAppearance()
    {
        if (Selected)
        {
            GetComponent<Text>().color = SelectedColor;
        }
        else
        {
            GetComponent<Text>().color = UnselectedColor;
        }
    }
}
