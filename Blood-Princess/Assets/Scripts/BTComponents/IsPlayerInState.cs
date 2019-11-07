using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class IsPlayerInState : Conditional
{
	public SharedString StateName;
	private GameObject m_Player;

	public override void OnAwake()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		Debug.Assert(m_Player != null, "Player Missing");
	}

	public override TaskStatus OnUpdate()
	{
		return m_Player.GetComponent<CharacterAction>().InState(StateName.Value) ? TaskStatus.Success : TaskStatus.Failure;
	}
}
