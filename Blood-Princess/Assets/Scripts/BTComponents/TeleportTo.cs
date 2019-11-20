using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TeleportTo : Action
{
	public SharedGameObject Target;
	public SharedVector2 Offset;
	public SharedBool RelativeToTarget;

	public override void OnAwake()
	{
		base.OnAwake();
		if (Target == null || Target.Value == null)
			Target.Value = GameObject.FindGameObjectWithTag("Player");
	}

	public override void OnStart()
	{
		base.OnStart();
		if (!RelativeToTarget.Value)
			Owner.transform.position = Target.Value.transform.position + (Vector3)Offset.Value;
		else
			Owner.transform.position = Target.Value.transform.position + new Vector3(Offset.Value.x * Target.Value.transform.right.x, Offset.Value.y);
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
