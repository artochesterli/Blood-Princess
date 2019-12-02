using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class RollNumber : Action
{
	public SharedFloat NumberVar;
	public SharedFloat MaxRoll;
	public SharedFloat MinRoll;

	public override void OnStart()
	{
		base.OnStart();
		NumberVar.Value = Random.Range(MinRoll.Value, MaxRoll.Value);
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
