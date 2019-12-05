using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour, IHittable
{
	public int MaxHP { get => 1; set => MaxHP = value; }
	public int CurrentHP { get => 1; set => CurrentHP = value; }
	public bool Interrupted { get => false; set => Interrupted = value; }

	public bool OnHit(AttackInfo Attack)
	{
		Destroy(gameObject);
		return true;
	}
}
