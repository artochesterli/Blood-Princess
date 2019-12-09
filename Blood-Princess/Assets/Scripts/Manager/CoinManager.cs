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
			int minCoinNum = 6;
			int coinTenNum = amount / 10;
			int coinThreeNum = (amount % 10) / 3;
			int coinOneNum = ((amount % 10) % 3) / 1;
			int curCoinNum = coinTenNum + coinThreeNum + coinOneNum;
			if (curCoinNum < minCoinNum)
			{
				if (coinTenNum > 0)
				{
					// 10 → 3 + 3 + 3 + 1
					coinTenNum--;
					coinThreeNum += 3;
					coinOneNum++;
					curCoinNum = coinTenNum + coinThreeNum + coinOneNum;
				}
				
				while (curCoinNum < minCoinNum)
				{
					if (coinThreeNum < 1)
					{
						break;
					}
					// 3 → 1 + 1 + 1
					coinThreeNum --;
					coinOneNum+=3;
					curCoinNum = coinTenNum + coinThreeNum + coinOneNum;
				}
			}
			for (int i = 0; i < coinTenNum; i++)
			{
				GameObject coin10 = GameObject.Instantiate(LootData.GetPrefabFromName("Coin10"), Obj.transform.position, Quaternion.identity);
			}

			for (int i = 0; i < coinThreeNum; i++)
			{
				GameObject coin3 = GameObject.Instantiate(LootData.GetPrefabFromName("Coin3"), Obj.transform.position, Quaternion.identity);
			}

			for (int i = 0; i < coinOneNum; i++)
			{
				GameObject coin1 = GameObject.Instantiate(LootData.GetPrefabFromName("Coin1"), Obj.transform.position, Quaternion.identity);
			}
		}

		public void Destroy()
		{

		}
	}

}
