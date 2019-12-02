using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	[RequireComponent(typeof(CraftingTable))]
	public class InteractableCraftingTable : Interactable
	{
		private CraftingTable m_CraftingTable;

		protected override void Awake()
		{
			base.Awake();
			m_CraftingTable = GetComponent<CraftingTable>();
		}

		protected override void OnCancelInteract()
		{
			m_CraftingTable.OnCloseUI();
		}

		protected override void OnInteract()
		{
			m_CraftingTable.OnOpenUI();
		}
	}

}
