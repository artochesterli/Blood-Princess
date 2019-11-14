using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(GravityManager))]
public class SetGravity : Action
{
	public SharedFloat Gravity;
	public SharedBool Relative;

	public override void OnStart()
	{
		base.OnStart();
		if (Relative.Value)
		{
			GetComponent<GravityManager>().Gravity += Gravity.Value;
		}
		else
		{
			GetComponent<GravityManager>().Gravity = Gravity.Value;
		}
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
