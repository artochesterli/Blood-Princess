using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodSlashEnhancementSlotManager : MonoBehaviour,ISkillSlot
{
    public bool IsEnhancementSlot { get; set; }
    public SkillSlotState State { get; set; }

    public GameObject Player;


    public BattleArtEnhancement EquipedEnhancement;

    public int index;
    public Color UnselectedColor;
    public Color HoveredColor;
    public Color SelectedColor;
    // Start is called before the first frame update
    void Start()
    {
        IsEnhancementSlot = true;
        GetAbility();
        SetSlot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSlot()
    {

        if (EquipedEnhancement.Type != BattleArtEnhancementType.Null)
        {
            GetComponent<Text>().text = EquipedEnhancement.name + "(" + EquipedEnhancement.Level + ")";
        }
        else
        {
            GetComponent<Text>().text = "Empty";
        }
    }

    private void GetAbility()
    {
        EquipedEnhancement = Player.GetComponent<CharacterAction>().BloodSlashEnhancements[index];
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

    public void Equip(CharacterAbility Ability)
    {
        EquipedEnhancement = (BattleArtEnhancement)Ability;
        EquipedEnhancement.EnhancementAttackType = CharacterAttackType.BloodSlash;

        Player.GetComponent<CharacterAction>().BloodSlashEnhancements[index] = EquipedEnhancement;
    }
    public void UpgradeAbility()
    {
        EquipedEnhancement.Level++;
    }
    public void DowngradeAbility()
    {
        EquipedEnhancement.Level--;
    }
    public void RemoveAbility()
    {
        EquipedEnhancement = new BattleArtEnhancement("", CharacterAttackType.BloodSlash, BattleArtEnhancementType.Null, 0);
        Player.GetComponent<CharacterAction>().BloodSlashEnhancements[index] = new BattleArtEnhancement("", CharacterAttackType.BloodSlash, BattleArtEnhancementType.Null, 0);
    }
    public bool IsAbilityEquiped()
    {
        return EquipedEnhancement.Type != BattleArtEnhancementType.Null;
    }
    public int GetAbilityLevel()
    {
        return EquipedEnhancement.Level;
    }

    public void SetState(SkillSlotState state)
    {
        State = state;
    }
}
