using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface ISkillSlot
{
    SkillSlotState State { get; set; }
    AbilityType Type { get; set; }
    void Equip(CharacterAbility Ability);
    void UpgradeAbility();
    void DowngradeAbility();
    void RemoveAbility();
    bool IsAbilityEquiped();
    int GetAbilityLevel();
    void SetState(SkillSlotState state);
    void SetSlot();
    void SetAppearance();
}

interface ISkillSelection
{
    bool Selected { get; set; }
    void SetAppearance();
}

public enum SkillSlotState
{
    Unselected,
    Hovered,
    Selected
}

public enum AbilityType
{
    BattleArt,
    PassiveAbility
}

public enum SkillPanelState
{
    SelectSlot,
    SelectBattleArt,
    SelectPassiveAbility
}

public class SkillPanelManager : MonoBehaviour
{
    public SkillPanelState State;
    public List<GameObject> SlotList;
    public List<GameObject> BattleArtSelectionList;
    public List<GameObject> PassiveAbilitySelectionList;
    public GameObject SkillPointText;

    public int SkillPoint;

    public int EquipCost;
    public int Lv2Cost;
    public int Lv3Cost;

    private int SelectedSlot;
    private int SelectedBattleArt;
    private int SelectedPassiveAbility;

    // Start is called before the first frame update
    void Start()
    {
        Init();

    }

    // Update is called once per frame
    void Update()
    {
        Confirm();

        SelectBack();

        CheckUpgrade();
        CheckDowngrade();
        CheckRemove();

        if (Utility.InputSelectUp())
        {
            MoveSelection(true);
        }

        if (Utility.InputSelectDown())
        {
            MoveSelection(false);
        }

    }

    private void Init()
    {
        SelectedSlot = 0;
        for(int i = 0; i < SlotList.Count; i++)
        {
            if (i == SelectedSlot)
            {
                SlotList[i].GetComponent<ISkillSlot>().SetState(SkillSlotState.Hovered);
            }
            else
            {
                SlotList[i].GetComponent<ISkillSlot>().SetState(SkillSlotState.Unselected);
            }
            SlotList[i].GetComponent<ISkillSlot>().SetAppearance();
        }
        SetSkillPoint();

    }

    private void SetSkillPoint()
    {
        SkillPointText.GetComponent<Text>().text = "Skill Points: " + SkillPoint.ToString();
    }

    private void CheckUpgrade()
    {
        if (Utility.InputUpgrade() && State == SkillPanelState.SelectSlot && SlotList[SelectedSlot].GetComponent<ISkillSlot>().Type == AbilityType.BattleArt)
        {
            bool AbleToUpgrade = SlotList[SelectedSlot].GetComponent<ISkillSlot>().IsAbilityEquiped();
            int Level = SlotList[SelectedSlot].GetComponent<ISkillSlot>().GetAbilityLevel();

            int SkillPointCost = 0;
            switch (Level)
            {
                case 1:
                    SkillPointCost = Lv2Cost;
                    break;
                case 2:
                    SkillPointCost = Lv3Cost;
                    break;
                case 3:
                    AbleToUpgrade = false;
                    break;
            }

            if (AbleToUpgrade && SkillPoint >= SkillPointCost)
            {
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().UpgradeAbility();
                SkillPoint -= SkillPointCost;
                SetSkillPoint();
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
            }
        }
    }

    private void CheckDowngrade()
    {
        if (Utility.InputDowngrade() && State == SkillPanelState.SelectSlot && SlotList[SelectedSlot].GetComponent<ISkillSlot>().Type == AbilityType.BattleArt)
        {
            bool AbleToDowngrade = SlotList[SelectedSlot].GetComponent<ISkillSlot>().IsAbilityEquiped();
            int Level = SlotList[SelectedSlot].GetComponent<ISkillSlot>().GetAbilityLevel();

            int SkillPointGain = 0;
            switch (Level)
            {
                case 3:
                    SkillPointGain = Lv3Cost;
                    break;
                case 2:
                    SkillPointGain = Lv2Cost;
                    break;
                case 1:
                    SkillPointGain = EquipCost;
                    break;
            }

            if (AbleToDowngrade)
            {
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().DowngradeAbility();
                SkillPoint += SkillPointGain;
                SetSkillPoint();
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
            }
        }
    }

    private void CheckRemove()
    {
        if (Utility.InputRemove() && State == SkillPanelState.SelectSlot)
        {
            bool AbleToRemove = SlotList[SelectedSlot].GetComponent<ISkillSlot>().IsAbilityEquiped();
            int Level = SlotList[SelectedSlot].GetComponent<ISkillSlot>().GetAbilityLevel();

            int SkillPointGain = 0;
            switch (Level)
            {
                case 3:
                    SkillPointGain = Lv3Cost + Lv2Cost + EquipCost;
                    break;
                case 2:
                    SkillPointGain = Lv2Cost + EquipCost;
                    break;
                case 1:
                    SkillPointGain = EquipCost;
                    break;
            }
            if (AbleToRemove)
            {
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().RemoveAbility();
                SkillPoint += SkillPointGain;
                SetSkillPoint();
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
            }

        }
    }


    private void SelectBack()
    {
        if (Utility.InputSelectBack())
        {
            SlotList[SelectedSlot].GetComponent<ISkillSlot>().State = SkillSlotState.Hovered;
            SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
            switch (State)
            {
                case SkillPanelState.SelectBattleArt:
                    BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().Selected = false;
                    BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().SetAppearance();
                    break;
                case SkillPanelState.SelectPassiveAbility:
                    PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().Selected = false;
                    PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().SetAppearance();
                    break;
            }
            State = SkillPanelState.SelectSlot;
        }
    }

    private void MoveSelection(bool up)
    {
        int value = 0;
        if (up)
        {
            value = -1;
        }
        else
        {
            value = 1;
        }

        switch (State)
        {
            case SkillPanelState.SelectSlot:
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().State = SkillSlotState.Unselected;
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();

                SelectedSlot = (SelectedSlot + value) % SlotList.Count;
                if (SelectedSlot<0)
                {
                    SelectedSlot += SlotList.Count;
                }

                SlotList[SelectedSlot].GetComponent<ISkillSlot>().State = SkillSlotState.Hovered;
                SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
                break;

            case SkillPanelState.SelectBattleArt:

                BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().Selected = false;
                BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().SetAppearance();

                SelectedBattleArt = (SelectedBattleArt + value) % BattleArtSelectionList.Count;
                if (SelectedBattleArt < 0)
                {
                    SelectedBattleArt += BattleArtSelectionList.Count;
                }

                BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().Selected = true;
                BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().SetAppearance();
                break;

            case SkillPanelState.SelectPassiveAbility:
                PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().Selected = false;
                PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().SetAppearance();

                SelectedPassiveAbility = (SelectedPassiveAbility + value) % PassiveAbilitySelectionList.Count;
                if(SelectedPassiveAbility < 0)
                {
                    SelectedPassiveAbility += PassiveAbilitySelectionList.Count;
                }

                PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().Selected = true;
                PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().SetAppearance();
                break;
        }
    }



    private void Confirm()
    {
        if (Utility.InputComfirm())
        {
            switch (State)
            {
                case SkillPanelState.SelectSlot:

                    SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetState(SkillSlotState.Selected);
                    SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();

                    if (SlotList[SelectedSlot].GetComponent<ISkillSlot>().Type == AbilityType.BattleArt)
                    {
                        State = SkillPanelState.SelectBattleArt;
                        SelectedBattleArt = 0;
                        BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().Selected = true;
                        BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().SetAppearance();
                    }
                    else
                    {
                        State = SkillPanelState.SelectPassiveAbility;
                        SelectedPassiveAbility = 0;
                        PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().Selected = true;
                        PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().SetAppearance();
                    }

                    break;
                case SkillPanelState.SelectBattleArt:

                    if (SkillPoint >= EquipCost && !SlotList[SelectedSlot].GetComponent<ISkillSlot>().IsAbilityEquiped())
                    {
                        SkillPoint -= EquipCost;
                        SetSkillPoint();

                        State = SkillPanelState.SelectSlot;

                        BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().Selected = false;
                        BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().SetAppearance();

                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetState(SkillSlotState.Hovered);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();

                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().Equip(BattleArtSelectionList[SelectedBattleArt].GetComponent<BattleArtSelection>().Ability);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
                    }

                    break;
                case SkillPanelState.SelectPassiveAbility:

                    if (SkillPoint >= EquipCost && !SlotList[SelectedSlot].GetComponent<ISkillSlot>().IsAbilityEquiped())
                    {
                        SkillPoint -= EquipCost;
                        SetSkillPoint();

                        State = SkillPanelState.SelectSlot;

                        PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().Selected = false;
                        PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().SetAppearance();

                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetState(SkillSlotState.Hovered);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().Equip(PassiveAbilitySelectionList[SelectedPassiveAbility].GetComponent<PassiveAbilitySelection>().Ability);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
                    }
                    break;
            }
        }
    }


}
