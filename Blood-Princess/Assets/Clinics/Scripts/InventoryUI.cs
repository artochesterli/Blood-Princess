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

		private void Update()
		{
			m_MoveCursor();
		}

		private void m_MoveCursor()
		{
			if (m_SelectionCursor == null) return;
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if (m_CurrentSelectionIndex - 1 >= 0)
				{
					m_CurrentSelectionIndex--;
					m_MoveCursorToIndex(m_CurrentSelectionIndex);
				}
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				if (m_CurrentSelectionIndex + 1 < m_Inventory.MaxSlots)
				{
					m_CurrentSelectionIndex++;
					m_MoveCursorToIndex(m_CurrentSelectionIndex);
				}
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				m_Inventory.m_Items[m_CurrentSelectionIndex].OnSelect(GameObject.FindGameObjectWithTag("Player"));
			}
		}

		private void m_MoveCursorToIndex(int index)
		{
			m_SelectionCursor.GetComponent<RectTransform>().anchoredPosition = InventoryPanel.GetChild(index).GetComponent<RectTransform>().localPosition;
		}

		public void OnAddItem(Item item, int index, int num = 1)
		{
			InventoryPanel.GetChild(index).GetComponent<Image>().sprite = item.Sprite;
		}

		public void OnOpenUI()
		{
			InventoryPanel.gameObject.SetActive(true);
			m_SelectionCursor = Instantiate(Resources.Load("SelectionFrame") as GameObject, InventoryPanel.parent);
			m_MoveCursorToIndex(0);
		}

		public void OnCloseUI()
		{
			InventoryPanel.gameObject.SetActive(false);
			Destroy(m_SelectionCursor);
			m_SelectionCursor = null;
		}
	}

}
