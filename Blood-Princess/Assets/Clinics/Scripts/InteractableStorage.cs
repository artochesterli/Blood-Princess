using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(InventoryUI))]
    public class InteractableStorage : Interactable
    {
        public GameObject WorldCanvas;
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
            Services.StorageManager.SaveItem(new EmptyItem());
        }

        protected override void OnCancelInteract()
        {
            ControlStateManager.CurrentControlState = ControlState.Action;
            m_InventoryUI.OnCloseUI();
        }

        protected override void OnInteract()
        {
            ControlStateManager.CurrentControlState = ControlState.Storage;
            m_InventoryUI.OnOpenUI();
        }

        protected override void OnEnterZone()
        {
            WorldCanvas.SetActive(true);
        }

        protected override void OnExitZone()
        {
            WorldCanvas.SetActive(false);
        }
    }

}
