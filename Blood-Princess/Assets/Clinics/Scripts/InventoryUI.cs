using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Clinic
{
	[RequireComponent(typeof(Inventory))]
	public class InventoryUI : MonoBehaviour
	{
		public RectTransform InventoryPanel;

		private Inventory m_Inventory;
		private GameObject m_SelectionCursor;
		private int m_CurrentSelectionIndex;

		private void Awake()
		{
			m_Inventory = GetComponent<Inventory>();
			for (int i = 0; i < m_Inventory.MaxSlots; i++)
			{
				Instantiate(Resources.Load("Slot") as GameObject, InventoryPanel);
			}
		}

		public void OnAddItem(Item item, int index, int num = 1)
		{
			InventoryPanel.GetChild(index).GetComponent<Image>().sprite = item.Sprite;
		}

		public void OnOpenUI()
		{
			InventoryPanel.gameObject.SetActive(true);
			m_SelectionCursor = Instantiate(Resources.Load("SelectionFrame") as GameObject, InventoryPanel.parent);
			m_SelectionCursor.GetComponent<RectTransform>().anchoredPosition = InventoryPanel.GetChild(0).GetComponent<RectTransform>().localPosition;
		}

		public void OnCloseUI()
		{
			InventoryPanel.gameObject.SetActive(false);
			Destroy(m_SelectionCursor);
			m_SelectionCursor = null;
		}
	}

}
