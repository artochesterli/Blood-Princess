using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class TimedIdle : Action
{
    public SharedFloat MinIdleTime;
    public SharedFloat MaxIdleTime;
    private float m_Timer;
    private SpeedManager m_SpeedManager;

    public override void OnStart()
    {
        m_Timer = Time.timeSinceLevelLoad + Random.Range(MinIdleTime.Value, MaxIdleTime.Value);
        m_SpeedManager = GetComponent<SpeedManager>();
        m_SpeedManager.SelfSpeed = Vector2.zero;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer < Time.timeSinceLevelLoad)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
