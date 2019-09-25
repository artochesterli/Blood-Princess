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
}
