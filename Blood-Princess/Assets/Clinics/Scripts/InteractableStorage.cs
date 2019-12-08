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
			Services.StorageManager.SaveItem(new Rug(m_ItemData), 1);
			Services.StorageManager.SaveItem(new Wood(m_ItemData), 1);
			Services.StorageManager.SaveItem(new Cloth(m_ItemData), 2);
			Services.StorageManager.SaveItem(new OakSeed(m_ItemData), 1);
			Services.StorageManager.SaveItem(new Scroll(m_ItemData), 1);
		}

		protected override void OnCancelInteract()
		{
			m_InventoryUI.OnCloseUI();
		}

		protected override void OnInteract()
		{
			m_InventoryUI.OnOpenUI();
		}

		protected override void OnEnterZone()
		{
			return;
		}

		protected override void OnExitZone()
		{
		}
	}

}
