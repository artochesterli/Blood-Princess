using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHittable
{

    int CurrentHP { get; set; }
    bool Interrupted { get; set; }
    int ReceivedInterruptionLevel { get; set; }

	bool OnHit(AttackInfo Attack);
}

interface IShield
{
    int MaxShield { get; set; }
	int CurrentShield { get; set; }
}

public class StatusManagerBase : MonoBehaviour, IHittable
{
    public int CurrentHP { get; set; }
    public int CurrentMaxHP { get; set; }

    public bool Interrupted { get; set; }
    public int ReceivedInterruptionLevel { get; set; }
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
