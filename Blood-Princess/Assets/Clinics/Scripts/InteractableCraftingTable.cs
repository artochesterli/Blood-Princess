using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
    [RequireComponent(typeof(CraftingTable))]
    public class InteractableCraftingTable : Interactable
    {
        public GameObject WorldCanvas;
        private CraftingTable m_CraftingTable;

        protected override void Awake()
        {
            base.Awake();
            m_CraftingTable = GetComponent<CraftingTable>();
        }

        protected override void OnCancelInteract()
        {
            ControlStateManager.CurrentControlState = ControlState.Action;
            m_CraftingTable.OnCloseUI();
        }

        protected override void OnEnterZone()
        {
            WorldCanvas.SetActive(true);
        }

        protected override void OnExitZone()
        {
            WorldCanvas.SetActive(false);
        }

        protected override void OnInteract()
        {
            ControlStateManager.CurrentControlState = ControlState.CraftingTable;
            m_CraftingTable.OnOpenUI();
        }
    }

}
