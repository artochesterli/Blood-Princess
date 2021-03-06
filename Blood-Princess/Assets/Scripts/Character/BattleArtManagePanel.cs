﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleArtPanelOpenMode
{
    PickUp,
    Upgrade
}

public class BattleArtManagePanel : MonoBehaviour
{
    public GameObject CurrentBattleArtInfo;
    public GameObject UpdateBattleArtInfo;

    public BattleArt UpdatedBattleArt;


    public GameObject ConfirmGuide;
    public GameObject Title;

    public BattleArtPanelOpenMode Mode;

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
        if (Utility.InputComfirm(ControlState.ReplaceBattleArt) && TransactionAvailable)
        {
            if (Mode == BattleArtPanelOpenMode.PickUp)
            {
                Equip();
            }
            else
            {
                Upgrade();
            }
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }

        if (Utility.InputCancel(ControlState.ReplaceBattleArt))
        {
            CharacterOpenInfo.Self.GetComponent<ControlStateManager>().AttachedUpgradeMerchant = null;
            UpdatedBattleArt = null;
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }
    }

    private void Equip()
    {
        GameObject Player = CharacterOpenInfo.Self;
        BattleArt Current = Player.GetComponent<CharacterAction>().EquipedBattleArt;

        if(Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().PriceType == AbilityObjectPriceType.Purchase)
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


        EventManager.instance.Fire(new PlayerEquipBattleArt(UpdatedBattleArt));
    }

    private void Upgrade()
    {
        GameObject Player = CharacterOpenInfo.Self;
        BattleArt Current = Player.GetComponent<CharacterAction>().EquipedBattleArt;

        EventManager.instance.Fire(new PlayerGetMoney(-Player.GetComponent<ControlStateManager>().AttachedUpgradeMerchant.GetComponent<UpgradeMerchant>().Price));

        Current.Level++;
    }

    public void SetPanel()
    {
        GameObject Player = CharacterOpenInfo.Self;

        BattleArt Current = Player.GetComponent<CharacterAction>().EquipedBattleArt;

        if(Mode == BattleArtPanelOpenMode.PickUp)
        {
            Title.GetComponent<Text>().text = "Equip Battle Art";
        }
        else
        {
            Title.GetComponent<Text>().text = "Upgrade Battle Art";
        }

        if (Current != null)
        {
            if(Mode == BattleArtPanelOpenMode.PickUp)
            {
                ConfirmGuide.GetComponent<Text>().text = "Equip";
                if (Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().PriceType == AbilityObjectPriceType.Purchase)
                {
                    ConfirmGuide.GetComponent<Text>().text += "(" + Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Price + ")";
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
                    ConfirmGuide.GetComponent<Text>().color = Color.white;
                    TransactionAvailable = true;
                }
            }
            else
            {
                ConfirmGuide.GetComponent<Text>().text = "Upgrade" + "(" + Player.GetComponent<ControlStateManager>().AttachedUpgradeMerchant.GetComponent<UpgradeMerchant>().Price.ToString() + ")";
                if(Player.GetComponent<ControlStateManager>().AttachedUpgradeMerchant.GetComponent<UpgradeMerchant>().Price > Player.GetComponent<StatusManager_Character>().CoinAmount)
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

            ConfirmGuide.GetComponent<Text>().text += ":A";


            CurrentBattleArtInfo.GetComponent<Text>().text = Current.name + "(" + Current.Level.ToString() + ")";
            CurrentBattleArtInfo.transform.Find("Icon").GetComponent<Image>().sprite = Current.Icon;

            CurrentBattleArtInfo.transform.Find("Description").GetComponent<Text>().text = "";

            for (int i = 1; i <= Current.Level; i++)
            {


                CurrentBattleArtInfo.transform.Find("Description").GetComponent<Text>().text += "-" + Current.Description[i - 1];
                if (i < Current.Level - 1)
                {
                    CurrentBattleArtInfo.transform.Find("Description").GetComponent<Text>().text += "\n";
                }
            }
        }
        else
        {
            ConfirmGuide.GetComponent<Text>().text = "Equip";
            if (Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().PriceType == AbilityObjectPriceType.Purchase)
            {
                ConfirmGuide.GetComponent<Text>().text += "(" + Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Price + ")" + ":A";
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
                ConfirmGuide.GetComponent<Text>().text += ":A";
                ConfirmGuide.GetComponent<Text>().color = Color.white;
                TransactionAvailable = true;
            }

            CurrentBattleArtInfo.GetComponent<Text>().text = "Empty";
            CurrentBattleArtInfo.transform.Find("Icon").GetComponent<Image>().sprite = null;
            CurrentBattleArtInfo.transform.Find("Description").GetComponent<Text>().text = "";
        }


        if (Mode == BattleArtPanelOpenMode.PickUp)
        {
            UpdateBattleArtInfo.GetComponent<Text>().text = UpdatedBattleArt.name + "(" + UpdatedBattleArt.Level.ToString() + ")";
        }
        else
        {
            UpdateBattleArtInfo.GetComponent<Text>().text = UpdatedBattleArt.name + "(" + (UpdatedBattleArt.Level + 1).ToString() + ")";
        }
        UpdateBattleArtInfo.transform.Find("Icon").GetComponent<Image>().sprite = UpdatedBattleArt.Icon;


        UpdateBattleArtInfo.transform.Find("Description").GetComponent<Text>().text = "";

        for (int i = 1; i <= UpdatedBattleArt.Level; i++)
        {
            UpdateBattleArtInfo.transform.Find("Description").GetComponent<Text>().text += "-" + UpdatedBattleArt.Description[i - 1];
            if(i<UpdatedBattleArt.Level)
            {
                UpdateBattleArtInfo.transform.Find("Description").GetComponent<Text>().text += "\n";

            }
        }

        
    }

    private void OnPlayerPickUpAbility(PlayerPickUpAbility e)
    {
        if(e.Ability.GetType() == typeof(BattleArt))
        {
            UpdatedBattleArt = (BattleArt)e.Ability;
        }
    }


}
