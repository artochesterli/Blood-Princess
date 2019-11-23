﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHittable
{
    int MaxHP { get; set; }
    int CurrentHP { get; set; }
    bool Interrupted { get; set; }

	bool OnHit(AttackInfo Attack);
}

interface IShield
{
    int MaxShield { get; set; }
	int CurrentShield { get; set; }
}

public class StatusManagerBase : MonoBehaviour, IHittable
{
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }

    public bool Interrupted { get; set; }
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public virtual bool OnHit(AttackInfo Attack)
	{
		return CurrentHP <= 0;
	}
}
