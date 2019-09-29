using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHittable
{
    bool hit { get; set; }
    AttackInfo HitAttack { get; set; }

    bool OnHit(AttackInfo Attack);
}

interface IRage
{
    bool Rage { get; set; }
    int RageCount { get; set; }
}

public class StatusManagerBase : MonoBehaviour, IHittable
{
    public int CurrentHP;
    public AttackInfo HitAttack { get; set; }
    public bool hit { get; set; }
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
        hit = true;
        HitAttack = Attack;
        return CurrentHP <= 0;
    }
}
