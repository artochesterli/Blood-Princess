using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    public LayerMask IgnoredLayers;

    public Vector2 SelfSpeed;
    public Vector2 ForcedSpeed;

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
        Vector2 temp = SelfSpeed + ForcedSpeed;

        if (temp.y > 0)
        {
            float TrueTopDis = TopDis;
            if (Top && Top.GetComponent<SpeedManager>())
            {
                TrueTopDis -= (Top.GetComponent<SpeedManager>().SelfSpeed.y + Top.GetComponent<SpeedManager>().ForcedSpeed.y)*Time.deltaTime;
            }

            if (TrueTopDis < temp.y * Time.deltaTime)
            {
                temp.y = TrueTopDis / Time.deltaTime;
                SelfSpeed.y = 0;
                ForcedSpeed.y = 0;
            }
        }

        if (temp.y < 0)
        {
            float TrueGroundDis = GroundDis;
            if (Ground && Ground.GetComponent<SpeedManager>())
            {
                TrueGroundDis -= (Ground.GetComponent<SpeedManager>().SelfSpeed.y + Ground.GetComponent<SpeedManager>().ForcedSpeed.y)*Time.deltaTime;
            }

            if (TrueGroundDis < -temp.y * Time.deltaTime)
            {
                temp.y = -TrueGroundDis / Time.deltaTime;
                SelfSpeed.y = 0;
                ForcedSpeed.y = 0;
            }
        }

        if (temp.x < 0)
        {
            float TrueLeftDis = LeftDis;
            if (Left && Left.GetComponent<SpeedManager>())
            {
                TrueLeftDis -= (Left.GetComponent<SpeedManager>().SelfSpeed.x + Left.GetComponent<SpeedManager>().ForcedSpeed.x)*Time.deltaTime;
            }

            if (TrueLeftDis < -temp.x * Time.deltaTime)
            {
                
                temp.x = -TrueLeftDis / Time.deltaTime;
                SelfSpeed.x = 0;
                ForcedSpeed.x = 0;
            }
        }

        if (temp.x > 0)
        {
            float TrueRightDis = RightDis;
            if (Right && Right.GetComponent<SpeedManager>())
            {
                TrueRightDis -= (Right.GetComponent<SpeedManager>().SelfSpeed.x + Right.GetComponent<SpeedManager>().ForcedSpeed.x)*Time.deltaTime;
            }
            if (TrueRightDis < temp.x * Time.deltaTime)
            {
                temp.x = TrueRightDis / Time.deltaTime;
                SelfSpeed.x = 0;
                ForcedSpeed.x = 0;
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
