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
            Debug.Log("Lootmanager ctor");
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
                    for (int i = 0; i < monster.DropsList.Count; i++)
                    {
                        float roll = Random.Range(0f, 1f);

                        if (roll < monster.DropsList[i].Weight)
                        {
                            string dropName = monster.DropsList[i].Item.id;
                            if (dropName.ToLower() == "money")
                            {
                                Services.CoinManager.OnDropMoney(ev.Enemy, monster.DropsList[i].Quantity);
                            }
                            else
                            {
                                // Drop other stuff
                                Debug.Log("Drop " + dropName);
                                GameObject theLoot = m_LootData.GetPrefabFromName(dropName);
                                Debug.Assert(theLoot != null, "Loot: " + dropName + " is not in loot data");
                                GameObject lootInstance = GameObject.Instantiate<GameObject>(theLoot, ev.Enemy.transform.position, Quaternion.identity);
                                lootInstance.GetComponent<SpeedManager>().SelfSpeed = new Vector2(Random.Range(-3, 3), Random.Range(3, 5));
                                if (dropName.ToLower().Contains("abilityobject"))
                                {
                                    lootInstance.GetComponent<AbilityObject>().PriceType = AbilityObjectPriceType.Drop;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

}


