using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	[RequireComponent(typeof(Inventory))]
	[RequireComponent(typeof(InventoryUI))]
	public class InteractableStorage : Interactable
	{
		private Inventory m_Inventory;
		private InventoryUI m_InventoryUI;
		private ItemData m_ItemData;

		protected override void Awake()
		{
			base.Awake();
			m_Inventory = GetComponent<Inventory>();
			m_InventoryUI = GetComponent<InventoryUI>();
			m_ItemData = Resources.Load<ItemData>("ItemData");
		}

		private void Start()
		{
			m_Inventory.OnAddItem(new Rug(m_ItemData));
			m_Inventory.OnAddItem(new Wood(m_ItemData));
			m_Inventory.OnAddItem(new Wood(m_ItemData));
			m_Inventory.OnAddItem(new Charcoal(m_ItemData));
		}

		protected override void OnCancelInteract()
		{
			m_InventoryUI.OnCloseUI();
		}

		protected override void OnInteract()
		{
			m_InventoryUI.OnOpenUI();
		}
	}

}
