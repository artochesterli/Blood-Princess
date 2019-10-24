using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PCG
{
	/// <summary>
	/// A board class that once constructed
	/// Give the whole map
	/// </summary>
	public class Board
	{
		private int _seed;
		private int _width;
		private int _height;
		/// <summary>
		/// board width as x
		/// board height as y
		/// down left corner is (0, 0)
		/// string value means the tile type
		/// </summary>
		private string[,] _board;
		private GameObject _boardGameObject;
		private System.Random m_Rand;

		public Board(int width, int height, int seed, int length)
		{
			_seed = seed;
			_width = width;
			_height = height;
			_board = new string[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					_board[i, j] = "";
				}
			}
			_boardGameObject = new GameObject("Map");
			_boardGameObject.transform.position = Vector2.zero;
			m_Rand = new System.Random(_seed);
			//_setupBoard();
			_setupBoard2(length);
			Print();
			//_fillupBoard();
		}

		private void _setupBoard2(int length)
		{
			IntVector2 startPosition = new IntVector2(1, _height / 2);
			Room entrance = new Room(startPosition, "8", _seed, 5, ref _board, _boardGameObject);
			startPosition = entrance.RoomExit.BoardPosition;
			startPosition.x += 1;
			for (int i = 0; i < length; i++)
			{
				Room newRoom = new Room(startPosition, "8", _seed, 3, ref _board, _boardGameObject);
				// Calculate the new Room's starting position and entry type
				startPosition = newRoom.RoomExit.BoardPosition;
				startPosition.x += 1;
			}
		}

		///
		/// Fill Up the Empty Part of the Board
		///
		private void _fillupBoard()
		{
			for (int i = 0; i < _width; i++)
			{
				for (int j = 0; j < _height; j++)
				{
					if (_board[i, j] == "")
					{
						Vector2 curTileWorldPosition = Vector2.zero +
							new Vector2(i * Utility.TileSize().x, j * Utility.TileSize().y);
						GameObject instantiatedObject = GameObject.Instantiate(Resources.Load("BlockTile" + m_Rand.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;
						instantiatedObject.transform.position = curTileWorldPosition;

					}
				}
			}
		}

		public void Print()
		{
			string _boardstr = "";
			for (int i = _height - 1; i >= 0; i--)
			{
				for (int j = 0; j < _width; j++)
				{
					_boardstr += _board[j, i];
				}
				_boardstr += "\n";
			}
		}
	}

	[Serializable]
	public struct IntVector2
	{
		public int x;
		public int y;

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static IntVector2 operator -(IntVector2 a, IntVector2 b)
		{
			return new IntVector2
			{
				x = a.x - b.x,
				y = a.y - b.y
			};
		}

		public static IntVector2 operator +(IntVector2 a, IntVector2 b)
		{
			return new IntVector2
			{
				x = a.x + b.x,
				y = a.y + b.y
			};
		}

		public override string ToString()
		{
			return x.ToString() + "," + y.ToString();
		}
	}
}
