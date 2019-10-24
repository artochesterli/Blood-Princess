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

		/// <summary>
		/// 1  Pick first room position
		/// 2  Name the First Room
		/// 3  Find Critical Path
		/// </summary>
		//private void _setupBoard()
		//{
		//	// Setup Random using a specific Seed value (default 0)
		//	System.Random rand = new System.Random(_seed);

		//	// Pick the first room Position
		//	_currentCriticalPathPosition.x = rand.Next(0, _width - 1);
		//	_currentCriticalPathPosition.y = _height - 1;

		//	// Remeber First Room Position
		//	_firstRoomPosition.x = _currentCriticalPathPosition.x;
		//	_firstRoomPosition.y = _currentCriticalPathPosition.y;

		//	// Set the last room position
		//	_lastCriticalPathPosition.x = _currentCriticalPathPosition.x;
		//	_lastCriticalPathPosition.y = _currentCriticalPathPosition.y;

		//	// Name the First Room, usually a room 1 type or room 2 type
		//	_board[_currentCriticalPathPosition.x, _currentCriticalPathPosition.y] = rand.Next(1, 3);

		//	do
		//	{
		//		// Generate a random number 1..5
		//		// 1/2 -- Left
		//		// 3/4 -- Right
		//		// 5 -- Down
		//		int pathDirection = rand.Next(1, 6);

		//		// If the last direction is left/right
		//		// Then this direction needs to continue left/right
		//		// Unless it is going down
		//		if (_lastCriticalPathDirection != 0 && _lastCriticalPathDirection <= 2 && pathDirection != 5)
		//		{
		//			pathDirection = 1;
		//		}
		//		else if (_lastCriticalPathDirection != 0 && _lastCriticalPathDirection <= 4 && _lastCriticalPathDirection > 2
		//		&& pathDirection != 5)
		//		{
		//			pathDirection = 3;
		//		}
		//		Console.Write(pathDirection);
		//		// Go to next Position based on pathDirection
		//		if (pathDirection <= 2)
		//		{
		//			_currentCriticalPathPosition.x--;
		//			_lastCriticalPathDirection = 1;
		//			// If This position hits a wall, then go down and turn Right
		//			if (_currentCriticalPathPosition.x < 0)
		//			{
		//				_currentCriticalPathPosition.x = 0;
		//				_currentCriticalPathPosition.y--;
		//				_lastCriticalPathDirection = 3;
		//				if (_lastCriticalPathPosition.y + 1 < _height &&
		//				   (_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 2 ||
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 4))
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 4;
		//				else
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 2;
		//			}
		//		}
		//		else if (pathDirection > 2 && pathDirection <= 4)
		//		{
		//			_currentCriticalPathPosition.x++;
		//			_lastCriticalPathDirection = 3;
		//			// If this position hits a wall, then go down and turn left
		//			if (_currentCriticalPathPosition.x >= _width)
		//			{
		//				_currentCriticalPathPosition.x = _width - 1;
		//				_currentCriticalPathPosition.y--;
		//				_lastCriticalPathDirection = 1;
		//				// Check if last room was 2
		//				if (_lastCriticalPathPosition.y + 1 < _height &&
		//				   (_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 2 ||
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 4))
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 4;
		//				else
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 2;
		//			}
		//		}
		//		else
		//		{
		//			// Just Go Down
		//			_currentCriticalPathPosition.y--;
		//			_lastCriticalPathDirection = 5;
		//			if (_lastCriticalPathPosition.y + 1 < _height &&
		//				   (_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 2 ||
		//					_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 4))
		//				_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 4;
		//			else
		//				_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 2;
		//		}

		//		// On Default, Set the current moved room to room type 1
		//		_board[_currentCriticalPathPosition.x, _currentCriticalPathPosition.y] = 1;
		//		// If the last room was on top and was a 2/4
		//		// Then this one must be a 3 or 4
		//		if (_lastCriticalPathPosition.x == _currentCriticalPathPosition.x &&
		//			_lastCriticalPathPosition.y == _currentCriticalPathPosition.y + 1)
		//		{
		//			_board[_currentCriticalPathPosition.x, _currentCriticalPathPosition.y] = rand.Next(3, 5);

		//		}
		//		_lastCriticalPathPosition.x = _currentCriticalPathPosition.x;
		//		_lastCriticalPathPosition.y = _currentCriticalPathPosition.y;

		//	} while (_currentCriticalPathPosition.y > 0 && _currentCriticalPathPosition.x >= 0 &&
		//	_currentCriticalPathPosition.x < _width);
		//}

		//private void _fillUpBoard()
		//{
		//	//for (int i = _height - 1; i >= 0; i--)
		//	//{
		//	//	for (int j = 0; j < _width; j++)
		//	//	{
		//	//		Room rm = new Room(new IntVector2(j, i), new IntVector2(_width, _height), _seed, _board[j, i], _boardGameObject);
		//	//		if (_firstRoomPosition.x == j && _firstRoomPosition.y == i)
		//	//			rm.GeneratePlayer();
		//	//	}
		//	//}
		//}

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
