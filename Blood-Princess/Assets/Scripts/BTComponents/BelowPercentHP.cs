using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequiredComponent(typeof(StatusManager_General))]
public class BelowPercentHP : Conditional
{
	public SharedFloat Percent;
	public SharedInt CurrentHP;
	private int MaxHP { get { return GetComponent<StatusManager_General>().MaxHP; } }

	public override TaskStatus OnUpdate()
	{
		if (CurrentHP.Value <= MaxHP * Percent.Value)
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}
}
