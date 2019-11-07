using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    Solid,
    BorderSolid,
    ForceField
}

public class ColliderInfo : MonoBehaviour
{
    public ColliderType Type;

    public bool LeftPassable;
    public bool RightPassable;
    public bool TopPassable;
    public bool BottomPassable;
}
