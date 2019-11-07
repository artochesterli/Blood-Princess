using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CanAct : Conditional
{
	public SharedBool variable;
	public SharedBool compareTo;

	public override TaskStatus OnUpdate()
	{
		if (variable.Value.Equals(compareTo.Value))
		{
			return TaskStatus.Success;
		}
		else
			return TaskStatus.Failure;
	}
}
