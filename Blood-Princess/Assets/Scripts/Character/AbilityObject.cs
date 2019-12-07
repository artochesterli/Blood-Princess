using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityObject : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public CharacterAbility Ability;
    public GameObject Text;

    // Start is called before the first frame update
    void Start()
    {
        RandomAbility();
        SetSelf();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();

    }

    private void RandomAbility()
    {
        var Data = CharacterOpenInfo.Self.GetComponent<CharacterData>();
        int value = Random.Range(0, Data.BattleArtTypeNumber + Data.PassiveAbilityTypeNumber);

        if (value < Data.BattleArtTypeNumber)
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

    public void SetSelf()
    {
        GetComponent<SpriteRenderer>().sprite = Ability.Icon;
        Text.GetComponent<Text>().text = Ability.name;
        if(Ability.GetType() == typeof(BattleArt))
        {
            Text.GetComponent<Text>().text += "(" + Ability.Level + ")";
        }
    }

    private void DetectPlayer()
    {
        var SpeedManager = GetComponent<SpeedManager>();

        RaycastHit2D Hit = Physics2D.BoxCast(SpeedManager.GetTruePos(), new Vector2(SpeedManager.BodyWidth, SpeedManager.BodyHeight), 0, Vector2.down,0,PlayerLayer);

        if(Hit.collider != null)
        {
            Text.SetActive(true);

            if (Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAbilityObject == null)
            {
                Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAbilityObject = gameObject;

                if (Ability.GetType().BaseType == typeof(BattleArt))
                {
                    Hit.collider.gameObject.GetComponent<ControlStateManager>().BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().UpdatedBattleArt = (BattleArt)Ability;
                }
                else
                {
                    Hit.collider.gameObject.GetComponent<ControlStateManager>().PassiveAbilityManagerPanel.GetComponent<PassiveAbilityManagePanel>().UpdatePassiveAbility = (PassiveAbility)Ability;
                }
            }
        }
        else
        {
            Text.SetActive(false);
        }
    }

    
}
