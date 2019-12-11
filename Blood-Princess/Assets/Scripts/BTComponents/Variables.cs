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

    [System.Serializable]
    public class SharedSprite : SharedVariable<Sprite>
    {
        public static implicit operator SharedSprite(Sprite value) { return new SharedSprite { mValue = value }; }
    }

    [System.Serializable]
    public class SharedLayerMask : SharedVariable<LayerMask>
    {
        public static implicit operator SharedLayerMask(LayerMask value) { return new SharedLayerMask { mValue = value }; }
    }
    [System.Serializable]
    public class SharedIntList : SharedVariable<List<int>>
    {
        public static implicit operator SharedIntList(List<int> value) { return new SharedIntList { mValue = value }; }
    }

}
