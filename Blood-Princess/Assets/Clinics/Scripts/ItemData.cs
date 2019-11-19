using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	[CreateAssetMenu(fileName = "ItemData", menuName = "FarmingPrototype/ItemData", order = 1)]
	public class ItemData : ScriptableObject
	{
		public List<ItemDatium> Items;
		public List<CraftableItemDatium> CraftableItems;
		public List<SeedItemDatium> SeedItems;

		public ItemDatium GetItem(string Name)
		{
			foreach (var item in Items)
			{
				if (item.Name.ToLower() == Name.ToLower())
					return item;
			}
			foreach (var item in CraftableItems)
			{
				if (item.Name.ToLower() == Name.ToLower())
					return item;
			}
			foreach (var item in SeedItems)
			{
				if (item.Name.ToLower() == Name.ToLower())
					return item;
			}
			return null;
		}
	}
}
