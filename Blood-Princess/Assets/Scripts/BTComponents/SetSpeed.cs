using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class SetSpeed : Action
{
	public SharedVector2 Speed;

	public override void OnStart()
	{
		base.OnStart();
		Owner.GetComponent<SpeedManager>().SelfSpeed = Speed.Value;
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
