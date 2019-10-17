using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private const float DetectDis = 1;
    private const float HitMargin = 0.02f;
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

        if (Ground && Ground.GetComponent<SpeedManager>() && !Ground.GetComponent<SpeedManager>().MoveExecuted)
        {
            GroundDis -= (Ground.GetComponent<SpeedManager>().SelfSpeed.y) * Time.deltaTime;
        }

        if (Top && Top.GetComponent<SpeedManager>() && !Top.GetComponent<SpeedManager>().MoveExecuted)
        {
            TopDis -= (Top.GetComponent<SpeedManager>().SelfSpeed.y) * Time.deltaTime;
        }

        if (Right && Right.GetComponent<SpeedManager>() && !Right.GetComponent<SpeedManager>().MoveExecuted)
        {
            RightDis -= (Right.GetComponent<SpeedManager>().SelfSpeed.x) * Time.deltaTime;
        }

        if (Left && Left.GetComponent<SpeedManager>() && !Left.GetComponent<SpeedManager>().MoveExecuted)
        {
            LeftDis -= (Left.GetComponent<SpeedManager>().SelfSpeed.x) * Time.deltaTime;
        }


        if (temp.y > 0)
        {
            if (TopDis < temp.y * Time.deltaTime)
            {
                temp.y = TopDis / Time.deltaTime;
                SelfSpeed.y = 0;
                HitTop = true;
                if (Top && Top.GetComponent<SpeedManager>())
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
                temp.y = -GroundDis / Time.deltaTime;
                SelfSpeed.y = 0;
                HitGround = true;
                if (Ground && Ground.GetComponent<SpeedManager>())
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
                temp.x = -LeftDis / Time.deltaTime;
                SelfSpeed.x = 0;
                HitLeft = true;

                if (Left && Left.GetComponent<SpeedManager>())
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
                temp.x = RightDis / Time.deltaTime;
                SelfSpeed.x = 0;
                HitRight = true;

                if(Right && Right.GetComponent<SpeedManager>())
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

        RaycastHit2D hit = Physics2D.BoxCast(GetTruePos(), new Vector2(BodyWidth - 2 * HitMargin, CastBoxThickness), 0, Vector2.down, DetectDis+ BodyHeight / 2 + CastBoxThickness / 2, ~IgnoredLayers);

        if (hit)
        {
            GroundDis = GetTruePos().y - BodyHeight / 2 - hit.point.y;
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
        RaycastHit2D hit = Physics2D.BoxCast(GetTruePos(), new Vector2(BodyWidth- 2 * HitMargin , CastBoxThickness), 0, Vector2.up, DetectDis + BodyHeight / 2 + CastBoxThickness / 2, ~IgnoredLayers);

        if (hit)
        {
            TopDis = hit.point.y - GetTruePos().y - BodyHeight / 2;
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

        RaycastHit2D hit = Physics2D.BoxCast(GetTruePos(), new Vector2(CastBoxThickness, BodyHeight - 2 * HitMargin), 0, Vector2.left, DetectDis+ BodyWidth / 2 + CastBoxThickness / 2, ~IgnoredLayers);

        if (hit)
        {
            LeftDis = GetTruePos().x - BodyWidth / 2  - hit.point.x;
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

        RaycastHit2D hit = Physics2D.BoxCast(GetTruePos(), new Vector2(CastBoxThickness, BodyHeight - 2 * HitMargin),0,Vector2.right, DetectDis+ BodyWidth / 2 + CastBoxThickness / 2, ~IgnoredLayers);

        if (hit)
        {
            RightDis = hit.point.x - GetTruePos().x - BodyWidth / 2;
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
}
