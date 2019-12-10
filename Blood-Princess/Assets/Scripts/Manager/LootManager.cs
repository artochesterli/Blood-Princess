using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompiledTypes;

namespace Loot
{
	public class LootManager
	{
		private TextAsset m_CastleDBAsset;
		private LootScriptableObject m_LootData;

		private CastleDB m_DB;

		public LootManager(TextAsset m_CastleDBAsset, LootScriptableObject m_LootData)
		{
			this.m_CastleDBAsset = m_CastleDBAsset;
			this.m_LootData = m_LootData;
			m_DB = new CastleDB(m_CastleDBAsset);
			EventManager.instance.AddHandler<PlayerKillEnemy>(m_OnEnemyDied);
		}

		public void Destroy()
		{
			EventManager.instance.RemoveHandler<PlayerKillEnemy>(m_OnEnemyDied);
		}

		private void m_OnEnemyDied(PlayerKillEnemy ev)
		{
			foreach (var monster in m_DB.MonsterDB.GetAll())
			{
				if (ev.Enemy.name.ToLower().Contains(monster.id.ToLower()))
				{
					float roll = Random.Range(0f, 1f);
					float x = 0f;
					for (int i = 0; i < monster.DropsList.Count; i++)
					{
						x += monster.DropsList[i].Weight;
						if (roll < x)
						{
							string dropName = monster.DropsList[i].Item.id;
							if (dropName.ToLower() == "money")
							{
								Services.CoinManager.OnDropMoney(ev.Enemy, monster.DropsList[i].Quantity);
							}
							else
							{
								// TODO: Drop other stuff
							}
							break;
						}
					}
					break;
				}
			}
		}
	}

}


