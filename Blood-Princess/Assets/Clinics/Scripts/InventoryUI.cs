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
		private GameObject m_CurrentInstantiatedObject;

		private void Awake()
		{
			m_Inventory = GetComponent<Inventory>();
			m_SelectionCursor = Instantiate(Resources.Load("SelectionFrame") as GameObject, InventoryPanel.parent);
			m_InventoryFSM = new FSM<InventoryUI>(this);
			m_InventoryFSM.TransitionTo<InventoryClosedState>();
		}

		private void Update()
		{
			m_InventoryFSM.Update();
		}

		/// <summary>
		/// Read Item from disk to UI
		/// </summary>
		public void ReadItem()
		{
			List<Item> items = Services.StorageManager.LoadItem();
			for (int i = 0; i < items.Count; i++)
			{
				Item curitem = items[i];
				Transform curPanel = InventoryPanel.GetChild(i);
				if (!curitem.GetType().Equals(typeof(EmptyItem)))
				{
					curPanel.GetChild(1).GetComponent<Image>().sprite = curitem.GetID().Sprite;
					curPanel.GetChild(1).GetComponent<Image>().color = Color.white;
					if (curitem.Number > 1)
					{
						curPanel.GetChild(2).GetComponent<TextMeshProUGUI>().text = curitem.Number.ToString();
					}
					else
					{
						curPanel.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
					}
				}
				else
				{
					curPanel.GetChild(1).GetComponent<Image>().sprite = null;
					curPanel.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
					curPanel.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
				}

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
				Context.m_SelectionCursor.SetActive(false);
			}
		}

		private class InventoryOpenState : InventoryUIState
		{
			private int m_CurrentSelectionIndex = 0;
			private bool m_FirstFrameSkip = false;
			public override void OnEnter()
			{
				base.OnEnter();
				Context.ReadItem();
				Context.InventoryPanel.gameObject.SetActive(true);
				Context.m_SelectionCursor.SetActive(true);
				m_MoveCursorToIndex(0);
				m_CurrentSelectionIndex = 0;
				m_FirstFrameSkip = false;
			}

			public override void Update()
			{
				base.Update();
				if (!m_FirstFrameSkip)
				{
					m_FirstFrameSkip = true;
					return;
				}
				m_MoveCursor();
			}

			private void m_MoveCursorToIndex(int index)
			{
				Context.m_SelectionCursor.GetComponent<RectTransform>().localPosition = Context.InventoryPanel.GetChild(index).GetComponent<RectTransform>().localPosition;
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
					if (m_CurrentSelectionIndex + 1 < Services.StorageManager.MaxSlots)
					{
						m_CurrentSelectionIndex++;
						m_MoveCursorToIndex(m_CurrentSelectionIndex);
					}
				}

				if (Input.GetKeyDown(KeyCode.J))
				{
					Item curItem = Services.StorageManager.LoadItem()[m_CurrentSelectionIndex];
					if (curItem.GetType().IsSubclassOf(typeof(DecorationItem)))
					{
						Context.m_CurrentInstantiatedObject = ((DecorationItem)curItem).OnSelectObject(Context.gameObject);
					}
					else
					{
						curItem.OnSelect(Context.gameObject);
					}
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
				Context.m_SelectionCursor.SetActive(false);

				Context.ClinicGrid.SetActive(true);
				BuildPosition = new Vector2Int();
				BaseBuildingGrid = Context.ClinicGrid.GetComponent<BaseBuildingGrid>();
				DecorationItem = Context.m_DecorationItem;
				m_FitPosition(BuildPosition);
				// Make Camera Follow this 
				Camera.main.GetComponent<CameraManager>().Character = Context.m_CurrentInstantiatedObject;
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
						GameObject.Instantiate(Context.m_CurrentInstantiatedObject);
						Services.StorageManager.SaveItem(DecorationItem, -1);
						Services.HomeManager.OnSave(BuildPosition, DecorationItem);
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
						if (BaseBuildingGrid.Grids[i, j].GridState != GridState.Occupied)
						{
							BaseBuildingGrid.Grids[i, j].GridState = GridState.Empty;
						}
					}
				}
				// First Fit the actual gameObject's upper left corner to the grids upper left corner
				Context.m_CurrentInstantiatedObject.transform.position = BaseBuildingGrid.Grids[Pos.x, Pos.y].gameObject.transform.position;

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
						if (targetGrid.GridState == GridState.Occupied)
						{
							continue;
						}

						if (targetGrid.GridName == DecorationItem.OccupySize[i, j] && canPlace)
						{
							targetGrid.GridState = GridState.CanPlace;
						}
						else
						{
							targetGrid.GridState = GridState.CannotPlace;
						}
					}
				}
			}

			private void m_Place(Vector2Int Pos, DecorationItem di, BaseBuildingGrid bbg)
			{
				int row = di.OccupySize.GetLength(0);
				int column = di.OccupySize.GetLength(1);

				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < column; j++)
					{
						bbg.Grids[Pos.x + i, Pos.y + j].GridState = GridState.Occupied;
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
						if (bbg.Grids[Pos.x + i, Pos.y + j].GridState != GridState.CanPlace &&
							bbg.Grids[Pos.x + i, Pos.y + j].GridState != GridState.Empty)
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
				GameObject.Destroy(Context.m_CurrentInstantiatedObject);
				Context.ClinicGrid.SetActive(false);
			}
		}
	}

}
