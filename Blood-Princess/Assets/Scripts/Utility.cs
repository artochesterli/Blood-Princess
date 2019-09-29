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
        return Input.GetKey(KeyCode.RightArrow);
    }

    public static bool InputLeft()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }

    public static bool InputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public static bool InputJumpHold()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public static bool InputLightAttack()
    {
        return Input.GetKeyDown(KeyCode.Z);
    }

    public static bool InputHeavyAttack()
    {
        return Input.GetKeyDown(KeyCode.X);
    }

    public static bool InputDrain()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }
}
