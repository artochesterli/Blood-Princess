﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlState
{
    Action,
    ReplaceBattleArt,
    ReplacePassiveAbility,
    UpgradeStats,
    CheckStatus,
    Storage,
    CraftingTable
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
    public GameObject AttachedUpgradeMerchant;
    public GameObject AttachedPotion;

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
                    BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().Mode = BattleArtPanelOpenMode.PickUp;
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

            if(AttachedUpgradeMerchant != null)
            {
                CurrentControlState = ControlState.ReplaceBattleArt;
                BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().UpdatedBattleArt = (BattleArt)(GetComponent<CharacterAction>().EquipedBattleArt);
                BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().Mode = BattleArtPanelOpenMode.Upgrade;
                BattleArtManagerPanel.GetComponent<BattleArtManagePanel>().SetPanel();
                BattleArtManagerPanel.SetActive(true);
            }

            if(AttachedAltar != null)
            {
                CurrentControlState = ControlState.UpgradeStats;
                UpgradeStatsPanel.GetComponent<UpgradeStatsPanel>().SetPanel();
                UpgradeStatsPanel.SetActive(true);
                return;
            }

            if(AttachedPotion != null)
            {
                var Status = GetComponent<StatusManager_Character>();
                Status.Heal(Mathf.RoundToInt(Status.CurrentMaxHP * AttachedPotion.GetComponent<HealingPotion>().Proportion/100.0f));
                Destroy(AttachedPotion);
            }
        }

    }
}
