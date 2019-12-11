using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapGenerationData", menuName = "FoxData/MapGenerationScriptableObject", order = 1)]
public class MapGenerationScriptableObject : ScriptableObject
{
    public List<int> MapBag;
    public List<int> S1Bag;
    public List<int> S2Bag;
    public List<int> S3Bag;
}
