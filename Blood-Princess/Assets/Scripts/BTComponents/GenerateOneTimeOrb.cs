using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class GenerateOneTimeOrb : Action
{
	public SharedColor OrbColor;
	public SharedVector2 Offset;
	public SharedFloat UpTime;
	private SpeedManager m_SpeedManager;

	public override void OnAwake()
	{
		m_SpeedManager = Owner.GetComponent<SpeedManager>();
	}

	public override TaskStatus OnUpdate()
	{
		GameObject orb = GameObject.Instantiate(Resources.Load("Prefabs/Orb") as GameObject, m_SpeedManager.GetTruePos() + Offset.Value, Quaternion.identity, Owner.transform);
		orb.GetComponent<SpriteRenderer>().color = OrbColor.Value;
		GameObject.Destroy(orb, UpTime.Value);
		return TaskStatus.Success;
	}
}
