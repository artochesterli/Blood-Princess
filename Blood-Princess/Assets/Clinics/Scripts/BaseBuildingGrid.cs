﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	public class BaseBuildingGrid : MonoBehaviour
	{
		public Grid[,] Grids;
		public int ColumnNum = 12;
		public int RowNum = 4;

		private Vector2 WallGridSize { get { return Utility.WallTileSize(); } }
		private Vector2 GroundGridSize { get { return Utility.GroundTileSize(); } }
		private MaterialItem m_MaterialItem;

		private void Awake()
		{
			Grids = new Grid[RowNum, ColumnNum];
			int rowLength = Grids.GetLength(0);
			int columnLength = Grids.GetLength(1);
			for (int i = 0; i < rowLength - 1; i++)
			{
				for (int j = 0; j < columnLength; j++)
				{
					Grids[i, j] = new WallGrid(new Vector2Int(i, j), Instantiate(Resources.Load("WallGrid") as GameObject, transform));
					Grids[i, j].WorldPosition = new Vector2(j * WallGridSize.x, -i * WallGridSize.y);
					Grids[i, j].gameObject.transform.localPosition = Grids[i, j].WorldPosition;
				}
			}

			for (int j = 0; j < columnLength; j++)
			{
				int i = rowLength - 1;
				Grids[i, j] = new GroundGrid(new Vector2Int(i, j), Instantiate(Resources.Load("GroundGrid") as GameObject, transform));
				Grids[i, j].WorldPosition = new Vector2(j * WallGridSize.x, -i * WallGridSize.y + 0.3f);
				Grids[i, j].gameObject.transform.localPosition = Grids[i, j].WorldPosition;

			}

			m_OccupyCraftAndStorage();
		}

		private void m_OccupyCraftAndStorage()
		{
			for (int i = 1; i <= 3; i++)
			{
				for (int j = 0; j <= 2; j++)
				{
					Grids[i, j].GridState = GridState.Occupied;
				}
			}

			for (int i = 2; i <= 2; i++)
			{
				for (int j = 3; j <= 4; j++)
				{
					Grids[i, j].GridState = GridState.Occupied;
				}
			}
		}
	}
}

