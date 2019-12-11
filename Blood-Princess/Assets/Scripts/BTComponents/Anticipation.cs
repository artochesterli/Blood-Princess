using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpriteRenderer))]
public class Anticipation : Action
{
    public SharedFloat Duration;
    public Sprite ChangedSprite;

    private SpriteRenderer m_SpriteRenderer;
    private float m_Timer;
    private GameObject m_Player;

    public override void OnAwake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void OnStart()
    {
        m_Timer = Time.timeSinceLevelLoad + Duration.Value;
        m_SpriteRenderer.sprite = ChangedSprite;
    }

    public override TaskStatus OnUpdate()
    {
        if (m_Timer <= Time.timeSinceLevelLoad)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
