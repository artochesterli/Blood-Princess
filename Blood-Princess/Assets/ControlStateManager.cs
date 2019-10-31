using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlState
{
    Action,
    SkillManagement
}

public class ControlStateManager : MonoBehaviour
{
    public static ControlState CurrentControlState;
    public GameObject SkillPanel;

    // Start is called before the first frame update
    void Start()
    {
        CurrentControlState = ControlState.Action;
    }

    // Update is called once per frame
    void Update()
    {
        if (Utility.InputOpenCloseSkillPanel())
        {
            if (CurrentControlState == ControlState.Action)
            {
                CurrentControlState = ControlState.SkillManagement;
                SkillPanel.SetActive(true);
            }
            else
            {
                CurrentControlState = ControlState.Action;
                SkillPanel.SetActive(false);
            }
        }
    }
}
