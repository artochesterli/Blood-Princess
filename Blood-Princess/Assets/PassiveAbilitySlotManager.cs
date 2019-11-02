using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveAbilitySlotManager : MonoBehaviour, ISkillSlot
{
    public bool IsEnhancementSlot { get; set; }

    public SkillSlotState State { get; set; }

    public GameObject Player;
    public CharacterPassiveAbility EquipedPassiveAbility;

    public int index;
    public Color UnselectedColor;
    public Color HoveredColor;
    public Color SelectedColor;

    // Start is called before the first frame update
    void Start()
    {
        IsEnhancementSlot = false;
        GetAbility();
        SetSlot();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSlot()
    {

        if (EquipedPassiveAbility.Type != CharacterPassiveAbilityType.Null)
        {
            GetComponent<Text>().text = EquipedPassiveAbility.name + "(" + EquipedPassiveAbility.Level + ")";
        }
        else
        {
            GetComponent<Text>().text = "Empty";
        }
    }

    private void GetAbility()
    {
        EquipedPassiveAbility = Player.GetComponent<CharacterAction>().EquipedPassiveAbilities[index];
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
        EquipedPassiveAbility = (CharacterPassiveAbility)Ability;
        Player.GetComponent<CharacterAction>().EquipedPassiveAbilities[index] = EquipedPassiveAbility;
    }
    public void UpgradeAbility()
    {
        EquipedPassiveAbility.Level++;
    }
    public void DowngradeAbility()
    {
        EquipedPassiveAbility.Level--;
    }
    public void RemoveAbility()
    {
        EquipedPassiveAbility = new CharacterPassiveAbility("", CharacterPassiveAbilityType.Null, 0);
        Player.GetComponent<CharacterAction>().EquipedPassiveAbilities[index] = new CharacterPassiveAbility("", CharacterPassiveAbilityType.Null, 0);
    }
    public bool IsAbilityEquiped()
    {
        return EquipedPassiveAbility.Type != CharacterPassiveAbilityType.Null;
    }
    public int GetAbilityLevel()
    {
        return EquipedPassiveAbility.Level;
    }
    public void SetState(SkillSlotState state)
    {
        State = state;
    }
}
