using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[RequiredComponent(typeof(SpeedManager))]
public class PlayerInRange : Conditional
{
    public SharedVector2 RangeLeftDownMostPosition, RangeRightDownMostPosition,
    RangeLeftUpMostPosition, RangeRightUpMostPosition;

    private Vector2 m_LeftDownPos, m_RightDownPos, m_LeftUpPos, m_RightUpPos;
    private SpeedManager m_SpeedManager;
    private SpeedManager m_PlayerSpeedManager;
    private GameObject m_Player;

    public override void OnAwake()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        Debug.Assert(m_Player != null, "Could not Find Player Object");
        m_PlayerSpeedManager = m_Player.GetComponent<SpeedManager>();
        Debug.Assert(m_PlayerSpeedManager != null, "Player Does Not Have a Speed Manager");
        m_SpeedManager = GetComponent<SpeedManager>();
        m_LeftDownPos = new Vector2(m_SpeedManager.GetTruePos().x + RangeLeftDownMostPosition.Value.x,
                                    m_SpeedManager.GetTruePos().y + RangeLeftDownMostPosition.Value.y);
        m_RightDownPos = new Vector2(m_SpeedManager.GetTruePos().x + RangeRightDownMostPosition.Value.x,
                                    m_SpeedManager.GetTruePos().y + RangeRightDownMostPosition.Value.y);
        m_LeftUpPos = new Vector2(m_SpeedManager.GetTruePos().x + RangeLeftUpMostPosition.Value.x,
                                    m_SpeedManager.GetTruePos().y + RangeLeftUpMostPosition.Value.y);
        m_RightUpPos = new Vector2(m_SpeedManager.GetTruePos().x + RangeRightUpMostPosition.Value.x,
                                    m_SpeedManager.GetTruePos().y + RangeRightUpMostPosition.Value.y);
    }

    public override TaskStatus OnUpdate()
    {
        float playerPosX = m_PlayerSpeedManager.GetTruePos().x;
        float playerPosY = m_PlayerSpeedManager.GetTruePos().y;
        if (playerPosX >= m_LeftDownPos.x &&
            playerPosX <= m_RightDownPos.x &&
            playerPosY <= m_LeftUpPos.y &&
            playerPosY >= m_LeftDownPos.y)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.red;

        UnityEditor.Handles.CircleHandleCap(0, RangeLeftDownMostPosition.Value + new Vector2(Owner.transform.position.x, Owner.transform.position.y),
        Quaternion.identity,
        0.5f,
        EventType.Repaint);

        UnityEditor.Handles.CircleHandleCap(0, RangeRightDownMostPosition.Value + new Vector2(Owner.transform.position.x, Owner.transform.position.y),
        Quaternion.identity,
        0.5f,
        EventType.Repaint);

        UnityEditor.Handles.CircleHandleCap(0, RangeLeftUpMostPosition.Value + new Vector2(Owner.transform.position.x, Owner.transform.position.y),
        Quaternion.identity,
        0.5f,
        EventType.Repaint);

        UnityEditor.Handles.CircleHandleCap(0, RangeRightUpMostPosition.Value + new Vector2(Owner.transform.position.x, Owner.transform.position.y),
        Quaternion.identity,
        0.5f,
        EventType.Repaint);


        UnityEditor.Handles.color = oldColor;
#endif
    }
}
