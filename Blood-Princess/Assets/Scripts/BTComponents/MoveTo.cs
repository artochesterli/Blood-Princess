using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class MoveTo : Action
{
    public SharedFloat speed;
    public SharedFloat arriveDistance;
    public SharedVector2 targetPosition;

    private Vector2 m_TargetPosition { get { return m_OriginalPosition + targetPosition.Value; } }
    private Vector2 m_OriginalPosition;
    private SpeedManager m_SpeedManager;
    private Vector2 m_TruePosition { get { return m_SpeedManager.GetTruePos(); } }

    public override void OnAwake()
    {
        m_SpeedManager = GetComponent<SpeedManager>();
        m_OriginalPosition.x = m_TruePosition.x;
        m_OriginalPosition.y = m_TruePosition.y;
    }

    public override TaskStatus OnUpdate()
    {
        // Check if arrived
        // Return Success if so
        if (Vector2.Distance(m_TruePosition, m_TargetPosition) <= arriveDistance.Value)
        {
            return TaskStatus.Success;
        }
        bool MovingRight = m_TruePosition.x < m_TargetPosition.x;

        if (MovingRight) transform.eulerAngles = Vector3.zero;
        else transform.eulerAngles = new Vector3(0f, 180f, 0f);

        // Set Speed
        m_SpeedManager.SelfSpeed.x = (MovingRight ? 1f : -1f) * speed.Value;

        return TaskStatus.Running;
    }

}
