using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using PCG;
using DG.Tweening;
using TMPro;

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

    public static void ObjectFollow(GameObject Target, GameObject Follower, Vector2 Offset)
    {
        Vector2 TargetPos;

        if (Target.GetComponent<SpeedManager>())
        {
            TargetPos = Target.GetComponent<SpeedManager>().GetTruePos();
        }
        else
        {
            TargetPos = Target.transform.position;
        }

        if (Follower.GetComponent<SpeedManager>())
        {
            Follower.GetComponent<SpeedManager>().MoveToPoint(TargetPos + Offset);
        }
        else
        {
            Follower.transform.position = TargetPos + Offset;
        }
    }


    public static void TurnAround(GameObject obj)
    {
        Vector2 TruePos = obj.GetComponent<SpeedManager>().GetTruePos();
        if (obj.transform.right.x > 0)
        {
            obj.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            obj.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        obj.GetComponent<SpeedManager>().MoveToPoint(TruePos);
    }

    public static void SetAttackHitBox(CharacterAttackInfo Attack, Vector2 Offset, Vector2 HitBoxSize, float TimeCount)
    {
        float CurrentSizeX = Mathf.Lerp(0, HitBoxSize.x, TimeCount / Attack.StrikeTime);
        float CurrentOffsetX = Offset.x - HitBoxSize.x / 2 + CurrentSizeX / 2;

        Attack.HitBoxSize = new Vector2(CurrentSizeX, HitBoxSize.y);
        Attack.HitBoxOffset = new Vector2(CurrentOffsetX, Offset.y);
    }

    public static int GetEffectValue(int Power, int Potency)
    {
        return Mathf.RoundToInt(Power * Potency/100.0f);
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


    public static bool InputOpenStatusPanel(ControlState Current)
    {
        if(ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.H) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Start");
    }

    public static bool InputPickUp()
    {
        if(ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.G) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("PickUp");
    }

    public static bool InputCancel(ControlState Current)
    {
        if (ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.LeftAlt) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Roll");
    }

    public static bool InputComfirm(ControlState Current)
    {
        if(ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.Space) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Jump");
    }

    public static bool InputSelectUp(ControlState Current)
    {
        if (ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.W) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("UpArrow");
    }

    public static bool InputSelectDown(ControlState Current)
    {
        if(ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.S) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("DownArrow");
    }

    public static bool InputSelectLeft(ControlState Current)
    {
        if (ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.A) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("LeftArrow");
    }

    public static bool InputSelectRight(ControlState Current)
    {
        if (ControlStateManager.CurrentControlState != Current)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.D) || ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("RightArrow");
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
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }

        return Input.GetKey(KeyCode.D) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("HorizontalMove") > StickAvailableThreshold);
    }

    public static bool InputLeft()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKey(KeyCode.A) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("HorizontalMove") < -StickAvailableThreshold);
    }

    public static bool InputUp()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKey(KeyCode.W) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("VerticalMove") > StickAvailableThreshold);
    }

    public static bool InputDown()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKey(KeyCode.S) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetAxis("VerticalMove") < -StickAvailableThreshold);
    }

    public static bool InputJump()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.Space) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Jump"));
    }

    public static bool InputJumpHold()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKey(KeyCode.Space) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButton("Jump"));
    }

    public static bool InputNormalSlash()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.J) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Slash"));
    }

    public static bool InputBattleArt()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }

        return Input.GetKeyDown(KeyCode.I)|| (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("BattleArt"));

    }

    public static bool InputRoll()
    {
        if (ControlStateManager.CurrentControlState != ControlState.Action)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.L) || (ControllerManager.CharacterJoystick != null && ControllerManager.Character.GetButtonDown("Roll"));
    }
}
