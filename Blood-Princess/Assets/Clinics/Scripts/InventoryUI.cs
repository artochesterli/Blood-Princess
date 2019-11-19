using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Clinic
{
	[RequireComponent(typeof(Inventory))]
	public class InventoryUI : MonoBehaviour
	{
		public RectTransform InventoryPanel;
		public GameObject ClinicGrid;

		private Inventory m_Inventory;
		private GameObject m_SelectionCursor;
		private DecorationItem m_DecorationItem;
		private int m_CurrentSelectionIndex;

		private FSM<InventoryUI> m_InventoryFSM;

		private void Awake()
		{
			m_Inventory = GetComponent<Inventory>();
			for (int i = 0; i < m_Inventory.MaxSlots; i++)
			{
				Instantiate(Resources.Load("Slot") as GameObject, InventoryPanel);
			}
			m_InventoryFSM = new FSM<InventoryUI>(this);
			m_InventoryFSM.TransitionTo<InventoryClosedState>();
		}

		private void Update()
		{
			m_InventoryFSM.Update();
		}

		public void OnAddItem(Item item, int index, int num = 1)
		{
			InventoryPanel.GetChild(index).GetChild(1).GetComponent<Image>().sprite = item.Sprite;
			InventoryPanel.GetChild(index).GetChild(1).GetComponent<Image>().color = Color.white;
			if (num > 1)
				InventoryPanel.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().text = num.ToString();
			else
				InventoryPanel.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
		}

		public void OnRemoveItem(Item item, int index, int num, bool toEmpty)
		{
			if (toEmpty)
			{
				InventoryPanel.GetChild(index).GetChild(1).GetComponent<Image>().sprite = null;
				InventoryPanel.GetChild(index).GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
				InventoryPanel.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
			}
			else
			{
				if (num > 1)
					InventoryPanel.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().text = num.ToString();
				else
					InventoryPanel.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
			}
		}

		public void OnOpenUI()
		{
			if (m_InventoryFSM.CurrentState.GetType().Equals(typeof(InventoryClosedState)))
				m_InventoryFSM.TransitionTo<InventoryOpenState>();
		}

		public void OnCloseUI()
		{
			if (m_InventoryFSM.CurrentState.GetType().Equals(typeof(InventoryOpenState)))
				m_InventoryFSM.TransitionTo<InventoryClosedState>();
		}

		public void OnUseDecoration(DecorationItem di)
		{
			if (m_InventoryFSM.CurrentState.GetType().Equals(typeof(InventoryOpenState)))
			{
				m_DecorationItem = di;
				m_InventoryFSM.TransitionTo<InventoryBuildState>();
			}
		}

		private abstract class InventoryUIState : FSM<InventoryUI>.State
		{
		}

		private class InventoryClosedState : InventoryUIState
		{
			public override void OnEnter()
			{
				base.OnEnter();
				Context.InventoryPanel.gameObject.SetActive(false);
				Destroy(Context.m_SelectionCursor);
				Context.m_SelectionCursor = null;
			}
		}

		private class InventoryOpenState : InventoryUIState
		{
			private int m_CurrentSelectionIndex = 0;
			public override void OnEnter()
			{
				base.OnEnter();
				Context.InventoryPanel.gameObject.SetActive(true);
				Context.m_SelectionCursor = Instantiate(Resources.Load("SelectionFrame") as GameObject, Context.InventoryPanel.parent);
				m_MoveCursorToIndex(0);
				m_CurrentSelectionIndex = 0;
			}

			public override void Update()
			{
				base.Update();
				m_MoveCursor();
			}

			private void m_MoveCursorToIndex(int index)
			{
				Context.m_SelectionCursor.GetComponent<RectTransform>().anchoredPosition = Context.InventoryPanel.GetChild(index).GetComponent<RectTransform>().localPosition;
			}

			private void m_MoveCursor()
			{
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
					if (m_CurrentSelectionIndex + 1 < Context.m_Inventory.MaxSlots)
					{
						m_CurrentSelectionIndex++;
						m_MoveCursorToIndex(m_CurrentSelectionIndex);
					}
				}

				if (Input.GetKeyDown(KeyCode.J))
				{
					Context.m_Inventory.m_Items[m_CurrentSelectionIndex].OnSelect(Context.gameObject);
				}
			}
		}

		private class InventoryBuildState : InventoryUIState
		{
			private Vector2Int BuildPosition;
			private BaseBuildingGrid BaseBuildingGrid;
			private DecorationItem DecorationItem;

			public override void OnEnter()
			{
				base.OnEnter();
				Context.InventoryPanel.gameObject.SetActive(false);
				Destroy(Context.m_SelectionCursor);
				Context.m_SelectionCursor = null;
				Context.ClinicGrid.SetActive(true);
				BuildPosition = new Vector2Int();
				BaseBuildingGrid = Context.ClinicGrid.GetComponent<BaseBuildingGrid>();
				DecorationItem = Context.m_DecorationItem;
				m_FitPosition(BuildPosition);
				// Make Camera Follow this 
				Camera.main.GetComponent<CameraManager>().Character = DecorationItem.ItemInstance;
			}

			public override void Update()
			{
				base.Update();
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					if (m_PositionFitBoard(new Vector2Int(BuildPosition.x + 1, BuildPosition.y), DecorationItem, BaseBuildingGrid))
					{
						BuildPosition.x++;
						m_FitPosition(BuildPosition);
					}
				}
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					if (m_PositionFitBoard(new Vector2Int(BuildPosition.x - 1, BuildPosition.y), DecorationItem, BaseBuildingGrid))
					{
						BuildPosition.x--;
						m_FitPosition(BuildPosition);
					}
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if (m_PositionFitBoard(new Vector2Int(BuildPosition.x, BuildPosition.y - 1), DecorationItem, BaseBuildingGrid))
					{
						BuildPosition.y--;
						m_FitPosition(BuildPosition);
					}
				}
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					if (m_PositionFitBoard(new Vector2Int(BuildPosition.x, BuildPosition.y + 1), DecorationItem, BaseBuildingGrid))
					{
						BuildPosition.y++;
						m_FitPosition(BuildPosition);
					}
				}

				if (Input.GetKeyDown(KeyCode.J))
				{
					if (m_CanPlace(BuildPosition, DecorationItem, BaseBuildingGrid))
					{
						m_Place(BuildPosition, DecorationItem, BaseBuildingGrid);
						GameObject.Instantiate(DecorationItem.ItemInstance);
						Context.m_Inventory.OnRemoveItem(DecorationItem);
						TransitionTo<InventoryClosedState>();
						return;
					}
				}

				if (Input.GetKeyDown(KeyCode.K))
				{
					TransitionTo<InventoryClosedState>();
					return;
				}
			}

			private void m_FitPosition(Vector2Int Pos)
			{
				// Purge all Grids' Color First
				for (int i = 0; i < BaseBuildingGrid.Grids.GetLength(0); i++)
				{
					for (int j = 0; j < BaseBuildingGrid.Grids.GetLength(1); j++)
					{
						if (BaseBuildingGrid.Grids[i, j].gameObject.GetComponent<SpriteRenderer>().color != Color.black)
						{
							BaseBuildingGrid.Grids[i, j].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
						}
					}
				}
				// First Fit the actual gameObject's upper left corner to the grids upper left corner
				DecorationItem.ItemInstance.transform.position = BaseBuildingGrid.Grids[Pos.x, Pos.y].gameObject.transform.position;

				// Check grid availability and pre change grid's color to cater to the avaiability
				int row = DecorationItem.OccupySize.GetLength(0);
				int column = DecorationItem.OccupySize.GetLength(1);

				bool canPlace = m_CanPlace(Pos, DecorationItem, BaseBuildingGrid);
				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < column; j++)
					{
						Grid targetGrid = BaseBuildingGrid.Grids[Pos.x + i, Pos.y + j];
						// If the Grid was pre-occupied(i.e black), continue and do not change color
						// If the Grid was the same type as required, then green, other wise, red
						if (targetGrid.gameObject.GetComponent<SpriteRenderer>().color == Color.black)
						{
							continue;
						}

						if (targetGrid.GridName == DecorationItem.OccupySize[i, j] && canPlace)
						{
							targetGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
						}
						else
						{
							targetGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
						}
					}
				}
				//for (int i = 0; i < row; i++)
				//{
				//	for (int j = 0; j < column; j++)
				//	{
				//		Grid targetGrid = BaseBuildingGrid.Grids[Pos.x + i, Pos.y + j];
				//		// If the Grid was pre-occupied(i.e black), continue and do not change color
				//		// If the Grid was the same type as required, then green, other wise, red
				//		if (targetGrid.gameObject.GetComponent<SpriteRenderer>().color == Color.black)
				//		{
				//			continue;
				//		}

				//		if (targetGrid.GridName == DecorationItem.OccupySize[i, j])
				//		{
				//			targetGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
				//		}
				//		else
				//		{
				//			targetGrid.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
				//		}
				//	}
				//}

			}

			private void m_Place(Vector2Int Pos, DecorationItem di, BaseBuildingGrid bbg)
			{
				int row = di.OccupySize.GetLength(0);
				int column = di.OccupySize.GetLength(1);

				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < column; j++)
					{
						bbg.Grids[Pos.x + i, Pos.y + j].gameObject.GetComponent<SpriteRenderer>().color = Color.black;
					}
				}
			}

			/// <summary>
			/// Check if can place
			/// </summary>
			/// <param name="Pos"></param>
			/// <param name="di"></param>
			/// <param name="bbg"></param>
			/// <returns></returns>
			private bool m_CanPlace(Vector2Int Pos, DecorationItem di, BaseBuildingGrid bbg)
			{
				int row = di.OccupySize.GetLength(0);
				int column = di.OccupySize.GetLength(1);

				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < column; j++)
					{
						if (bbg.Grids[Pos.x + i, Pos.y + j].gameObject.GetComponent<SpriteRenderer>().color != Color.green &&
							bbg.Grids[Pos.x + i, Pos.y + j].gameObject.GetComponent<SpriteRenderer>().color != Color.white)
							return false;
					}
				}
				return true;
			}

			/// <summary>
			/// Check if Item's Pos is fully within the board posiiton, assuming Upper Left alignment
			/// </summary>
			/// <param name="Pos"></param>
			/// <param name="di"></param>
			/// <returns></returns>
			private bool m_PositionFitBoard(Vector2Int Pos, DecorationItem di, BaseBuildingGrid bbg)
			{
				int row = di.OccupySize.GetLength(0);
				int column = di.OccupySize.GetLength(1);

				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < column; j++)
					{
						if (Pos.x + i > bbg.RowNum - 1)
							return false;
						if (Pos.x + i < 0)
							return false;
						if (Pos.y + j > bbg.ColumnNum - 1)
							return false;
						if (Pos.y + j < 0)
							return false;
					}
				}
				return true;
			}

			public override void OnExit()
			{
				base.OnExit();
				Camera.main.GetComponent<CameraManager>().Character = GameObject.FindGameObjectWithTag("Player");
				GameObject.Destroy(DecorationItem.ItemInstance);
				DecorationItem.ItemInstance = null;
				Context.ClinicGrid.SetActive(false);
			}
		}
	}

}
