using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Clinic
{
	public class Inventory : MonoBehaviour
	{
		public int MaxSlots = 15;
		private ItemData ItemData;
		public List<Item> m_Items { get; set; }

		private void Awake()
		{
			ItemData = Resources.Load<ItemData>("ItemData");

			m_Items = new List<Item>();
			for (int i = 0; i < MaxSlots; i++)
			{
				m_Items.Add(new EmptyItem(ItemData));
			}
		}

		/// <summary>
		/// Takes an item and try to add it to the inventory
		/// </summary>
		/// <param name="item">Item to add</param>
		/// <returns>true if successfully added</returns>
		public bool OnAddItem(Item item)
		{
			InventoryUI inUI = GetComponent<InventoryUI>();
			for (int i = 0; i < m_Items.Count; i++)
			{
				if (m_Items[i].GetType().Equals(item.GetType()))
				{
					m_Items[i].Number++;
					if (inUI != null)
						inUI.OnAddItem(item, i, m_Items[i].Number);
					return true;
				}
			}
			for (int i = 0; i < m_Items.Count; i++)
			{
				if (m_Items[i].GetType().Equals(typeof(EmptyItem)))
				{
					m_Items[i] = item;
					if (inUI != null)
						inUI.OnAddItem(item, i, m_Items[i].Number);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Remove an item in the inventory
		/// </summary>
		/// <param name="item"></param>
		public void OnRemoveItem(Item item, int num = 1)
		{
			InventoryUI inUI = GetComponent<InventoryUI>();
			for (int i = 0; i < m_Items.Count; i++)
			{
				if (m_Items[i].GetType().Equals(item.GetType()))
				{
					m_Items[i].Number -= num;
					if (inUI != null)
						inUI.OnRemoveItem(item, i, m_Items[i].Number, m_Items[i].Number <= 0);
					if (m_Items[i].Number <= 0)
					{
						m_Items[i] = new EmptyItem(ItemData);
					}
					return;
				}
			}
		}


	}

}
