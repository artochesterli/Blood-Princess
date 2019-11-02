using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface ISkillSlot
{
    SkillSlotState State { get; set; }
    bool IsEnhancementSlot { get; set; }
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

public enum SkillPanelState
{
    SelectSlot,
    SelectEnhancement,
    SelectPassiveSkill
}

public class SkillPanelManager : MonoBehaviour
{
    public SkillPanelState State;
    public List<GameObject> SlotList;
    public List<GameObject> EnhancementSelectionList;
    public List<GameObject> PassiveSkillSelectionList;
    public GameObject SkillPointText;

    public int SkillPoint;

    public int EquipCost;
    public int Lv2Cost;
    public int Lv3Cost;

    private int SelectedSlot;
    private int SelectedEnhancement;
    private int SelectedPassiveSkill;

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
        if (Utility.InputUpgrade() && State == SkillPanelState.SelectSlot)
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
        if (Utility.InputDowngrade() && State == SkillPanelState.SelectSlot)
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
            switch (State)
            {
                case SkillPanelState.SelectEnhancement:
                    EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().Selected = false;
                    EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().SetAppearance();
                    SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetState(SkillSlotState.Hovered);
                    SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
                    break;
                case SkillPanelState.SelectPassiveSkill:
                    PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().Selected = false;
                    PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().SetAppearance();
                    SlotList[SelectedSlot].GetComponent<ISkillSlot>().State = SkillSlotState.Hovered;
                    SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
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
            case SkillPanelState.SelectEnhancement:

                EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().Selected = false;
                EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().SetAppearance();

                SelectedEnhancement = (SelectedEnhancement + value) % EnhancementSelectionList.Count;
                if (SelectedEnhancement < 0)
                {
                    SelectedEnhancement += EnhancementSelectionList.Count;
                }

                EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().Selected = true;
                EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().SetAppearance();
                break;
            case SkillPanelState.SelectPassiveSkill:
                PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().Selected = false;
                PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().SetAppearance();

                SelectedPassiveSkill = (SelectedPassiveSkill + value) % PassiveSkillSelectionList.Count;
                if(SelectedPassiveSkill < 0)
                {
                    SelectedPassiveSkill += PassiveSkillSelectionList.Count;
                }

                PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().Selected = true;
                PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().SetAppearance();
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

                    if (SlotList[SelectedSlot].GetComponent<ISkillSlot>().IsEnhancementSlot)
                    {
                        State = SkillPanelState.SelectEnhancement;
                        SelectedEnhancement = 0;
                        EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().Selected = true;
                        EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().SetAppearance();
                    }
                    else
                    {
                        State = SkillPanelState.SelectPassiveSkill;
                        SelectedPassiveSkill = 0;
                        PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().Selected = true;
                        PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().SetAppearance();
                    }

                    break;
                case SkillPanelState.SelectEnhancement:

                    if (SkillPoint >= EquipCost)
                    {
                        SkillPoint -= EquipCost;
                        SetSkillPoint();

                        State = SkillPanelState.SelectSlot;

                        EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().Selected = false;
                        EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().SetAppearance();

                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetState(SkillSlotState.Hovered);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().Equip(EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>().Enhancement);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
                    }

                    break;
                case SkillPanelState.SelectPassiveSkill:

                    if (SkillPoint >= EquipCost)
                    {
                        SkillPoint -= EquipCost;
                        SetSkillPoint();

                        State = SkillPanelState.SelectSlot;

                        PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().Selected = false;
                        PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().SetAppearance();

                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetState(SkillSlotState.Hovered);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetAppearance();
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().Equip(PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().PassiveAbility);
                        SlotList[SelectedSlot].GetComponent<ISkillSlot>().SetSlot();
                    }
                    break;
            }
        }
    }

}
