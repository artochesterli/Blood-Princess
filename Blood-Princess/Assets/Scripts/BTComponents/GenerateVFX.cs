using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class GenerateVFX : Action
{
	public SharedGameObject VFXPrefab;
	public SharedVector2 VFXOffset;
	public SharedVector3 VFXEulerAngle;
	public SharedBool Relative;

	public override void OnStart()
	{
		if (VFXPrefab != null && VFXPrefab.Value != null)
		{
			GameObject vfx = GameObject.Instantiate(VFXPrefab.Value, Owner.transform.position, Quaternion.Euler(VFXEulerAngle.Value));
			vfx.transform.position = Owner.transform.position + new Vector3((Relative.Value ? Owner.transform.right.x : 1f) * VFXOffset.Value.x, VFXOffset.Value.y);
		}
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}
