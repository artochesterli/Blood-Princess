using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TetrisRollBag : Action
{
    public SharedIntList RollBagResult;
    public SharedIntList RollBagTemplate;
    public SharedInt Result;

    public override void OnStart()
    {
        if (m_RollBagEmpty())
        {
            m_FillRollBag();
        }
        int randInt = Random.Range(0, RollBagResult.Value.Count);
        while (RollBagResult.Value[randInt] <= 0)
        {
            randInt = Random.Range(0, RollBagResult.Value.Count);
        }
        Result.Value = randInt;
        RollBagResult.Value[randInt]--;
    }

    private void m_FillRollBag()
    {
        for (int i = 0; i < RollBagTemplate.Value.Count; i++)
        {
            RollBagResult.Value[i] = RollBagTemplate.Value[i];
        }
    }

    private bool m_RollBagEmpty()
    {
        for (int i = 0; i < RollBagResult.Value.Count; i++)
        {
            if (RollBagResult.Value[i] > 0) return false;
        }
        return true;
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
