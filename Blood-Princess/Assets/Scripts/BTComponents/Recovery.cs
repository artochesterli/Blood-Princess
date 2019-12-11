using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class Recovery : Action
{
    public SharedFloat Duration;
    public SharedSprite Sprite;

    private float m_Timer;

    public override void OnStart()
    {
        base.OnStart();
        m_Timer = Time.timeSinceLevelLoad + Duration.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer < Time.timeSinceLevelLoad)
        {
            Owner.GetComponent<SpriteRenderer>().sprite = Sprite.Value;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
