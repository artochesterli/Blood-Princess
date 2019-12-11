using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager
{
    private VFXScriptableObject m_VFXData;

    public VFXManager(VFXScriptableObject m_VFXData)
    {
        this.m_VFXData = m_VFXData;
        EventManager.instance.AddHandler<PlayerKillEnemy>(m_EnemyOnDeath);
    }

    private void m_EnemyOnDeath(PlayerKillEnemy ev)
    {
        GameObject.Instantiate(m_VFXData.EnemyDeathEffect, ev.Enemy.GetComponent<SpeedManager>().GetTruePos(), m_VFXData.EnemyDeathEffect.transform.rotation);
    }

    public void Destroy()
    {
        EventManager.instance.RemoveHandler<PlayerKillEnemy>(m_EnemyOnDeath);

    }
}
