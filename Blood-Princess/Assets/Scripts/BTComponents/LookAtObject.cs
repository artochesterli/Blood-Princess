using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(SpeedManager))]
public class LookAtObject : Action
{
    public SharedGameObject Target;
    private SpeedManager m_SpeedManager;
    private SpeedManager m_PlayerSpeedManager;

    public override void OnAwake()
    {
        if (Target == null || Target.Value == null)
        {
            Target.Value = GameObject.FindGameObjectWithTag("Player");
            Debug.Assert(Target.Value != null, "Could not find Player Object");
        }
        m_SpeedManager = Owner.GetComponent<SpeedManager>();
        Debug.Assert(m_SpeedManager != null, "Could not find speed manager");
        m_PlayerSpeedManager = Target.Value.GetComponent<SpeedManager>();
        Debug.Assert(m_PlayerSpeedManager != null, "Could not find speed manager on player");
    }

    public override TaskStatus OnUpdate()
    {
        if (m_SpeedManager.GetTruePos().x > m_PlayerSpeedManager.GetTruePos().x)
        {
            Owner.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else
        {
            Owner.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        return TaskStatus.Success;
    }

}
