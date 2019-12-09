using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlState
{
    Action,
    ReplaceBattleArt,
    ReplacePassiveAbility,
    UpgradeStats,
    CheckStatus
}

public class ControlStateManager : MonoBehaviour
{
    public static ControlState CurrentControlState;
    public GameObject StatusPanel;
    public GameObject BattleArtManagerPanel;
    public GameObject PassiveAbilityManagerPanel;
    public GameObject UpgradeStatsPanel;

    public GameObject AttachedAbilityObject;
    public GameObject AttachedAltar;

    // Start is called before the first frame update
    void Start()
    {
        CurrentControlState = ControlState.Action;
    }

    // Update is called once per frame
    void Update()
    {
        if (Utility.InputOpenStatusPanel(ControlState.Action))
        {
            CurrentControlState = ControlState.CheckStatus;
            StatusPanel.SetActive(true);
        }

        if (Utility.InputPickUp())
        {
            if (AttachedAbilityObject != null)
            {
                if (AttachedAbilityObject.GetComponent<AbilityObject>().Ability.GetType().BaseType == typeof(BattleArt))
                {
                    CurrentControlState = ControlState.ReplaceBattleArt;
                    BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().UpdatedBattleArt = (BattleArt)(AttachedAbilityObject.GetComponent<AbilityObject>().Ability);
                    BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().SetPanel();
                    BattleArtManagerPanel.SetActive(true);
                }
                else
                {
                    CurrentControlState = ControlState.ReplacePassiveAbility;
                    PassiveAbilityManagerPanel.GetComponent<PassiveAbilityManagePanel>().UpdatePassiveAbility = (PassiveAbility)(AttachedAbilityObject.GetComponent<AbilityObject>().Ability);
                    PassiveAbilityManagerPanel.GetComponent<PassiveAbilityManagePanel>().SetPanel();
                    PassiveAbilityManagerPanel.SetActive(true);
                }
                return;
            }

            if(AttachedAltar != null)
            {
                CurrentControlState = ControlState.UpgradeStats;
                UpgradeStatsPanel.GetComponent<UpgradeStatsPanel>().SetPanel();
                UpgradeStatsPanel.SetActive(true);
                return;
            }
        }

    }
}
