using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(SpeedManager))]
public class BackStepAvoid : Action
{
    public SharedFloat BackStepDuration;
    public SharedVector2 BackStepSpeed;
    private SpeedManager m_SpeedManager;
    private float m_Timer;

    public override void OnAwake()
    {
        m_SpeedManager = Owner.GetComponent<SpeedManager>();
        Debug.Assert(m_SpeedManager != null, "Does not have a Speed Manager on it");
    }

    public override void OnStart()
    {
        m_Timer = Time.timeSinceLevelLoad + BackStepDuration.Value;
        m_SpeedManager.SelfSpeed.y = BackStepSpeed.Value.y;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer < Time.timeSinceLevelLoad)
            return TaskStatus.Success;
        m_SpeedManager.SelfSpeed.x = -Owner.transform.right.x * BackStepSpeed.Value.x;
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        m_SpeedManager.SelfSpeed.x = 0f;
    }
}
