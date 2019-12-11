using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class Patrol : Action
{
    public SharedFloat Speed;
    public SharedFloat MinWayPointPauseDuration = 0f;
    public SharedFloat MaxWayPointPauseDuration = 2f;

    private List<Vector2> m_TargetPositions;
    private int m_WayPointIndex;
    private float m_WayPointReachTimer;
    private SpeedManager m_SpeedManager;
    private Vector2 m_TruePosition { get { return m_SpeedManager.GetTruePos(); } }
    private bool m_MovingRight;

    public override void OnAwake()
    {
        m_WayPointReachTimer = -1f;
        m_SpeedManager = GetComponent<SpeedManager>();
        Debug.Assert(m_SpeedManager != null, "There is no Speed Manager on GameObject: " + gameObject.name);
        m_TargetPositions = new List<Vector2>();
    }

    public override void OnStart()
    {
        m_MovingRight = false;
        if (m_MovingRight) transform.eulerAngles = Vector3.zero;
        else transform.eulerAngles = new Vector3(0f, 180f, 0f);

        // Set Speed
        m_SpeedManager.SelfSpeed.x = (m_MovingRight ? 1f : -1f) * Speed.Value;

    }

    public override TaskStatus OnUpdate()
    {
        // If Arrived at point, then wait for seconds
        if (_hasArrived())
        {
            if (m_WayPointReachTimer == -1)
            {
                m_SpeedManager.SelfSpeed = Vector2.zero;
                m_WayPointReachTimer = Time.timeSinceLevelLoad + Random.Range(MinWayPointPauseDuration.Value, MaxWayPointPauseDuration.Value);
            }
            if (m_WayPointReachTimer <= Time.timeSinceLevelLoad)
            {
                m_MovingRight = !m_MovingRight;
                if (m_MovingRight) transform.eulerAngles = Vector3.zero;
                else transform.eulerAngles = new Vector3(0f, 180f, 0f);

                // Set Speed
                m_SpeedManager.SelfSpeed.x = (m_MovingRight ? 1f : -1f) * Speed.Value;

                m_WayPointReachTimer = -1;
            }
        }
        else
        {
            if (m_MovingRight) transform.eulerAngles = Vector3.zero;
            else transform.eulerAngles = new Vector3(0f, 180f, 0f);

            // Set Speed
            m_SpeedManager.SelfSpeed.x = (m_MovingRight ? 1f : -1f) * Speed.Value;

        }
        return TaskStatus.Running;
    }
    private bool _hasArrived()
    {
        if ((m_SpeedManager.SelfSpeed.x > 0 || m_SpeedManager.SelfSpeed.x == 0) && m_SpeedManager.HitRight)
            return true;
        if ((m_SpeedManager.SelfSpeed.x < 0 || m_SpeedManager.SelfSpeed.x == 0) && m_SpeedManager.HitLeft)
            return true;
        if ((m_SpeedManager.SelfSpeed.x > 0 || m_SpeedManager.SelfSpeed.x == 0) && _isOnRightEdge())
            return true;
        if ((m_SpeedManager.SelfSpeed.x < 0 || m_SpeedManager.SelfSpeed.x == 0) && _isOnLeftEdge())
            return true;
        return false;
    }

    private bool _isOnLeftEdge()
    {
        Vector2 castOrigin = m_SpeedManager.GetTruePos();
        castOrigin.x -= m_SpeedManager.BodyWidth / 2f - 0.2f;
        castOrigin.y -= m_SpeedManager.BodyHeight / 2f;
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, new Vector2(0.1f, 0.1f), 0f, Vector2.zero);

        return hit.collider == null && m_SpeedManager.HitGround;
    }

    private bool _isOnRightEdge()
    {
        Vector2 castOrigin = m_SpeedManager.GetTruePos();
        castOrigin.x += m_SpeedManager.BodyWidth / 2f + 0.2f;
        castOrigin.y -= m_SpeedManager.BodyHeight / 2f;
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, new Vector2(0.1f, 0.1f), 0f, Vector2.zero);

        return hit.collider == null && m_SpeedManager.HitGround;
    }
}
