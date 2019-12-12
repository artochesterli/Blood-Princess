using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpriteRenderer))]
public class ChangeSprite : Action
{
    public SharedFloat Duration;
    public SharedSprite Sprite;
    private SpriteRenderer m_SpriteRenderer;
    private float m_Timer;

    public override void OnAwake()
    {
        base.OnAwake();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnStart()
    {
        base.OnStart();
        m_Timer = Time.timeSinceLevelLoad + Duration.Value;
        m_SpriteRenderer.sprite = Sprite.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer < Time.timeSinceLevelLoad)
        {
            //m_SpriteRenderer.sprite = m_KnightSpriteData.Idle;
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

}
