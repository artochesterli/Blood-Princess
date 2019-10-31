using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillSlotType
{
    BloodSlash,
    DeadSlash,
    Passive
}

public enum SkillSlotState
{
    Unselected,
    Hovered,
    Selected
}

public class SkillSlotManager : MonoBehaviour
{
    public SkillSlotState State;
    public SkillSlotType Type;
    public int index;
    public CharacterAbility EquipedAbility;

    public Color UnselectedColor;
    public Color HoveredColor;
    public Color SelectedColor;

    private void OnEnable()
    {
        GetAbility();
        SetSlot();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSlot()
    {
        bool Empty = false;

        BattleArtEnhancement B;
        CharacterPassiveAbility C;


        switch (Type)
        {

            case SkillSlotType.BloodSlash:
                B = (BattleArtEnhancement)EquipedAbility;
                Empty = B.Type == BattleArtEnhancementType.Null;
                break;
            case SkillSlotType.DeadSlash:
                B = (BattleArtEnhancement)EquipedAbility;
                Empty = B.Type == BattleArtEnhancementType.Null;
                break;
            case SkillSlotType.Passive:
                C = (CharacterPassiveAbility)EquipedAbility;
                Empty = C.Type == CharacterPassiveAbilityType.Null;
                break;
        }

        if (!Empty)
        {
            GetComponent<Text>().text = EquipedAbility.name + "(" + EquipedAbility.Level + ")";
        }
        else
        {
            GetComponent<Text>().text = "Empty";
        }
    }

    private void GetAbility()
    {
        switch (Type)
        {
            case SkillSlotType.BloodSlash:
                EquipedAbility = transform.root.GetComponent<CharacterAction>().BloodSlashEnhancements[index];
                break;
            case SkillSlotType.DeadSlash:
                EquipedAbility = transform.root.GetComponent<CharacterAction>().DeadSlashEnhancements[index];
                break;
            case SkillSlotType.Passive:
                EquipedAbility = transform.root.GetComponent<CharacterAction>().EquipedPassiveAbilities[index];
                break;
        }
    }

    public void SetAppearance()
    {
        switch (State)
        {
            case SkillSlotState.Selected:
                GetComponent<Text>().color = SelectedColor;
                break;
            case SkillSlotState.Unselected:
                GetComponent<Text>().color = UnselectedColor;
                break;
            case SkillSlotState.Hovered:
                GetComponent<Text>().color = HoveredColor;
                break;
        }


    }
}
