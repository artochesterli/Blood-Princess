using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PCG;

public class Utility
{
    private const float StickAvailableThreshold = 0.9f;

    public static float GetConstraintValue(float Value, float Min, float Max)
    {
        float Ans = Value;

        if (Value > Max)
        {
            Ans = Max;
        }

        if (Value < Min)
        {
            Ans = Min;
        }

        return Ans;
    }



    public static bool InputOpenCloseSkillPanel()
    {
        return Input.GetKeyDown(KeyCode.P);
    }

    public static bool InputUpgrade()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.U);
    }

    public static bool InputDowngrade()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.J);
    }

    public static bool InputRemove()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.L);
    }

    public static bool InputSelectBack()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool InputComfirm()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.Space);
    }

    public static bool InputSelectUp()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.W);
    }

    public static bool InputSelectDown()
    {
        if (ControlStateManager.CurrentControlState == ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.S);
    }

    public static bool InputRight()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }

        return Input.GetKey(KeyCode.D) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("HorizontalMove") > StickAvailableThreshold);
    }

    public static bool InputLeft()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKey(KeyCode.A) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("HorizontalMove") < -StickAvailableThreshold);
    }

    public static bool InputUp()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKey(KeyCode.W) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("VerticalMove") > StickAvailableThreshold);
    }

    public static bool InputDown()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKey(KeyCode.S) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("VerticalMove") < -StickAvailableThreshold);
    }

    public static bool InputJump()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.Space) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Jump"));
    }

    public static bool InputJumpHold()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKey(KeyCode.Space) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButton("Jump"));
    }

    public static bool InputNormalSlash()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetMouseButtonDown(0) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Slash"));
    }

    public static bool InputBattleArt()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }

        return Input.GetMouseButtonDown(1) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("BattleArt"));

    }

    public static bool InputRoll()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.LeftControl)|| (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Roll"));
    }

    public static bool InputParry()
    {
        if (ControlStateManager.CurrentControlState == ControlState.SkillManagement)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.Q) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Parry"));
    }
}

[System.Serializable]
public class ItemDatium
{
    public string Name;
    public Sprite Sprite;
    public float Interval;
    public float VitalityDrain = 10f;
    public float RestoreVitality = 10f;
    public float RestoreOxygen = 0f;
    public GameObject PlantedSeed;
}
