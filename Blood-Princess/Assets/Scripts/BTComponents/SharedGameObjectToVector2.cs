using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class SharedGameObjectToVector2 : Action
{
	public SharedGameObject Target;
	public SharedVector2 Vector2Target;

	public override void OnStart()
	{
		base.OnStart();
		if (Target.Value == null)
			Target.Value = GameObject.FindGameObjectWithTag("Player");
		Vector2Target.Value = (Vector2)Target.Value.transform.position;
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
