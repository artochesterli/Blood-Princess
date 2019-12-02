using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(BoxCollider2D))]
public class ToggleBoxcollider2D : Action
{
	public SharedBool Bool;

	public override TaskStatus OnUpdate()
	{
		Owner.GetComponent<BoxCollider2D>().enabled = Bool.Value;
		return TaskStatus.Success;
	}
}
