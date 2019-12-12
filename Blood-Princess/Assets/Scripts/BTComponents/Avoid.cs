using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(SpeedManager))]
public class Avoid : Action
{
    public SharedFloat BackStepDuration;
    public SharedVector2 BackStepSpeed;
    public SharedLayerMask WallLayer;
    private SpeedManager m_SpeedManager;
    private float m_Timer;
    private int m_Direction = 1;

    public override void OnAwake()
    {
        m_SpeedManager = Owner.GetComponent<SpeedManager>();
        Debug.Assert(m_SpeedManager != null, "Does not have a Speed Manager on it");
    }

    public override void OnStart()
    {
        m_Timer = Time.timeSinceLevelLoad + BackStepDuration.Value;
        m_SpeedManager.SelfSpeed.y = BackStepSpeed.Value.y;
        if (Physics2D.Raycast(m_SpeedManager.GetTruePos(), -Owner.transform.right, BackStepDuration.Value * BackStepSpeed.Value.x, WallLayer.Value))
        {
            m_Direction = -1;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer < Time.timeSinceLevelLoad)
            return TaskStatus.Success;
        m_SpeedManager.SelfSpeed.x = -Owner.transform.right.x * BackStepSpeed.Value.x * m_Direction;
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        m_SpeedManager.SelfSpeed.x = 0f;
    }
}
