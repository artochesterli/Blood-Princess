using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, ISkillSlot
{
    public SkillSlotState State { get; set; }
    public AbilityType Type { get; set; }
    public AbilityType ThisType;

    public GameObject Player;
    public int index;
    public CharacterAbility CurrentAbility;

    public Color UnselectedColor;
    public Color SelectedColor;
    public Color HoveredColor;

    // Start is called before the first frame update
    void Start()
    {
        Type = ThisType;
        SetSlot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Equip(CharacterAbility Ability)
    {
        CurrentAbility = Ability;
        switch (Type)
        {
            case AbilityType.BattleArt:
                EventManager.instance.Fire(new PlayerEquipBattleArt((BattleArt)Ability));
                break;
            case AbilityType.PassiveAbility:
                EventManager.instance.Fire(new PlayerEquipPassiveAbility((PassiveAbility)Ability, index));
                break;
        }
    }

    public void UpgradeAbility()
    {
        CurrentAbility.Level++;
    }

    public void DowngradeAbility()
    {
        CurrentAbility.Level--;
        if(CurrentAbility.Level == 0)
        {
            RemoveAbility();
        }
    }

    public void RemoveAbility()
    {
        switch (Type)
        {
            case AbilityType.BattleArt:
                EventManager.instance.Fire(new PlayerUnequipBattleArt((BattleArt)CurrentAbility));
                break;
            case AbilityType.PassiveAbility:
                EventManager.instance.Fire(new PlayerUnequipPassiveAbility((PassiveAbility)CurrentAbility, index));
                break;
        }
        CurrentAbility = null;
    }


    public bool IsAbilityEquiped()
    {
        return CurrentAbility != null;
    }

    public int GetAbilityLevel()
    {
        return CurrentAbility.Level;
    }

    public void SetState(SkillSlotState state)
    {
        State = state;
    }

    public void SetSlot()
    {
        if (CurrentAbility != null)
        {
            GetComponent<Text>().text = CurrentAbility.name;
            if(Type == AbilityType.BattleArt)
            {
                GetComponent<Text>().text += "(" + CurrentAbility.Level + ")";
            }
        }
        else
        {
            GetComponent<Text>().text = "Empty";
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
