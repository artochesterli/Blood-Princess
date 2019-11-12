using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class ChaosBlade : Action
{
    public SharedVector2 BladeDirection;
    public SharedFloat MaxDistance;
    public SharedFloat FlySpeed;
    public SharedFloat WaitTime;
    public SharedInt Damage;
    public LayerMask PlayerLayer;

    private float m_Timer;

    public override void OnStart()
    {
        GameObject InstantiatedBlade = GameObject.Instantiate(Resources.Load("Prefabs/ChaosBlade"), Owner.transform.position, Owner.transform.rotation) as GameObject;
        InstantiatedBlade.GetComponent<ChaosBladeControl>().InitiatedBlade(Owner.gameObject, BladeDirection.Value,
        MaxDistance.Value, FlySpeed.Value, WaitTime.Value, Damage.Value, PlayerLayer);
        m_Timer = Time.timeSinceLevelLoad + WaitTime.Value + 2f * MaxDistance.Value / FlySpeed.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer < Time.timeSinceLevelLoad)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
