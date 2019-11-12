using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class CompareModInt : Conditional
{
	public SharedInt Target;
	public SharedInt Mod;
	public SharedInt EqualTo;

	public override TaskStatus OnUpdate()
	{
		return Target.Value % Mod.Value == EqualTo.Value ? TaskStatus.Success : TaskStatus.Failure;
	}
}
