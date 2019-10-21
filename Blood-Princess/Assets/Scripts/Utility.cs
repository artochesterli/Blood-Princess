using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static float GetConstraintValue(float Value, float Min, float Max)
    {
        float Ans = Value;

        if (Value>Max)
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
        return Input.GetKey(KeyCode.D);
        //return Input.GetKey(KeyCode.RightArrow);
    }

    public static bool InputLeft()
    {
        return Input.GetKey(KeyCode.A);
        //return Input.GetKey(KeyCode.LeftArrow);
    }

    public static bool InputUp()
    {
        return Input.GetKey(KeyCode.W);
        //return Input.GetKey(KeyCode.UpArrow);
    }

    public static bool InputDown()
    {
        return Input.GetKey(KeyCode.S);
        //return Input.GetKey(KeyCode.DownArrow);
    }

    public static bool InputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public static bool InputJumpHold()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public static bool InputNormalSlash()
    {
        return Input.GetKeyDown(KeyCode.J);
        //return Input.GetKeyDown(KeyCode.Z);
    }

    public static bool InputBloodSlash()
    {
        return Input.GetKeyDown(KeyCode.I);
        //return Input.GetKeyDown(KeyCode.X);
    }

    public static bool InputDeadSlash()
    {
        return Input.GetKeyDown(KeyCode.L);
        //return Input.GetKeyDown(KeyCode.F);
    }

    public static bool InputBlock()
    {
        return Input.GetKey(KeyCode.LeftAlt);
    }

    public static bool InputBlink()
    {
        return false;
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public static bool InputRoll()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
}
