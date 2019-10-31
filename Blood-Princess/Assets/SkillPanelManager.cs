using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int SkillPoint;

    private int SelectedSlot;
    private int SelectedEnhancement;
    private int SelectedPassiveSkill;

    // Start is called before the first frame update
    void Start()
    {
        SlotList[SelectedSlot].GetComponent<SkillSlotManager>().State = SkillSlotState.Hovered;
        SlotList[SelectedSlot].GetComponent<SkillSlotManager>().SetAppearance();
    }

    // Update is called once per frame
    void Update()
    {
        Confirm();

        SelectBack();

        if (Utility.InputSelectUp())
        {
            MoveSelection(true);
        }

        if (Utility.InputSelectDown())
        {
            MoveSelection(false);
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
                    SlotList[SelectedSlot].GetComponent<SkillSlotManager>().State = SkillSlotState.Hovered;
                    SlotList[SelectedSlot].GetComponent<SkillSlotManager>().SetAppearance();
                    break;
                case SkillPanelState.SelectPassiveSkill:
                    PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().Selected = false;
                    PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>().SetAppearance();
                    SlotList[SelectedSlot].GetComponent<SkillSlotManager>().State = SkillSlotState.Hovered;
                    SlotList[SelectedSlot].GetComponent<SkillSlotManager>().SetAppearance();
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

        var SlotManager = SlotList[SelectedSlot].GetComponent<SkillSlotManager>();
        var EnhancementSelection = EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>();
        var PassiveSkillSelection = PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>();

        switch (State)
        {
            case SkillPanelState.SelectSlot:
                SlotList[SelectedSlot].GetComponent<SkillSlotManager>().State = SkillSlotState.Unselected;
                SlotList[SelectedSlot].GetComponent<SkillSlotManager>().SetAppearance();

                SelectedSlot = (SelectedSlot + value) % SlotList.Count;
                if (SelectedSlot<0)
                {
                    SelectedSlot += SlotList.Count;
                }

                SlotList[SelectedSlot].GetComponent<SkillSlotManager>().State = SkillSlotState.Hovered;
                SlotList[SelectedSlot].GetComponent<SkillSlotManager>().SetAppearance();
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
            var SlotManager = SlotList[SelectedSlot].GetComponent<SkillSlotManager>();
            var EnhancementSelection = EnhancementSelectionList[SelectedEnhancement].GetComponent<EnhancementSelectionInfo>();
            var PassiveSkillSelection = PassiveSkillSelectionList[SelectedPassiveSkill].GetComponent<PassiveSkillSelectionInfo>();

            switch (State)
            {
                case SkillPanelState.SelectSlot:

                    SlotManager.State = SkillSlotState.Selected;
                    SlotManager.SetAppearance();


                    if (SlotManager.Type != SkillSlotType.Passive)
                    {
                        State = SkillPanelState.SelectEnhancement;
                        SelectedEnhancement = 0;
                        EnhancementSelection.Selected = true;
                        EnhancementSelection.SetAppearance();
                    }
                    else
                    {
                        State = SkillPanelState.SelectPassiveSkill;
                        SelectedPassiveSkill = 0;
                        PassiveSkillSelection.Selected = true;
                        PassiveSkillSelection.SetAppearance();
                    }

                    break;
                case SkillPanelState.SelectEnhancement:
                    State = SkillPanelState.SelectSlot;

                    EnhancementSelection.Selected = false;
                    EnhancementSelection.SetAppearance();

                    SlotManager.State = SkillSlotState.Hovered;
                    SlotManager.SetAppearance();
                    SlotManager.EquipedAbility = EnhancementSelection.Enhancement;
                    SlotManager.SetSlot();

                    EquipAbility(EnhancementSelection.Enhancement, SlotManager.Type, SlotManager.index);

                    break;
                case SkillPanelState.SelectPassiveSkill:
                    State = SkillPanelState.SelectSlot;

                    PassiveSkillSelection.Selected = false;
                    PassiveSkillSelection.SetAppearance();

                    SlotManager.State = SkillSlotState.Hovered;
                    SlotManager.SetAppearance();
                    SlotManager.EquipedAbility = PassiveSkillSelection.PassiveAbility;
                    SlotManager.SetSlot();

                    EquipAbility(PassiveSkillSelection.PassiveAbility, SlotManager.Type, SlotManager.index);
                    break;
            }
        }
    }

    private void EquipAbility(CharacterAbility Ability, SkillSlotType Type, int index)
    {
        var Action = transform.root.GetComponent<CharacterAction>();

        switch (Type)
        {
            case SkillSlotType.BloodSlash:
                Action.BloodSlashEnhancements[index] = (BattleArtEnhancement)Ability;
                break;
            case SkillSlotType.DeadSlash:
                Action.DeadSlashEnhancements[index] = (BattleArtEnhancement)Ability;
                break;
            case SkillSlotType.Passive:
                Action.EquipedPassiveAbilities[index] = (CharacterPassiveAbility)Ability;
                break;
        }
    }


    private void SetPanel()
    {
        
    }
}
