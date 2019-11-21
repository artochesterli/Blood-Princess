using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TeleportTo : Action
{
	public SharedGameObject Target;
	public SharedVector2 Offset;
	public SharedVector2 WorldPosition;
	public SharedBool ToWorldPositionInsteadofTarget;
	public SharedBool RelativeToTarget;
	public SharedBool IgnoreX;
	public SharedBool IgnoreY;

	public override void OnAwake()
	{
		base.OnAwake();
		if (Target == null || Target.Value == null)
			Target.Value = GameObject.FindGameObjectWithTag("Player");
	}

	public override void OnStart()
	{
		base.OnStart();
		if (ToWorldPositionInsteadofTarget.Value)
		{
			Owner.transform.position = WorldPosition.Value;
			return;
		}
		if (!RelativeToTarget.Value)
		{
			Vector3 targetPos = Target.Value.transform.position + (Vector3)Offset.Value;
			if (IgnoreX.Value)
				targetPos.x = Owner.transform.position.x;
			if (IgnoreY.Value)
				targetPos.y = Owner.transform.position.y;
			Owner.transform.position = targetPos;
		}
		else
		{
			Vector3 targetPos = Target.Value.transform.position + new Vector3(Offset.Value.x * Target.Value.transform.right.x, Offset.Value.y);
			if (IgnoreX.Value)
				targetPos.x = Owner.transform.position.x;
			if (IgnoreY.Value)
				targetPos.y = Owner.transform.position.y;
			Owner.transform.position = targetPos;
		}
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
