using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerationManager
{
    private EnemyGenerationScriptableObject EnemyGenerationData;
    public EnemyGenerationManager(EnemyGenerationScriptableObject enemygenerationdata)
    {
        EnemyGenerationData = enemygenerationdata;
    }

    public void Destroy()
    {

    }
}
