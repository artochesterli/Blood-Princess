using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleArtManagePanel : MonoBehaviour
{
    public GameObject CurrentBattleArtInfo;
    public GameObject UpdateBattleArtInfo;

    public BattleArt UpdatedBattleArt;

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
        if (Utility.InputComfirm(ControlState.ReplaceBattleArt))
        {
            Equip();
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }

        if (Utility.InputCancel(ControlState.ReplaceBattleArt))
        {
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }
    }

    private void Equip()
    {
        GameObject Player = CharacterOpenInfo.Self;
        BattleArt Current = Player.GetComponent<CharacterAction>().EquipedBattleArt;

        if (Current != null)
        {
            Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().Ability = Current;
            Player.GetComponent<ControlStateManager>().AttachedAbilityObject.GetComponent<AbilityObject>().SetSelf();
        }
        else
        {
            Destroy(Player.GetComponent<ControlStateManager>().AttachedAbilityObject);
        }


        EventManager.instance.Fire(new PlayerEquipBattleArt(UpdatedBattleArt));
    }

    public void SetPanel()
    {
        GameObject Player = CharacterOpenInfo.Self;

        BattleArt Current = Player.GetComponent<CharacterAction>().EquipedBattleArt;

        if (Current != null)
        {
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
            CurrentBattleArtInfo.GetComponent<Text>().text = "Empty";
            CurrentBattleArtInfo.transform.Find("Icon").GetComponent<Image>().sprite = null;
            CurrentBattleArtInfo.transform.Find("Description").GetComponent<Text>().text = "";
        }


        UpdateBattleArtInfo.GetComponent<Text>().text = UpdatedBattleArt.name +"("+UpdatedBattleArt.Level.ToString()+")";
        UpdateBattleArtInfo.transform.Find("Icon").GetComponent<Image>().sprite = UpdatedBattleArt.Icon;


        UpdateBattleArtInfo.transform.Find("Description").GetComponent<Text>().text = "";

        for (int i = 1; i <= UpdatedBattleArt.Level; i++)
        {
            UpdateBattleArtInfo.transform.Find("Description").GetComponent<Text>().text += "-" + UpdatedBattleArt.Description[i - 1];
            if(i<UpdatedBattleArt.Level - 1)
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
