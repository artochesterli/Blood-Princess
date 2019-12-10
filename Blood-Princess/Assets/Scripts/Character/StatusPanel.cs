using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatusPanel : MonoBehaviour
{
    public GameObject HPInfo;
    public GameObject PowerInfo;

    public GameObject BattleArtInfo;
    public GameObject PassiveAbilityInfo;

    private void OnEnable()
    {
        SetPanel();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Utility.InputCancel(ControlState.CheckStatus) || Utility.InputOpenStatusPanel(ControlState.CheckStatus))
        {
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }
    }

    private void SetPanel()
    {

        GameObject Player = CharacterOpenInfo.Self;

        var Info = Player.GetComponent<CharacterAbilitiesInfo>();

        HPInfo.GetComponent<Text>().text = "HP: " + Player.GetComponent<StatusManager_Character>().CurrentHP.ToString() + "/" + Player.GetComponent<StatusManager_Character>().CurrentMaxHP.ToString();
        PowerInfo.GetComponent<Text>().text = "Power: " +Player.GetComponent<StatusManager_Character>().CurrentPower.ToString();

        GameObject BattleArtSlot = BattleArtInfo.transform.GetChild(0).gameObject;

        BattleArt battleArt = Player.GetComponent<CharacterAction>().EquipedBattleArt;

        if (battleArt != null)
        {
            BattleArtSlot.GetComponent<Text>().text = battleArt.name + "(Lv" + battleArt.Level + ")";
            BattleArtSlot.transform.Find("Icon").GetComponent<Image>().sprite = battleArt.Icon;

            for(int i = 1; i <= battleArt.Level; i++)
            {
                BattleArtSlot.transform.Find("Description").GetComponent<Text>().text += "-" + battleArt.Description[i - 1];
                if(i < battleArt.Level - 1)
                {
                    BattleArtSlot.transform.Find("Description").GetComponent<Text>().text += "\n";
                }
            }
        }
        else
        {
            BattleArtSlot.GetComponent<Text>().text = "Empty";

            BattleArtSlot.transform.Find("Icon").GetComponent<Image>().sprite = null;
            BattleArtSlot.transform.Find("Description").GetComponent<Text>().text = "";
        }

        
        for(int i = 0; i < PassiveAbilityInfo.transform.childCount; i++)
        {
            GameObject PassiveAbilitySlot = PassiveAbilityInfo.transform.GetChild(i).gameObject;

            PassiveAbility passiveAbility = Player.GetComponent<CharacterAction>().EquipedPassiveAbility[i];
            if(passiveAbility != null)
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

    }
}
