using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedManager : MonoBehaviour
{
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

    private int layermask;

    public float BodyWidth;
    public float BodyHeight;

    public Vector2 OriPos;

    private const float DetectDis = 1;
    private const float HitMargin = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        layermask = 1 << LayerMask.NameToLayer("Player") | 1<< LayerMask.NameToLayer("Zone");
        layermask = ~layermask;
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

    

    public void Move()
    {
        Vector2 temp = SelfSpeed + ForcedSpeed;

        if (TopDis < temp.y * Time.deltaTime)
        {
            temp.y = TopDis / Time.deltaTime;
            SelfSpeed.y = 0;
            ForcedSpeed.y = 0;
        }

        if (GroundDis < -temp.y * Time.deltaTime)
        {
            temp.y = -GroundDis / Time.deltaTime;
            SelfSpeed.y = 0;
            ForcedSpeed.y = 0;
        }

        if (LeftDis<-temp.x * Time.deltaTime)
        {
            temp.x = -LeftDis / Time.deltaTime;
            SelfSpeed = Vector2.zero;
            ForcedSpeed.x = 0;
        }

        if (RightDis < temp.x * Time.deltaTime)
        {
            temp.x = RightDis / Time.deltaTime;
            SelfSpeed = Vector2.zero;
            ForcedSpeed.x = 0;
        }

        transform.position += (Vector3)temp* Time.deltaTime;
    }

    public void CheckGroundDis()
    {
        Vector3 OriPoint = transform.position + (Vector3)OriPos;
        RaycastHit2D hit = Physics2D.BoxCast(OriPoint, new Vector2(BodyWidth - 2 * HitMargin, 0.01f), 0, Vector2.down, DetectDis, layermask);


        if (hit)
        {
            GroundDis = Mathf.Abs(hit.point.y - OriPoint.y) - BodyHeight / 2;
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
        Vector3 OriPoint = transform.position + (Vector3)OriPos;
        RaycastHit2D hit = Physics2D.BoxCast(OriPoint, new Vector2(BodyWidth- 2 * HitMargin , 0), 0, Vector2.up, DetectDis, layermask);

        if (hit)
        {
            TopDis = Mathf.Abs(hit.point.y - OriPoint.y) - BodyHeight/2;
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
        Vector3 OriPoint = transform.position + (Vector3)OriPos;

        RaycastHit2D hit = Physics2D.BoxCast(OriPoint, new Vector2(0, BodyHeight - 2 * HitMargin), 0, Vector2.left, DetectDis, layermask);

        if (hit)
        {
            LeftDis = Mathf.Abs(hit.point.x - OriPoint.x) - BodyWidth / 2;
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
        Vector3 OriPoint = transform.position + (Vector3)OriPos;

        RaycastHit2D hit = Physics2D.BoxCast(OriPoint, new Vector2(0, BodyHeight - 2 * HitMargin),0,Vector2.right, DetectDis, layermask);

        if (hit)
        {
            RightDis = Mathf.Abs(hit.point.x - OriPoint.x) - BodyWidth / 2;
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
