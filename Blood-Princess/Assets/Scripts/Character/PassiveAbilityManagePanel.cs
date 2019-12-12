using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveAbilityManagePanel : MonoBehaviour
{
    public GameObject CurrentPassiveAbilityInfo;
    public GameObject UpdatePassiveAbilityInfo;

    public PassiveAbility UpdatePassiveAbility;

    public GameObject ConfirmGuide;

    public float BaseY;
    public float YInterval;

    private int CurrentSlot;

    private bool TransactionAvailable;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDestroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Utility.InputSelectUp(ControlState.ReplacePassiveAbility))
        {
            CurrentSlot--;
            if (CurrentSlot < 0)
            {
                CurrentSlot += CharacterOpenInfo.Self.GetComponent<CharacterAction>().EquipedPassiveAbility.Count;
            }
            SetPanel();
        }

        if (Utility.InputSelectDown(ControlState.ReplacePassiveAbility))
        {
            CurrentSlot++;
            if(CurrentSlot >= CharacterOpenInfo.Self.GetComponent<CharacterAction>().EquipedPassiveAbility.Count)
            {
                CurrentSlot -= CharacterOpenInfo.Self.GetComponent<CharacterAction>().EquipedPassiveAbility.Count;
            }
            SetPanel();
        }

        if (Utility.InputComfirm(ControlState.ReplacePassiveAbility) && TransactionAvailable)
        {
            Equip();
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }

        if (Utility.InputCancel(ControlState.ReplacePassiveAbility))
        {
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }
    }

    private void Equip()
    {
        GameObject Player = CharacterOpenInfo.Self;

        PassiveAbility Current = Player.GetComponent<CharacterAction>().EquipedPassiveAbility[CurrentSlot];

        if (Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().PriceType == AbilityObjectPriceType.Purchase)
        {
            EventManager.instance.Fire(new PlayerGetMoney(-Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Price));
        }

        if (Current != null)
        {
            Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Ability = Current;
            Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().PriceType = AbilityObjectPriceType.Drop;
            Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().SetSelf();
        }
        else
        {
            Destroy(Player.GetComponent<ControlStateManager>().AttachedAbilityObject);
        }

        EventManager.instance.Fire(new PlayerEquipPassiveAbility(UpdatePassiveAbility,CurrentSlot));
    }

    public void SetPanel()
    {
        GameObject Player = CharacterOpenInfo.Self;

        if(Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().PriceType == AbilityObjectPriceType.Purchase)
        {
            ConfirmGuide.GetComponent<Text>().text = "Equip" + "(" + Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Price.ToString() + ")" + ":A";

            if (Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Price > Player.GetComponent<StatusManager_Character>().CoinAmount)
            {
                ConfirmGuide.GetComponent<Text>().color = Color.red;
                TransactionAvailable = false;
            }
            else
            {
                ConfirmGuide.GetComponent<Text>().color = Color.white;
                TransactionAvailable = true;
            }
        }
        else
        {
            ConfirmGuide.GetComponent<Text>().text = "Equip:A";
            ConfirmGuide.GetComponent<Text>().color = Color.white;
            TransactionAvailable = true;
        }

        for (int i = 0; i < CurrentPassiveAbilityInfo.transform.childCount; i++)
        {
            GameObject PassiveAbilitySlot = CurrentPassiveAbilityInfo.transform.GetChild(i).gameObject;

            PassiveAbility passiveAbility = Player.GetComponent<CharacterAction>().EquipedPassiveAbility[i];
            if (passiveAbility != null)
            {
                PassiveAbilitySlot.GetComponent<Text>().text = passiveAbility.name;
                PassiveAbilitySlot.transform.Find("Icon").GetComponent<Image>().sprite = passiveAbility.Icon;
                PassiveAbilitySlot.transform.Find("Description").GetComponent<Text>().text = passiveAbility.Description;
            }
            else
            {
                PassiveAbilitySlot.GetComponent<Text>().text = "Empty";
                PassiveAbilitySlot.transform.Find("Icon").GetComponent<Image>().sprite = null;
                PassiveAbilitySlot.transform.Find("Description").GetComponent<Text>().text = "";
            }
        }

        UpdatePassiveAbilityInfo.GetComponent<Text>().text = UpdatePassiveAbility.name;
        UpdatePassiveAbilityInfo.transform.Find("Icon").GetComponent<Image>().sprite = UpdatePassiveAbility.Icon;
        UpdatePassiveAbilityInfo.transform.Find("Description").GetComponent<Text>().text = UpdatePassiveAbility.Description;

        /*for (int i = 0; i < Player.GetComponent<CharacterAction>().EquipedPassiveAbility.Count; i++)
        {
            if (Player.GetComponent<CharacterAction>().EquipedPassiveAbility[i] == null)
            {
                CurrentSlot = i;
                return;
            }
        }*/


        Vector3 Pos = UpdatePassiveAbilityInfo.GetComponent<RectTransform>().localPosition;
        Pos.y = BaseY - YInterval * CurrentSlot;

        UpdatePassiveAbilityInfo.GetComponent<RectTransform>().localPosition = Pos;

    }

    private void OnPlayerPickUpAbility(PlayerPickUpAbility e)
    {
        if (e.Ability.GetType() == typeof(PassiveAbility))
        {
            UpdatePassiveAbility = (PassiveAbility)e.Ability;
            GameObject Player = CharacterOpenInfo.Self;

            CurrentSlot = 0;

            for(int i = 0; i < Player.GetComponent<CharacterAction>().EquipedPassiveAbility.Count; i++)
            {
                if (Player.GetComponent<CharacterAction>().EquipedPassiveAbility[i] == null)
                {
                    CurrentSlot = i;
                    return;
                }
            }
        }
    }
}
