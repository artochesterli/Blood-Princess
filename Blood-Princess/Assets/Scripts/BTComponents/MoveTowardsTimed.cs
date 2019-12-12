using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class MoveTowardsTimed : Action
{
    public SharedFloat Speed;
    public SharedGameObject Target;
    public SharedFloat ArrivalDistance;
    public SharedFloat Duration;

    private SpeedManager m_SpeedManager;
    private float m_Timer;

    public override void OnAwake()
    {
        base.OnAwake();
        m_SpeedManager = GetComponent<SpeedManager>();
        if (Target == null || Target.Value == null)
            Target.Value = GameObject.FindGameObjectWithTag("Player");
    }

    public override void OnStart()
    {
        base.OnStart();
        m_Timer = Time.timeSinceLevelLoad + Duration.Value;
    }

    public override TaskStatus OnUpdate()
    {
        bool MovingRight = m_SpeedManager.GetTruePos().x < Target.Value.transform.position.x;

        if (MovingRight) transform.eulerAngles = Vector3.zero;
        else transform.eulerAngles = new Vector3(0f, 180f, 0f);

        m_SpeedManager.SelfSpeed.x = (MovingRight ? 1f : -1f) * Speed.Value;
        if (Mathf.Abs(AIUtility.GetXDiff(Target.Value, Owner.gameObject)) <= ArrivalDistance.Value)
        {
            m_SpeedManager.SelfSpeed.x = 0f;
        }

        if (m_Timer < Time.timeSinceLevelLoad)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
