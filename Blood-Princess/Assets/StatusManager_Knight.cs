using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager_Knight : StatusManagerBase , IHittable, IRage
{
    public bool Rage { get; set; }
    public int RageCount { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool OnHit(AttackInfo Attack)
    {
        return base.OnHit(Attack);

    }
}
