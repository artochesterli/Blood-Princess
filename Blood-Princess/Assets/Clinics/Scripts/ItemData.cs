using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	[CreateAssetMenu(fileName = "ItemData", menuName = "FarmingPrototype/ItemData", order = 1)]
	public class ItemData : ScriptableObject
	{
		public List<ItemDatium> Items;
		public ItemDatium GetItem(string Name)
		{
			foreach (var item in Items)
			{
				if (item.Name.ToLower() == Name.ToLower())
					return item;
			}
			return null;
		}
	}
}
