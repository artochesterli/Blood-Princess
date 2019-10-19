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

    public bool HitRight;
    public float RightDis;
    public GameObject Right;

    public bool HitLeft;
    public float LeftDis;
    public GameObject Left;

    public bool HitTop;
    public float TopDis;
    public GameObject Top;

    public bool HitGround;
    public float GroundDis;
    public GameObject Ground;

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
        CheckGroundHitting();
        CheckLeftWallHitting();
        CheckRightWallHitting();
        CheckTopHitting();

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
    

    public void Move()
    {
        Vector2 temp = SelfSpeed;

        if (temp.y > 0)
        {
            if (TopDis < temp.y * Time.deltaTime)
            {
                if (TopDis > 0)
                {
                    temp.y = TopDis / Time.deltaTime;
                }
                else
                {
                    temp.y = 0;
                }
                SelfSpeed.y = 0;
                HitTop = true;
                if (ConverseHit(Top))
                {
                    Top.GetComponent<SpeedManager>().HitGround = true;
                }
            }
            else
            {
                if (Top && Top.GetComponent<SpeedManager>() && Top.GetComponent<SpeedManager>().HitGround)
                {
                    HitTop = true;
                }
                else
                {
                    HitTop = false;
                }
            }
        }

        if (temp.y < 0)
        {
            if (GroundDis < -temp.y * Time.deltaTime)
            {
                if (GroundDis > 0)
                {
                    temp.y = -GroundDis / Time.deltaTime;
                }
                else
                {
                    temp.y = 0;
                }
                SelfSpeed.y = 0;
                HitGround = true;
                if (ConverseHit(Ground))
                {
                    Ground.GetComponent<SpeedManager>().HitTop = true;
                }
            }
            else
            {
                if (Ground && Ground.GetComponent<SpeedManager>() && Ground.GetComponent<SpeedManager>().HitTop)
                {
                    HitGround = true;
                }
                else
                {
                    HitGround = false;
                }
            }
        }

        if (temp.x < 0)
        {
            if (LeftDis < -temp.x * Time.deltaTime)
            {
                if (LeftDis > 0)
                {
                    temp.x = -LeftDis / Time.deltaTime;
                }
                else
                {
                    temp.x = 0;
                }
                SelfSpeed.x = 0;
                HitLeft = true;

                if (ConverseHit(Left))
                {
                    Left.GetComponent<SpeedManager>().HitRight = false;
                }
            }
            else
            {
                if(Left && Left.GetComponent<SpeedManager>() && Left.GetComponent<SpeedManager>().HitRight)
                {
                    HitLeft = true;
                }
                else
                {
                    HitLeft = false;
                }

            }
        }

        if (temp.x > 0)
        {
            if (RightDis < temp.x * Time.deltaTime)
            {
                if(RightDis > 0)
                {
                    temp.x = RightDis / Time.deltaTime;
                }
                else
                {
                    temp.x = 0;
                }

                SelfSpeed.x = 0;
                HitRight = true;

                if(ConverseHit(Right))
                {
                    Right.GetComponent<SpeedManager>().HitLeft = true;
                }
            }
            else
            {
                if (Right && Right.GetComponent<SpeedManager>() && Right.GetComponent<SpeedManager>().HitLeft)
                {
                    HitRight = true;
                }
                else
                {
                    HitRight = false;
                }

            }
        }

        transform.position += (Vector3)temp* Time.deltaTime;
    }

    public void CheckGroundDis()
    {
        //RaycastHit2D hit = Physics2D.BoxCast(GetTruePos(), new Vector2(BodyWidth - 2 * HitMargin, CastBoxThickness), 0, Vector2.down, DetectDis, layermask);

        //RaycastHit2D hit = Physics2D.BoxCast(GetTruePos(), new Vector2(BodyWidth - 2 * HitMargin, CastBoxThickness), 0, Vector2.down, BodyHeight / 2 + DetectDis / 2 + CastBoxThickness / 2, ~IgnoredLayers);

        //RaycastHit2D hit = Physics2D.BoxCast(GetTruePos() + (BodyHeight / 2 + DetectDis / 2) * Vector2.down, new Vector2(BodyWidth - 2 * HitMargin, DetectDis), 0, Vector2.down, 0, ~IgnoredLayers);

        RaycastHit2D[] HitList= Physics2D.BoxCastAll(GetTruePos() + (BodyHeight / 2 + DetectDis / 2) * Vector2.down, new Vector2(BodyWidth - 2 * HitMargin, DetectDis), 0, Vector2.down, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(HitList, Direction.Bottom);

        if (hit.collider!=null)
        {
            GroundDis = GetHitDis(hit, Direction.Bottom);
            //GroundDis = GetTruePos().y - BodyHeight/2 - hit.point.y ;
            //GroundDis = GetTruePos().y - BodyHeight / 2 - hit.point.y;
            //GroundDis = Mathf.Abs(hit.point.y - GetTruePos().y + CastBoxThickness / 2) - BodyHeight / 2;
            Ground = hit.collider.gameObject;
        }
        else
        {
            GroundDis = Mathf.Infinity;
            Ground = null;
        }

    }

    private void CheckGroundHitting()
    {
        float Dis = HitMargin;

        if(Ground && Ground.GetComponent<SpeedManager>() && !Ground.GetComponent<SpeedManager>().MoveExecuted)
        {
            GroundDis -= (Ground.GetComponent<SpeedManager>().SelfSpeed.y) * Time.deltaTime;
        }

        if (GroundDis <= Dis)
        {
            HitGround = true;
        }
        else
        {
            HitGround = false;
        }
    }

    public void CheckTopDis()
    {

        RaycastHit2D[] HitList= Physics2D.BoxCastAll(GetTruePos() + (DetectDis / 2 + BodyHeight / 2) * Vector2.up, new Vector2(BodyWidth - 2 * HitMargin, DetectDis), 0, Vector2.up, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(HitList, Direction.Top);

        if (hit.collider!=null)
        {
            TopDis = GetHitDis(hit, Direction.Top);
            //TopDis = hit.point.y - GetTruePos().y - BodyHeight / 2;
            //TopDis = Mathf.Abs(hit.point.y - GetTruePos().y - CastBoxThickness / 2) - BodyHeight/2;
            Top = hit.collider.gameObject;
        }
        else
        {
            TopDis = Mathf.Infinity;
            Top = null;
        }
    }

    private void CheckTopHitting()
    {
        float Dis = HitMargin;
        if (TopDis <= Dis)
        {
            HitTop = true;
        }
        else
        {
            HitTop = false;
        }
    }

    public void CheckLeftWallDis()
    {

        RaycastHit2D[] HitList = Physics2D.BoxCastAll(GetTruePos() + (DetectDis/2 + BodyWidth / 2)*Vector2.left, new Vector2(DetectDis, BodyHeight - 2 * HitMargin), 0, Vector2.left, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(HitList, Direction.Left);
        if (hit.collider!=null)
        {
            LeftDis = GetHitDis(hit, Direction.Left);
            //LeftDis = GetTruePos().x - BodyWidth / 2  - hit.point.x;
            //LeftDis = Mathf.Abs(hit.point.x - GetTruePos().x + CastBoxThickness / 2) - BodyWidth / 2;
            Left = hit.collider.gameObject;
        }
        else
        {
            LeftDis = Mathf.Infinity;
            Left = null;
        }
    }

    private void CheckLeftWallHitting()
    {
        float Dis = HitMargin;
        if (LeftDis <= Dis)
        {
            HitLeft = true;
        }
        else
        {
            HitLeft = false;
        }
    }

    public void CheckRightWallDis()
    {
        RaycastHit2D[] HitList = Physics2D.BoxCastAll(GetTruePos() + (DetectDis / 2 + BodyWidth / 2) * Vector2.right, new Vector2(DetectDis, BodyHeight - 2 * HitMargin), 0, Vector2.right, 0, ~IgnoredLayers);
        RaycastHit2D hit = GetClosestHit(HitList, Direction.Right);

        if (hit.collider!=null)
        {
            RightDis = GetHitDis(hit, Direction.Right);
            //RightDis = hit.point.x - GetTruePos().x - BodyWidth / 2;
            //RightDis = Mathf.Abs(hit.point.x - GetTruePos().x - CastBoxThickness / 2) - BodyWidth / 2;
            Right = hit.collider.gameObject;
        }
        else
        {
            RightDis = Mathf.Infinity;
            Right = null;
        }
    }

    private void CheckRightWallHitting()
    {
        float Dis = HitMargin;
        if (RightDis <= Dis)
        {
            HitRight = true;
        }
        else
        {
            HitRight = false;
        }
    }

    private RaycastHit2D GetClosestHit(RaycastHit2D[] HitList,Direction Dir)
    {
        float MinDis = Mathf.Infinity;

        RaycastHit2D Hit=new RaycastHit2D();

        for(int i = 0; i < HitList.Length; i++)
        {
            float Dis = GetHitDis(HitList[i], Dir);

            if (Dis < MinDis)
            {
                if (!HitList[i].collider.gameObject.GetComponent<PassableInfo>())
                {
                    MinDis = Dis;
                    Hit = HitList[i];
                }
                else
                {
                    var Passable = HitList[i].collider.gameObject.GetComponent<PassableInfo>();

                    switch (Dir)
                    {
                        case Direction.Right:
                            if (!Passable.LeftPassable)
                            {
                                MinDis = Dis;
                                Hit = HitList[i];
                            }
                            break;
                        case Direction.Left:
                            if (!Passable.RightPassable)
                            {
                                MinDis = Dis;
                                Hit = HitList[i];
                            }
                            break;
                        case Direction.Top:
                            if (!Passable.BottomPassable)
                            {
                                MinDis = Dis;
                                Hit = HitList[i];
                            }
                            break;
                        case Direction.Bottom:
                            if (!Passable.TopPassable)
                            {
                                MinDis = Dis;
                                Hit = HitList[i];
                            }
                            break;
                    }
                }
            }

        }

        return Hit;
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
}
