using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedVector2List : SharedVariable<List<Vector2>>
    {
        public static implicit operator SharedVector2List(List<Vector2> value) { return new SharedVector2List { mValue = value }; }
    }

}
