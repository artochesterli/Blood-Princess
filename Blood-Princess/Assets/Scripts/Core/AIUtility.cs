using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIUtility
{
    public static bool HitPlayer(Vector2 Pivot, EnemyAttackInfo Attack, LayerMask PlayerLayer)
    {
        Vector2 Offset = Attack.HitBoxOffset;
        Vector2 Direction = Vector2.right;
        if (!Attack.Right)
        {
            Offset.x = -Offset.x;
            Direction = Vector2.left;
        }

        RaycastHit2D Hit = Physics2D.BoxCast(Pivot + Offset, Attack.HitBoxSize, 0, Direction, 0, PlayerLayer);

        if (Hit)
        {
            Hit.collider.gameObject.GetComponent<IHittable>().OnHit(Attack);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool PlayerInDetectRange(GameObject Entity, GameObject Player,float DetectRightX, float DetectLeftX, float DetectHeight, LayerMask DetectLayer, bool CheckDirection)
    {

        Vector2 PlayerTruePos = Player.GetComponent<SpeedManager>().GetTruePos();
        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        if (CheckDirection)
        {
            if (GetXDiff(Player,Entity) > 0 && Entity.transform.right.x < 0 || GetXDiff(Player,Entity) < 0 && Entity.transform.right.x > 0)
            {
                return false;
            }
        }

        if (PlayerTruePos.x >= DetectLeftX && PlayerTruePos.x <= DetectRightX && Mathf.Abs(PlayerTruePos.y - TruePos.y) <= DetectHeight)
        {
            RaycastHit2D hit = Physics2D.Raycast(TruePos, PlayerTruePos - TruePos, (PlayerTruePos - TruePos).magnitude, DetectLayer);
            if (hit && hit.collider.gameObject == Player)
            {
                return true;
            }
        }

        return false;

    }

    public static void CheckPatronStayTime(GameObject Entity, ref float TimeCount, float PatronStayTime, ref bool Moving, bool MovingRight, float MoveSpeed)
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= PatronStayTime)
        {
            TimeCount = 0;
            Moving = true;
            if (MovingRight)
            {
                Entity.transform.rotation = Quaternion.Euler(0, 0, 0);
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = MoveSpeed;
            }
            else
            {
                Entity.transform.rotation = Quaternion.Euler(0, 180, 0);
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -MoveSpeed;
            }
        }
    }


    public static void PatronCheckSelfPos(GameObject Entity, float PatronRightX, float PatronLeftX, ref bool Moving,ref bool MovingRight)
    {
        var Data = Entity.GetComponent<PatronData>();
        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();


        if (MovingRight && TruePos.x >= PatronRightX)
        {
            MovingRight = false;
            Moving = false;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
        else if (!MovingRight && TruePos.x <= PatronLeftX)
        {
            MovingRight = true;
            Moving = false;
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    public static void PatronSetUp(GameObject Entity, ref bool Moving, ref bool MovingRight, float PatronRightX, float PatronLeftX, float MoveSpeed)
    {

        Moving = true;

        Vector2 TruePos = Entity.GetComponent<SpeedManager>().GetTruePos();

        if (TruePos.x > PatronRightX)
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
            MovingRight = false;
        }
        else if (TruePos.x < PatronLeftX)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
            MovingRight = true;
        }
        else
        {
            if (Entity.transform.right.x > 0)
            {
                MovingRight = true;
            }
            else
            {
                MovingRight = false;
            }
        }

        if (MovingRight)
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = MoveSpeed;
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = -MoveSpeed;
        }
    }

    public static void RandomPatronInit(GameObject Entity, float StayTime, float PatronRightX, float PatronLeftX, float MoveSpeed, ref bool Moving, ref bool MovingRight, ref float TimeCount)
    {

        float SelfX = Entity.GetComponent<SpeedManager>().GetTruePos().x;

        float CycleTime = 2 * (PatronRightX - PatronLeftX) / MoveSpeed + 2 * StayTime;

        float time = Random.Range(0, CycleTime);

        float PositionX = SelfX;

        if (time < (PatronRightX - SelfX) / MoveSpeed)
        {
            Moving = true;
            MovingRight = true;
            PositionX = Mathf.Lerp(Entity.transform.position.x, PatronRightX, time / (PatronRightX - SelfX) /MoveSpeed);
        }
        else if (time < (PatronRightX - SelfX) / MoveSpeed + StayTime)
        {
            Moving = false;
            MovingRight = true;
            TimeCount = time - (PatronRightX - SelfX) / MoveSpeed;
            PositionX = PatronRightX;

        }
        else if (time < (PatronRightX - SelfX) / MoveSpeed + StayTime + (PatronRightX - PatronLeftX) / MoveSpeed)
        {
            Moving = true;
            MovingRight = false;
            float CutTime = time - ((PatronRightX - SelfX) / MoveSpeed + StayTime);
            PositionX = Mathf.Lerp(PatronRightX, PatronLeftX, CutTime / (PatronRightX - PatronLeftX) / MoveSpeed);
        }
        else if (time < (PatronRightX - SelfX) / MoveSpeed + 2 * StayTime + (PatronRightX - PatronLeftX) / MoveSpeed)
        {
            Moving = false;
            MovingRight = false;
            TimeCount = time - (PatronRightX - SelfX) / MoveSpeed - StayTime - (PatronRightX - PatronLeftX) / MoveSpeed;
            PositionX = PatronLeftX;
        }
        else
        {
            Moving = true;
            MovingRight = true;
            float CutTime = time - ((PatronRightX - SelfX) / MoveSpeed + 2 * StayTime + (PatronRightX - PatronLeftX) / MoveSpeed);
            PositionX = Mathf.Lerp(PatronLeftX, PatronRightX, CutTime / (PatronRightX - PatronLeftX) / MoveSpeed);
        }

        Entity.transform.position = new Vector2(PositionX - Entity.GetComponent<SpeedManager>().OriPos.x, Entity.transform.position.y);

        if (MovingRight)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Moving)
        {
            if (MovingRight)
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = MoveSpeed;
            }
            else
            {
                Entity.GetComponent<SpeedManager>().SelfSpeed.x = -MoveSpeed;
            }
        }
        else
        {
            Entity.GetComponent<SpeedManager>().SelfSpeed.x = 0;
        }
    }

    public static float GetXDiff(GameObject Player, GameObject Entity)
    {
        return Player.GetComponent<SpeedManager>().GetTruePos().x - Entity.GetComponent<SpeedManager>().GetTruePos().x;
    }

    public static void RectifyDirection(GameObject Player, GameObject Entity)
    {
        if (GetXDiff(Player,Entity) > 0)
        {
            Entity.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Entity.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public static float GetBorderDis(GameObject Player, GameObject Entity)
    {
        var PlayerSpeedManager = Player.GetComponent<SpeedManager>();

        var SelfSpeedManager = Entity.GetComponent<SpeedManager>();

        if (GetXDiff(Player, Entity) > 0)
        {
            return (PlayerSpeedManager.GetTruePos().x - PlayerSpeedManager.BodyWidth / 2) - (SelfSpeedManager.GetTruePos().x + SelfSpeedManager.BodyWidth / 2);
        }
        else
        {
            return (SelfSpeedManager.GetTruePos().x - SelfSpeedManager.BodyWidth / 2) - (PlayerSpeedManager.GetTruePos().x + PlayerSpeedManager.BodyWidth / 2);
        }
    }


}
