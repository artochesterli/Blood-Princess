using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SharedFloatSmallerThan : Conditional
{
	public SharedFloat Target;
	public SharedFloat CompareTo;

	public override TaskStatus OnUpdate()
	{
		if (Target.Value <= CompareTo.Value)
			return TaskStatus.Success;
		else
			return TaskStatus.Failure;
	}
}
