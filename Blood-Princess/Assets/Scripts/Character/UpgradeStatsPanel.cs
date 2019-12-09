using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStatsPanel : MonoBehaviour
{
    public GameObject CurrentHPInfo;
    public GameObject CurrentPowerInfo;

    public GameObject UpgradeInfo;

    public float HPY;
    public float PowerY;

    private bool UpgradeHP;

    // Start is called before the first frame update
    void Start()
    {
        UpgradeHP = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Utility.InputSelectUp(ControlState.UpgradeStats))
        {
            UpgradeHP = !UpgradeHP;
            SetPanel();
        }

        if (Utility.InputSelectDown(ControlState.UpgradeStats))
        {
            UpgradeHP = !UpgradeHP;
            SetPanel();
        }

        if (Utility.InputComfirm(ControlState.UpgradeStats))
        {
            Upgrade();
            ControlStateManager.CurrentControlState = ControlState.Action;
            gameObject.SetActive(false);
        }
    }



    private void Upgrade()
    {
        var Status = CharacterOpenInfo.Self.GetComponent<StatusManager_Character>();
        var AbilityData = CharacterOpenInfo.Self.GetComponent<CharacterAbilityData>();

        if (UpgradeHP)
        {
            Status.CurrentHP += AbilityData.HPUpgradeAmount;
            Status.MaxHP += AbilityData.HPUpgradeAmount;
        }
        else
        {
            Status.CurrentPower += AbilityData.PowerUpgradeAmount;
        }
    }

    public void SetPanel()
    {
        var Status = CharacterOpenInfo.Self.GetComponent<StatusManager_Character>();
        var Data = CharacterOpenInfo.Self.GetComponent<CharacterData>();
        var AbilityData = CharacterOpenInfo.Self.GetComponent<CharacterAbilityData>();

        CurrentHPInfo.GetComponent<Text>().text = "HP:" + Status.CurrentHP.ToString() + "/" + Status.CurrentMaxHP.ToString();
        CurrentPowerInfo.GetComponent<Text>().text = "Power:" + Status.CurrentPower.ToString();

        Vector3 Pos = UpgradeInfo.GetComponent<RectTransform>().localPosition;

        if (UpgradeHP)
        {
            Pos.y = HPY;
            UpgradeInfo.GetComponent<Text>().text = (Status.CurrentHP + AbilityData.HPUpgradeAmount).ToString() + "/" + (Status.CurrentMaxHP + AbilityData.HPUpgradeAmount).ToString();
        }
        else
        {
            Pos.y = PowerY;
            UpgradeInfo.GetComponent<Text>().text = (Status.CurrentPower + AbilityData.PowerUpgradeAmount).ToString();
        }

        UpgradeInfo.GetComponent<RectTransform>().localPosition = Pos;
    }
}
