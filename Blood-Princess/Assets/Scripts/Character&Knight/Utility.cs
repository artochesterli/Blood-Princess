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

    public static bool InputRight()
    {
        //return Input.GetKey(KeyCode.D);
        return Input.GetKey(KeyCode.RightArrow) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("HorizontalMove") > StickAvailableThreshold);
    }

    public static bool InputLeft()
    {
        //return Input.GetKey(KeyCode.A);
        return Input.GetKey(KeyCode.LeftArrow) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("HorizontalMove") < -StickAvailableThreshold);
    }

    public static bool InputUp()
    {
        //return Input.GetKey(KeyCode.W);
        return Input.GetKey(KeyCode.UpArrow) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("VerticalMove") > StickAvailableThreshold);
    }

    public static bool InputDown()
    {
        //return Input.GetKey(KeyCode.S);
        return Input.GetKey(KeyCode.DownArrow) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("VerticalMove") < -StickAvailableThreshold);
    }

    public static bool InputJump()
    {
        return Input.GetKeyDown(KeyCode.Space) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Jump"));
    }

    public static bool InputJumpHold()
    {
        return Input.GetKey(KeyCode.Space) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButton("Jump"));
    }

    public static bool InputNormalSlash()
    {
        //return Input.GetKeyDown(KeyCode.J);
        return Input.GetKeyDown(KeyCode.S) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("NormalSlash"));
    }

    public static bool InputFirstSkill()
    {
        return Input.GetKeyDown(KeyCode.D) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("FirstSkill"));
    }

    public static bool InputSecondSkill()
    {
        return Input.GetKeyDown(KeyCode.F) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("SecondSkill"));
    }

    public static bool InputRoll()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Roll"));
    }
}
