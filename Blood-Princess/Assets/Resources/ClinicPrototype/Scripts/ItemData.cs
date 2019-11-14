using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "FarmingPrototype/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
	public List<ItemDatium> Items;
}
