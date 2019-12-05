using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompiledTypes;

namespace Loot
{
	public class CoinManager
	{
		private LootScriptableObject LootData;

		public CoinManager(LootScriptableObject lootData)
		{
			LootData = lootData;
		}

		public void OnDropMoney(GameObject Obj, int amount)
		{
			for (int i = 0; i < amount / 10; i++)
			{
				GameObject coin10 = GameObject.Instantiate(LootData.GetPrefabFromName("Coin10"), Obj.transform.position, Quaternion.identity);
			}

			for (int i = 0; i < (amount % 10) / 3; i++)
			{
				GameObject coin3 = GameObject.Instantiate(LootData.GetPrefabFromName("Coin3"), Obj.transform.position, Quaternion.identity);
			}

			for (int i = 0; i < ((amount % 10) % 3) / 1; i++)
			{
				GameObject coin1 = GameObject.Instantiate(LootData.GetPrefabFromName("Coin1"), Obj.transform.position, Quaternion.identity);
			}
		}

		public void Destroy()
		{

		}
	}

}
