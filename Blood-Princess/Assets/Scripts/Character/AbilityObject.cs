using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AbilityObjectType
{
    OnlyBattleArt,
    OnlyPassiveAbility,
    All
}

public enum AbilityObjectPriceType
{
    Purchase,
    Drop
}

public class AbilityObject : MonoBehaviour
{
    public AbilityObjectType Type;
    public AbilityObjectPriceType PriceType;
    public int Price;

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
        if (GetComponent<SpeedManager>().HitGround)
        {
            GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    private void RandomAbility()
    {
        var Data = CharacterOpenInfo.Self.GetComponent<CharacterData>();

        int value;


        switch (Type)
        {
            case AbilityObjectType.OnlyBattleArt:
                value = Random.Range(0, Data.BattleArtTypeNumber);
                break;
            case AbilityObjectType.OnlyPassiveAbility:
                value = Random.Range(Data.BattleArtTypeNumber, Data.BattleArtTypeNumber + Data.PassiveAbilityTypeNumber);
                break;
            case AbilityObjectType.All:
                value = Random.Range(0, Data.BattleArtTypeNumber + Data.PassiveAbilityTypeNumber);
                break;
            default:
                value = Random.Range(0, Data.BattleArtTypeNumber + Data.PassiveAbilityTypeNumber);
                break;
        }




        if (value < Data.BattleArtTypeNumber)
        {
            value = 0;
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
                    Ability = new CriticalEye();
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

        if (PriceType == AbilityObjectPriceType.Drop)
        {
            Text.GetComponent<Text>().text = "LB: Pick up\n" + Ability.name;
        }
        else
        {
            Text.GetComponent<Text>().text = "LB: Purchase\n" + Ability.name;
        }

        if(Ability.GetType().BaseType == typeof(BattleArt))
        {
            Text.GetComponent<Text>().text += "(Lv" + Ability.Level + ")";
        }
    }

    private void DetectPlayer()
    {
        var SpeedManager = GetComponent<SpeedManager>();

        RaycastHit2D Hit = Physics2D.BoxCast(SpeedManager.GetTruePos(), new Vector2(SpeedManager.BodyWidth, SpeedManager.BodyHeight), 0, Vector2.down,0,PlayerLayer);

        if(Hit.collider != null)
        {
            if (Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAbilityObject == null || Hit.collider.gameObject.GetComponent<ControlStateManager>().AttachedAbilityObject == gameObject)
            {
                Text.SetActive(true);
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
            else
            {
                Text.SetActive(false);
            }
        }
        else
        {
            if(CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedAbilityObject == gameObject)
            {
                CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedAbilityObject = null;
            }

            Text.SetActive(false);
        }
    }

    
}
