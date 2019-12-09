using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Clinic
{
	/// <summary>
	/// UI and Inventory 2 in 1 script
	/// </summary>
	public class CraftingTable : MonoBehaviour
	{
		public Inventory Inventories;
		public GameObject CraftingPanel;
		public GameObject MaterialPanel;
		public GameObject Selection;

		private ItemData m_ItemData;
		private List<Item> m_Items
		{
			get
			{
				//return Inventories.m_Items;
				return Services.StorageManager.LoadItem();
			}
		}

		private FSM<CraftingTable> m_CraftingTableFSM;

		private void Awake()
		{
			m_ItemData = Resources.Load<ItemData>("ItemData");
			m_CraftingTableFSM = new FSM<CraftingTable>(this);
			m_CraftingTableFSM.TransitionTo<CraftTableClosedState>();
		}

		private void Update()
		{
			m_CraftingTableFSM.Update();
		}

		private int NumOfItem(string Name)
		{
			foreach (var item in m_Items)
			{
				if (item.Name.ToLower() == Name.ToLower())
				{
					return item.Number;
				}
			}
			return 0;
		}

		public void OnOpenUI()
		{
			if (m_CraftingTableFSM.CurrentState.GetType().Equals(typeof(CraftTableClosedState)))
				m_CraftingTableFSM.TransitionTo<CraftTableOpenState>();
		}

		public void OnCloseUI()
		{
			if (m_CraftingTableFSM.CurrentState.GetType().Equals(typeof(CraftTableOpenState)))
				m_CraftingTableFSM.TransitionTo<CraftTableClosedState>();
		}

		private abstract class CraftTableState : FSM<CraftingTable>.State
		{ }

		private class CraftTableClosedState : CraftTableState
		{
			public override void OnEnter()
			{
				base.OnEnter();
				Context.CraftingPanel.SetActive(false);
				Context.MaterialPanel.SetActive(false);
				Context.Selection.SetActive(false);
			}
		}

		private class CraftTableOpenState : CraftTableState
		{
			private int m_SelectionIndex;
			private bool m_CanMakeCurrentSelection;
			private List<CraftMaterial> m_CurrentCraftMaterialsNeeded;
			private bool m_FirstEntered = true;

			public override void OnEnter()
			{
				base.OnEnter();
				Context.CraftingPanel.SetActive(true);
				Context.MaterialPanel.SetActive(true);
				Context.Selection.SetActive(true);
				m_SetSelectionCursor(m_SelectionIndex);
				m_FirstEntered = true;
			}

			public override void Update()
			{
				base.Update();
				// Skip First Frame
				if (m_FirstEntered)
				{
					m_FirstEntered = false;
					return;
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					int index = m_SelectionIndex + 1;
					if (index < 0 || index > Context.CraftingPanel.transform.childCount - 1)
						return;
					m_SetSelectionCursor(++m_SelectionIndex);
				}

				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					int index = m_SelectionIndex - 1;
					if (index < 0 || index > Context.CraftingPanel.transform.childCount - 1)
						return;
					m_SetSelectionCursor(--m_SelectionIndex);
				}

				if (Input.GetKeyDown(KeyCode.J) && m_CanMakeCurrentSelection)
				{
					m_CraftCurrentSelection();
					m_SetSelectionCursor(m_SelectionIndex);
				}
			}

			private void m_CraftCurrentSelection()
			{
				// Remove Items required from storage
				foreach (var craftMaterial in m_CurrentCraftMaterialsNeeded)
				{
					//Context.Inventories.OnRemoveItem(Utility.NewItemFromString(craftMaterial.Name), craftMaterial.Amount);
					Services.StorageManager.SaveItem(Utility.NewItemFromString(craftMaterial.Name), -craftMaterial.Amount);
				}
				// Add items to storage
				//Context.Inventories.OnAddItem(Utility.NewItemFromString(Context.CraftingPanel.transform.GetChild(m_SelectionIndex).GetComponent<TextMeshProUGUI>().text));
				Services.StorageManager.SaveItem(Utility.NewItemFromString(Context.CraftingPanel.transform.GetChild(m_SelectionIndex).GetComponent<TextMeshProUGUI>().text));
			}

			private void m_SetSelectionCursor(int index)
			{
				// Destroy all child of a material panel
				foreach (Transform child in Context.MaterialPanel.transform.GetChild(0))
				{
					Destroy(child.gameObject);
				}

				Context.Selection.GetComponent<RectTransform>().position = Context.CraftingPanel.transform.GetChild(index).GetComponent<RectTransform>().position;
				m_CurrentCraftMaterialsNeeded = ((CraftableItemDatium)Context.m_ItemData.GetItem(Context.CraftingPanel.transform.GetChild(index).GetComponent<TextMeshProUGUI>().text)).CraftMaterials;
				m_CanMakeCurrentSelection = true;
				foreach (var craftMaterial in m_CurrentCraftMaterialsNeeded)
				{
					GameObject go = Instantiate(Resources.Load("MaterialsNeeded") as GameObject, Context.MaterialPanel.transform.GetChild(0));
					go.transform.GetChild(0).GetComponent<Image>().sprite = craftMaterial.Sprite;
					go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Context.NumOfItem(craftMaterial.Name).ToString() + " / " + craftMaterial.Amount.ToString();
					if (Context.NumOfItem(craftMaterial.Name) < craftMaterial.Amount)
					{
						go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
						m_CanMakeCurrentSelection = false;
					}
				}
			}
		}
	}

}
