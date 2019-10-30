using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class MoveTowards : Action
{
    public SharedFloat Speed;
    public SharedGameObject Target;
    public SharedFloat ArrivalDistance;

    private SpeedManager m_SpeedManager;

    public override void OnAwake()
    {
        base.OnAwake();
        m_SpeedManager = GetComponent<SpeedManager>();
        if (Target == null || Target.Value == null)
            Target.Value = GameObject.FindGameObjectWithTag("Player");
    }

    public override TaskStatus OnUpdate()
    {
        bool MovingRight = m_SpeedManager.GetTruePos().x < Target.Value.transform.position.x;

        if (MovingRight) transform.eulerAngles = Vector3.zero;
        else transform.eulerAngles = new Vector3(0f, 180f, 0f);

        m_SpeedManager.SelfSpeed.x = (MovingRight ? 1f : -1f) * Speed.Value;
        if (Vector2.Distance(Target.Value.transform.position, m_SpeedManager.GetTruePos()) <= ArrivalDistance.Value)
        {
            m_SpeedManager.SelfSpeed.x = 0f;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
