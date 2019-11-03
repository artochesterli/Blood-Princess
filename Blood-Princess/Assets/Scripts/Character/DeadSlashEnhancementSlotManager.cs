using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadSlashEnhancementSlotManager : MonoBehaviour, ISkillSlot
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
        IsEnhancementSlot = transform;
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
        EquipedEnhancement = Player.GetComponent<CharacterAction>().DeadSlashEnhancements[index];
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
        EquipedEnhancement.EnhancementAttackType = CharacterAttackType.DeadSlash;
        Player.GetComponent<CharacterAction>().DeadSlashEnhancements[index] = EquipedEnhancement;
        EventManager.instance.Fire(new PlayerEquipEnhancement(EquipedEnhancement));
    }
    public void UpgradeAbility()
    {
        EquipedEnhancement.Level++;
        EventManager.instance.Fire(new PlayerUpgradeEnhancement(EquipedEnhancement));
    }
    public void DowngradeAbility()
    {
        EquipedEnhancement.Level--;
        EventManager.instance.Fire(new PlayerDowngradeEnhancement(EquipedEnhancement));
    }
    public void RemoveAbility()
    {
        EventManager.instance.Fire(new PlayerUnequipEnhancement(EquipedEnhancement));
        EquipedEnhancement = new BattleArtEnhancement("", CharacterAttackType.DeadSlash, BattleArtEnhancementType.Null, 0);
        Player.GetComponent<CharacterAction>().DeadSlashEnhancements[index] = new BattleArtEnhancement("", CharacterAttackType.DeadSlash, BattleArtEnhancementType.Null, 0);

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
