using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityObject : MonoBehaviour
{
    public CharacterAbility Ability;


    // Start is called before the first frame update
    void Start()
    {
        RandomAbility();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RandomAbility()
    {
        var Data = CharacterOpenInfo.Self.GetComponent<CharacterData>();
        int value = Random.Range(0, Data.BattleArtTypeNumber + Data.PassiveAbilityTypeNumber);

        if(value < Data.BattleArtTypeNumber)
        {
            BattleArtType Type = (BattleArtType)value;
            switch (Type)
            {
                case BattleArtType.PowerSlash:
                    Ability = new PowerSlash();
                    break;
                case BattleArtType.CrossSlash:
                    Ability = new CrossSlash();
                    break;
            }
        }
        else
        {
            PassiveAbilityType Type = (PassiveAbilityType)(value - Data.BattleArtTypeNumber);
            switch (Type)
            {
                case PassiveAbilityType.Harmony:
                    Ability = new Harmony();
                    break;
                case PassiveAbilityType.SpiritMaster:
                    Ability = new SpiritMaster();
                    break;
                case PassiveAbilityType.UltimateAwakening:
                    Ability = new UltimateAwakening();
                    break;
                case PassiveAbilityType.CriticalEye:
                    Ability = new CriticalEye();
                    break;
                case PassiveAbilityType.BattleArtMaster:
                    Ability = new BattleArtMaster();
                    break;
                case PassiveAbilityType.Dancer:
                    Ability = new Dancer();
                    break;
                case PassiveAbilityType.OneMind:
                    Ability = new OneMind();
                    break;
            }
        }
    }
}
