using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TeleportTo : Action
{
	public SharedGameObject Target;
	public SharedVector2 Offset;

	public override void OnStart()
	{
		base.OnStart();
		Owner.transform.position = Target.Value.transform.position + (Vector3)Offset.Value;
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
