using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Right,
    Left,
    Top,
    Bottom
}

public class SpeedManager : MonoBehaviour
{
    public LayerMask IgnoredLayers;

    public Vector2 SelfSpeed;
    public Vector2 ForcedSpeed;
    public Vector2 AttackStepSpeed;

    public bool HitRight;
    public float RightDis;
    public GameObject Right;
    public float RightForceFieldDis;
    public GameObject RightForceField;

    public bool HitLeft;
    public float LeftDis;
    public GameObject Left;
    public float LeftForceFieldDis;
    public GameObject LeftForceField;

    public bool HitTop;
    public float TopDis;
    public GameObject Top;
    public float TopForceFieldDis;
    public GameObject TopForceField;

    public bool HitGround;
    public float GroundDis;
    public GameObject Ground;
    public float GroundForceFieldDis;
    public GameObject GroundForceField;

    public float BodyWidth;
    public float BodyHeight;

    public Vector2 OriPos;

    public bool MoveExecuted;

    private const float DetectDis = 1f;
    private const float HitMargin = 0.01f;
    private const float CastBoxThickness = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        MoveExecuted = false;

        CheckGroundDis();
        CheckLeftWallDis();
        CheckRightWallDis();
        CheckTopDis();

        //RectifySpeed();
        Move();

        MoveExecuted = true;
    }

    

    /*private void RectifySpeed()
    {
        if (HitRight && Speed.x > 0 || HitLeft && Speed.x < 0)
        {
            Speed.x = 0;
        }
        if (HitTop && Speed.y > 0 || HitGround && Speed.y < 0)
        {
            Speed.y = 0;
        }
    }*/

    public Vector2 GetTruePos()
    {
        if (transform.right.x > 0)
        {
            return transform.position + (Vector3)OriPos;
        }
        else
        {
            Vector2 Offset = OriPos;
            Offset.x = -Offset.x;
            return transform.position + (Vector3)Offset;
        }
    }

    public void SetBodyInfo(Vector2 Offset,Vector2 BodySize)
    {
        OriPos = Offset;
        BodyWidth = BodySize.x;
        BodyHeight = BodySize.y;
        GetComponent<BoxCollider2D>().offset = Offset;
        GetComponent<BoxCollider2D>().size = BodySize;
    }
    

    private void CheckPushedOut()
    {
        if (CompareTag("Player"))
        {
            var Data = GetComponent<CharacterData>();


            Vector2 temp = AttackStepSpeed;

            if (temp.x > 0)
            {
                if(RightForceFieldDis < temp.x * Time.deltaTime)
                {
                    if (RightForceFieldDis > 0)
                    {
                        temp.x = RightForceFieldDis / Time.deltaTime;
                    }
                    else
                    {
                        temp.x = 0;
                    }
                    AttackStepSpeed.x = 0;
                }
            }
            else if(temp.x < 0)
            {
                if (LeftForceFieldDis < -temp.x * Time.deltaTime)
                {
                    if(LeftForceFieldDis > 0)
                    {
                        temp.x = LeftForceFieldDis / Time.deltaTime;
                    }
                    else
                    {
                        temp.x = 0;
                    }

                    AttackStepSpeed.x = 0;
                }
            }

            bool RightStuckForceField = false;
            if(RightForceField && RightForceFieldDis < 0)
            {
                RightStuckForceField = true;
            }

            bool LeftStuckForceField = false;
            if(LeftForceField && LeftForceFieldDis < 0)
            {
                LeftStuckForceField = true;
            }

            if(!RightStuckForceField && !LeftStuckForceField)
            {
                ForcedSpeed.x = 0;
                return;
            }

            float ReferenceX = 0;
            int count = 0;

            if (RightStuckForceField)
            {
                if (RightForceField.GetComponent<SpeedManager>())
                {
                    ReferenceX += RightForceField.GetComponent<SpeedManager>().GetTruePos().x;
                }
                else
                {
                    ReferenceX += RightForceField.transform.position.x;
                }
                count++;
            }

            if (LeftStuckForceField)
            {
                if (LeftForceField.GetComponent<SpeedManager>())
                {
                    ReferenceX += LeftForceField.GetComponent<SpeedManager>().GetTruePos().x;
                }
                else
                {
                    ReferenceX += LeftForceField.transform.position.x;
                }
                count++;
            }

            ReferenceX /= count;

            if(GetTruePos().x > ReferenceX && SelfSpeed.x <= 0 && LeftStuckForceField)
            {
                ForcedSpeed.x = Data.PushedOutSpeed;
            }
            else if(GetTruePos().x < ReferenceX && SelfSpeed.x >= 0 && RightStuckForceField)
            {
                ForcedSpeed.x = -Data.PushedOutSpeed;
            }
            else
            {
                ForcedSpeed.x = 0;
            }

        }
    }

    private void Move()
    {
        CheckPushedOut();

        Vector2 temp = SelfSpeed + ForcedSpeed + AttackStepSpeed;

        if (temp.y >= 0 && Top)
        {
            ColliderType Type = Top.GetComponent<ColliderInfo>().Type;

            if (TopDis < temp.y * Time.deltaTime)
            {
                if (TopDis > 0)
                {
                    temp.y = TopDis / Time.deltaTime;
                    ResetAllSpeed(false, true);
                    HitTop = true;
                }
                else if (Type == ColliderType.Solid || TopDis > -HitMargin)
                {
                    temp.y = 0;
                    ResetAllSpeed(false, true);
                    HitTop = true;
                }
                else
                {
                    HitTop = false;
                }
            }
            else
            {
                HitTop = false;
            }
        }
        else
        {
            HitTop = false;
        }

        if (temp.y <= 0 && Ground)
        {
            ColliderType Type = Ground.GetComponent<ColliderInfo>().Type;

            if (GroundDis < -temp.y * Time.deltaTime)
            {
                if(GroundDis > 0)
                {
                    temp.y = -GroundDis / Time.deltaTime;
                    ResetAllSpeed(false, true);
                    HitGround = true;
                }
                else if(Type == ColliderType.Solid || GroundDis > -HitMargin)
                {
                    temp.y = 0;
                    ResetAllSpeed(false, true);
                    HitGround = true;
                }
                else
                {
                    HitGround = false;
                }

            }
            else
            {
                HitGround = false;
            }
        }
        else
        {
            HitGround = false;
        }

        if (temp.x <= 0 && Left)
        {
            ColliderType Type = Left.GetComponent<ColliderInfo>().Type;

            if (LeftDis < -temp.x * Time.deltaTime)
            {
                if (LeftDis > 0)
                {
                    temp.x = -LeftDis / Time.deltaTime;
                    ResetAllSpeed(true, false);
                    HitLeft = true;
                }
                else if (Type == ColliderType.Solid || LeftDis > -HitMargin)
                {
                    temp.x = 0;
                    ResetAllSpeed(true, false);
                    HitLeft = true;
                }
                else
                {
                    HitLeft = false;
                }

            }
            else
            {
                HitLeft = false;

            }
        }
        else
        {
            HitLeft = false;
        }

        if (temp.x >= 0 && Right)
        {
            ColliderType Type = Right.GetComponent<ColliderInfo>().Type;

            if (RightDis < temp.x * Time.deltaTime)
            {
                if (RightDis > 0)
                {
                    temp.x = RightDis / Time.deltaTime;
                    ResetAllSpeed(true, false);
                    HitRight = true;
                }
                else if (Type == ColliderType.Solid || RightDis > -HitMargin)
                {
                    temp.x = 0;
                    ResetAllSpeed(true, false);
                    HitRight = true;
                }
                else
                {
                    HitRight = false;
                }

            }
            else
            {
                HitRight = false;
            }
        }
        else
        {
            HitRight = false;
        }

        transform.position += (Vector3)temp* Time.deltaTime;
    }

    public void CheckGroundDis()
    {

        RaycastHit2D[] HitList= Physics2D.BoxCastAll(GetTruePos() + (BodyHeight / 2 + DetectDis / 2) * Vector2.down, new Vector2(BodyWidth - 2 * HitMargin, DetectDis), 0, Vector2.down, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(true, HitList, Direction.Bottom);
        RaycastHit2D forcefieldhit = GetClosestHit(false, HitList, Direction.Bottom);

        if (hit.collider!=null)
        {
            GroundDis = GetHitDis(hit, Direction.Bottom);
            Ground = hit.collider.gameObject;
        }
        else
        {
            GroundDis = Mathf.Infinity;
            Ground = null;
        }

        if(forcefieldhit.collider != null)
        {
            GroundForceFieldDis = GetHitDis(forcefieldhit, Direction.Bottom);
            GroundForceField = forcefieldhit.collider.gameObject;
        }
        else
        {
            GroundForceFieldDis = Mathf.Infinity;
            GroundForceField = null;
        }

    }

    public void CheckTopDis()
    {

        RaycastHit2D[] HitList= Physics2D.BoxCastAll(GetTruePos() + (DetectDis / 2 + BodyHeight / 2) * Vector2.up, new Vector2(BodyWidth - 2 * HitMargin, DetectDis), 0, Vector2.up, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(true, HitList, Direction.Top);
        RaycastHit2D forcefieldhit = GetClosestHit(false, HitList, Direction.Top);

        if (hit.collider!=null)
        {
            TopDis = GetHitDis(hit, Direction.Top);
            Top = hit.collider.gameObject;
        }
        else
        {
            TopDis = Mathf.Infinity;
            Top = null;
        }

        if (forcefieldhit.collider != null)
        {
            TopForceFieldDis = GetHitDis(forcefieldhit, Direction.Top);
            TopForceField = forcefieldhit.collider.gameObject;
        }
        else
        {
            TopForceFieldDis = Mathf.Infinity;
            TopForceField = null;
        }
    }

    public void CheckLeftWallDis()
    {

        RaycastHit2D[] HitList = Physics2D.BoxCastAll(GetTruePos() + (DetectDis/2 + BodyWidth / 2)*Vector2.left, new Vector2(DetectDis, BodyHeight - 2 * HitMargin), 0, Vector2.left, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(true, HitList, Direction.Left);
        RaycastHit2D forcefieldhit = GetClosestHit(false, HitList, Direction.Left);

        if (hit.collider!=null)
        {
            LeftDis = GetHitDis(hit, Direction.Left);
            Left = hit.collider.gameObject;
        }
        else
        {
            LeftDis = Mathf.Infinity;
            Left = null;
        }

        if (forcefieldhit.collider != null)
        {
            LeftForceFieldDis = GetHitDis(forcefieldhit, Direction.Left);
            LeftForceField = forcefieldhit.collider.gameObject;
        }
        else
        {
            LeftForceFieldDis = Mathf.Infinity;
            LeftForceField = null;
        }
    }


    public void CheckRightWallDis()
    {
        RaycastHit2D[] HitList = Physics2D.BoxCastAll(GetTruePos() + (DetectDis / 2 + BodyWidth / 2) * Vector2.right, new Vector2(DetectDis, BodyHeight - 2 * HitMargin), 0, Vector2.right, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(true, HitList, Direction.Right);
        RaycastHit2D forcefieldhit = GetClosestHit(false, HitList, Direction.Right);

        if (hit.collider!=null)
        {
            RightDis = GetHitDis(hit, Direction.Right);
            Right = hit.collider.gameObject;
        }
        else
        {
            RightDis = Mathf.Infinity;
            Right = null;
        }

        if (forcefieldhit.collider != null)
        {
            RightForceFieldDis = GetHitDis(forcefieldhit, Direction.Right);
            RightForceField = forcefieldhit.collider.gameObject;
        }
        else
        {
            RightForceFieldDis = Mathf.Infinity;
            RightForceField = null;
        }
    }

    private RaycastHit2D GetClosestHit(bool ReturnSolid, RaycastHit2D[] HitList,Direction Dir)
    {
        float MinDis = Mathf.Infinity;
        float MinForceFieldDis = Mathf.Infinity;

        RaycastHit2D Hit=new RaycastHit2D();
        RaycastHit2D ForceFieldHit = new RaycastHit2D();

        for(int i = 0; i < HitList.Length; i++)
        {
            float Dis = GetHitDis(HitList[i], Dir);

            if (Dis < MinDis)
            {
                var Info = HitList[i].collider.gameObject.GetComponent<ColliderInfo>();

                switch (Dir)
                {
                    case Direction.Right:

                        if(Info.Type == ColliderType.ForceField && !Info.LeftPassable)
                        {
                            MinForceFieldDis = Dis;
                            ForceFieldHit = HitList[i];
                        }
                        else if(!Info.LeftPassable)
                        {
                            MinDis = Dis;
                            Hit = HitList[i];
                        }

                        break;

                    case Direction.Left:
                        if(Info.Type == ColliderType.ForceField && !Info.RightPassable)
                        {
                            MinForceFieldDis = Dis;
                            ForceFieldHit = HitList[i];
                        }
                        else if (!Info.RightPassable)
                        {
                            MinDis = Dis;
                            Hit = HitList[i];
                        }
                        break;
                    case Direction.Top:

                        if (Info.Type == ColliderType.ForceField && !Info.BottomPassable)
                        {
                            MinForceFieldDis = Dis;
                            ForceFieldHit = HitList[i];
                        }
                        else if (!Info.BottomPassable)
                        {
                            MinDis = Dis;
                            Hit = HitList[i];
                        }
                        break;
                    case Direction.Bottom:
                        if (Info.Type == ColliderType.ForceField && !Info.TopPassable)
                        {
                            MinForceFieldDis = Dis;
                            ForceFieldHit = HitList[i];
                        }
                        else if (!Info.TopPassable)
                        {
                            MinDis = Dis;
                            Hit = HitList[i];
                        }
                        break;
                }
            }

        }
        if (ReturnSolid)
        {
            return Hit;
        }
        else
        {
            return ForceFieldHit;
        }

    }

    private float GetHitDis(RaycastHit2D hit,Direction Dir)
    {
        float Dis = 0;

        GameObject Obj = hit.collider.gameObject;

        switch (Dir)
        {
            case Direction.Right:
                Dis = (Obj.transform.position.x + Obj.GetComponent<BoxCollider2D>().offset.x - Obj.GetComponent<BoxCollider2D>().size.x / 2 * Obj.transform.localScale.x) - (GetTruePos().x + BodyWidth / 2) ;
                break;
            case Direction.Left:
                Dis = (GetTruePos().x - BodyWidth / 2) - (Obj.transform.position.x + Obj.GetComponent<BoxCollider2D>().offset.x+ Obj.GetComponent<BoxCollider2D>().size.x / 2 * Obj.transform.localScale.x);
                break;
            case Direction.Top:
                Dis = (Obj.transform.position.y + Obj.GetComponent<BoxCollider2D>().offset.y - Obj.GetComponent<BoxCollider2D>().size.y / 2 * Obj.transform.localScale.y) - (GetTruePos().y + BodyHeight / 2);
                break;
            case Direction.Bottom:
                Dis = (GetTruePos().y - BodyHeight / 2) - (Obj.transform.position.y + Obj.GetComponent<BoxCollider2D>().offset.y + Obj.GetComponent<BoxCollider2D>().size.y / 2 * Obj.transform.localScale.y);
                break;
        }

        if(hit.collider.gameObject.GetComponent<SpeedManager>() && !hit.collider.gameObject.GetComponent<SpeedManager>().MoveExecuted)
        {
            switch (Dir)
            {
                case Direction.Right:
                    Dis -= Obj.GetComponent<SpeedManager>().SelfSpeed.x * Time.deltaTime;
                    break;
                case Direction.Left:
                    Dis -= Obj.GetComponent<SpeedManager>().SelfSpeed.x * Time.deltaTime;
                    break;
                case Direction.Top:
                    Dis -= Obj.GetComponent<SpeedManager>().SelfSpeed.y * Time.deltaTime;
                    break;
                case Direction.Bottom:
                    Dis -= Obj.GetComponent<SpeedManager>().SelfSpeed.y * Time.deltaTime;
                    break;
            }
        }

        return Dis;
    }

    private bool ConverseHit(GameObject Obj)
    {
        if(Obj && Obj.GetComponent<SpeedManager>())
        {
            return !(Obj.GetComponent<SpeedManager>().IgnoredLayers == (Obj.GetComponent<SpeedManager>().IgnoredLayers | (1 << gameObject.layer)));
        }
        else
        {
            return false;
        }
    }

    private void ResetAllSpeed(bool x, bool y)
    {
        if (x)
        {
            SelfSpeed.x = 0;
            ForcedSpeed.x = 0;
            AttackStepSpeed.x = 0;
        }

        if (y)
        {
            SelfSpeed.y = 0;
            ForcedSpeed.y = 0;
            AttackStepSpeed.y = 0;
        }

    }
}
