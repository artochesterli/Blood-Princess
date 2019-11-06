using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class GetDirectionFromObject : Action
{
    public SharedGameObject Target;
    public SharedVector2 Result;

    public override void OnStart()
    {
        if (Target == null || Target.Value == null)
        {
            Target.Value = GameObject.FindGameObjectWithTag("Player");
            Debug.Assert(Target.Value != null, "Could not find Player GameObject");
        }
    }

    public override TaskStatus OnUpdate()
    {
        Result.Value = (Target.Value.transform.position - Owner.transform.position).normalized;
        return TaskStatus.Success;
    }
}
