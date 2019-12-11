using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG;

public class EnemyGenerationManager
{
    private List<EnemyInfo> m_EnemyInfo;
    private EnemyGenerationScriptableObject EnemyGenerationData;
    private System.Random m_Rand;
    public EnemyGenerationManager(EnemyGenerationScriptableObject enemygenerationdata)
    {
        EnemyGenerationData = enemygenerationdata;
        m_Rand = new System.Random();
        m_EnemyInfo = new List<EnemyInfo>();
    }

    public void RecordNextEnemyInfo(string type, IntVector2 boardPosition)
    {
        m_EnemyInfo.Add(new EnemyInfo(type, boardPosition));
    }

    public void GenerateEnemies(string[,] _board)
    {

    }

    private void m_GenerateEnemy(string type, IntVector2 boardPosition, string[,] board)
    {
        GameObject instantiatedEnmey = null;
        switch (type)
        {
            case "M-F":
                instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
                break;
            case "M-M":
                instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy2", typeof(GameObject))) as GameObject;
                break;
            case "M-S":
                instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/SoulWarrior", typeof(GameObject))) as GameObject;
                break;
            case "M-1":
                int randInt = m_Rand.Next(1, 101);
                if (randInt < 30)
                    instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
                else if (randInt < 65)
                    instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy2", typeof(GameObject))) as GameObject;
                else if (randInt < 101)
                    instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/SoulWarrior", typeof(GameObject))) as GameObject;
                break;
        }
    }

    public void Destroy()
    {

    }
}
