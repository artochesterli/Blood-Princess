using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

/// <summary>
/// RunTimer taks a bool and make it true after a certain number of time
/// if it's false
/// </summary>
public class RunTimer : Action
{
	public SharedBool PerformAction;
	public SharedFloat MinTimer;
	public SharedFloat MaxTimer;
	public SharedFloat PerformanceTimer;

	public override void OnAwake()
	{
		PerformanceTimer.Value = Random.Range(MinTimer.Value, MaxTimer.Value);
	}

	public override TaskStatus OnUpdate()
	{
		if (PerformAction.Value) return TaskStatus.Success;
		if (PerformanceTimer.Value >= 0f)
		{
			PerformanceTimer.Value -= Time.deltaTime;
		}
		else
		{
			PerformAction.SetValue(true);
			PerformanceTimer.Value = Random.Range(MinTimer.Value, MaxTimer.Value);
		}
		return TaskStatus.Running;
	}
}
