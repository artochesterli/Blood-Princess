using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loot
{
	[CreateAssetMenu(fileName = "LootData", menuName = "FoxData/LootData", order = 1)]
	public class LootScriptableObject : ScriptableObject
	{
		public List<Loot> Loots;

		public GameObject GetPrefabFromName(string Name)
		{
			//return Loots.Find(x => x.Name == Name).Prefab;
			return Loots.Find(x => Name.ToLower().Contains(x.Name.ToLower())).Prefab;
		}
	}

	[System.Serializable]
	public class Loot
	{
		public string Name;
		public GameObject Prefab;
	}

}

