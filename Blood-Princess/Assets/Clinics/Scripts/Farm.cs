using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Clinic
{
	public class Farm : MonoBehaviour
	{
		public GameObject FarmPanel;
		public GameObject Selection;
		public Inventory Inventory;

		private ItemData m_ItemData;
		private FSM<Farm> m_FarmFSM;
		private List<Item> m_Items
		{
			get
			{
				return Inventory.m_Items;
			}
		}

		private void Awake()
		{
			m_FarmFSM = new FSM<Farm>(this);
			m_FarmFSM.TransitionTo<FarmEmptyCloseState>();
			m_ItemData = Resources.Load<ItemData>("ItemData");
		}

		private void Update()
		{
			m_FarmFSM.Update();
		}

		public void OnOpenUI()
		{
			if (m_FarmFSM.CurrentState.GetType().Equals(typeof(FarmEmptyCloseState)))
				m_FarmFSM.TransitionTo<FarmEmptyOpenState>();
		}

		public void OnCloseUI()
		{
			if (m_FarmFSM.CurrentState.GetType().Equals(typeof(FarmEmptyOpenState)))
				m_FarmFSM.TransitionTo<FarmEmptyCloseState>();
		}

		private abstract class FarmState : FSM<Farm>.State
		{
		}

		private class FarmEmptyCloseState : FarmState
		{
			public override void OnEnter()
			{
				base.OnEnter();
				Context.FarmPanel.SetActive(false);
				Context.Selection.SetActive(false);
			}
		}

		private class FarmEmptyOpenState : FarmState
		{
			private int m_CurrentSelection;

			public override void OnEnter()
			{
				base.OnEnter();
				m_CurrentSelection = 0;
				Context.FarmPanel.SetActive(true);
				Context.Selection.SetActive(true);
				m_SetUpFarmPanel();
			}

			public override void Update()
			{
				base.Update();
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					if (m_CurrentSelection + 1 < Context.FarmPanel.transform.childCount)
					{
						m_SetSelectionCursor(++m_CurrentSelection);
					}
				}

				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if (m_CurrentSelection - 1 >= 0)
					{
						m_SetSelectionCursor(--m_CurrentSelection);
					}
				}
			}

			private void m_SetSelectionCursor(int index)
			{
				Context.Selection.GetComponent<RectTransform>().position = Context.FarmPanel.transform.GetChild(index).GetComponent<RectTransform>().position;
			}

			private void m_SetUpFarmPanel()
			{
				// Destroy First
				// Destroy all child of a material panel
				foreach (Transform child in Context.FarmPanel.transform)
				{
					Destroy(child.gameObject);
				}

				foreach (Item item in Context.m_Items)
				{
					if (!item.GetType().BaseType.Equals(typeof(SeedItem)))
						continue;
					SeedItem si = (SeedItem)item;
					GameObject go = Instantiate(Resources.Load("Seed") as GameObject, Context.FarmPanel.transform);
					go.transform.GetChild(0).GetComponent<Image>().sprite = si.Sprite;
					go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = si.Number.ToString();
				}
				m_SetSelectionCursor(m_CurrentSelection);
			}
		}
	}

}
