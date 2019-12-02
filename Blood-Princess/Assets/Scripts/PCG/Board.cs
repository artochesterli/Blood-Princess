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
			_setupBoard(length);
			_fillupBoard();
			Print();
		}

		private void _setupBoard(int length)
		{
			IntVector2 startPosition = new IntVector2(1, _height / 2);
			Room entrance = new Room(startPosition, "L", _seed, 5, ref _board, _boardGameObject);
			startPosition = entrance.RoomExit.BoardPosition;
			startPosition.x += 1;
			for (int i = 0; i < length; i++)
			{
				Room newRoom = new Room(startPosition, "L", _seed, 3, ref _board, _boardGameObject);
				// Calculate the new Room's starting position and entry type
				startPosition = newRoom.RoomExit.BoardPosition;
				startPosition.x += 1;
			}
		}

		/// <summary>
		/// Fill Up the Board
		/// </summary>
		private void _fillupBoard()
		{
			for (int i = 0; i < _board.GetLength(0); i++)
			{
				for (int j = 0; j < _board.GetLength(1); j++)
				{
					IntVector2 curTileRelativePosition = new IntVector2(i, j);
					//IntVector2 curTileWorldPosition = curTileRelativePosition + m_BoardRoomOffset;
					string curChar = _board[i, j];
					// Always Assumes Board zero position is world zero position
					// Place Tile irw
					_placeTile(curChar, curTileRelativePosition);
				}
			}
		}

		private void _placeTile(string curChar, IntVector2 worldPosition)
		{
			Vector2 curTileWorldPosition = Vector2.zero +
				new Vector2(worldPosition.x * Utility.TileSize().x, worldPosition.y * Utility.TileSize().y);

			GameObject instantiatedObject = null;
			if (curChar == "1" || curChar == "3")
			{
				instantiatedObject = GameObject.Instantiate(Resources.Load("BlockTile" + m_Rand.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;

			}
			else if (curChar == "2")
			{
				if (m_Rand.Next(0, 100) > 50)
				{
					instantiatedObject = GameObject.Instantiate(Resources.Load("BlockTile" + m_Rand.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;
				}
			}
			else if (curChar == "5")
			{
				instantiatedObject = GameObject.Instantiate(Resources.Load("Ladder", typeof(GameObject))) as GameObject;
			}
			else if (curChar == "6")
			{
				instantiatedObject = GameObject.Instantiate(Resources.Load("PassablePlatform", typeof(GameObject))) as GameObject;
			}
			else if (curChar == "a")
			{
				int randInt = m_Rand.Next(0, 100);
				if (randInt > 45)
					instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
				else if (randInt > 15)
					instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemy2", typeof(GameObject))) as GameObject;
				else
					instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Knight", typeof(GameObject))) as GameObject;


				// initialize knight's Patrol Point and Engage Point
				_initializeAI(instantiatedObject, worldPosition);
			}
			else if (curChar == "b")
			{
				if (m_Rand.Next(0, 100) > 50)
				{
					if (m_Rand.Next(0, 100) > 30)
						instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
					else
						instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Knight", typeof(GameObject))) as GameObject;
					_initializeAI(instantiatedObject, worldPosition);
				}
			}
			else if (curChar == "p")
			{
				instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;
			}
			else if (curChar == "dummy")
			{
				instantiatedObject = GameObject.Instantiate(Resources.Load("Dummy", typeof(GameObject))) as GameObject;
			}

			if (instantiatedObject != null)
			{
				instantiatedObject.transform.parent = _boardGameObject.transform;
				instantiatedObject.transform.position = curTileWorldPosition;
				if (instantiatedObject.name.Contains("Knight"))
					instantiatedObject.transform.position = curTileWorldPosition + Vector2.up * 0.2f;
				else if (instantiatedObject.name.Contains("Enemy1"))
					instantiatedObject.transform.position = curTileWorldPosition + Vector2.up * 0.8f;
				else if (instantiatedObject.name.Contains("Passable"))
					instantiatedObject.transform.position = curTileWorldPosition + Vector2.up * 0.4f;
				else if (instantiatedObject.name.Contains("Enemy2"))
					instantiatedObject.transform.position = curTileWorldPosition + Vector2.up * 0.8f;
			}
		}

		private void _initializeAI(GameObject AI, IntVector2 worldPosition)
		{
			// Go Left and Check
			bool leftIsWall = false;
			bool leftIsEdge = false;
			int currentX = worldPosition.x;
			while (!leftIsWall && !leftIsEdge)
			{
				string leftDownPos = _board[currentX - 1, worldPosition.y - 1];
				string leftPos = _board[currentX - 1, worldPosition.y];
				leftIsWall = (leftPos != "" && leftPos != "0" && leftPos != "6" && leftPos != "e" && leftPos != "a" && leftPos != "b" && leftPos != "7");
				leftIsEdge = (leftPos == "" || leftPos == "0") && (leftDownPos == "" || leftDownPos == "0");
				currentX--;
			}

			if (AI.name.Contains("Knight"))
			{
				AI.transform.Find("PatronLeftMark").localPosition = Utility.BoardPositionToWorldPosition(new IntVector2(currentX + 2, worldPosition.y) - worldPosition);
				AI.transform.Find("DetectLeftMark").localPosition = Utility.BoardPositionToWorldPosition(new IntVector2(currentX, worldPosition.y) - worldPosition);
			}
			// Go Right and Check
			bool rightIsWall = false;
			bool RightIsEdge = false;
			currentX = worldPosition.x;
			while (!rightIsWall && !RightIsEdge)
			{
				string rightDownPos = _board[currentX + 1, worldPosition.y - 1];
				string rightPos = _board[currentX + 1, worldPosition.y];
				rightIsWall = (rightPos != "" && rightPos != "0" && rightPos != "6" && rightPos != "e" && rightPos != "a" && rightPos != "b" && rightPos != "7");
				RightIsEdge = (rightPos == "" || rightPos == "0") && (rightDownPos == "" || rightDownPos == "0");
				currentX++;
			}
			if (AI.name.Contains("Knight"))
			{
				AI.transform.Find("PatronRightMark").localPosition = Utility.BoardPositionToWorldPosition(new IntVector2(currentX - 2, worldPosition.y) - worldPosition);
				AI.transform.Find("DetectRightMark").localPosition = Utility.BoardPositionToWorldPosition(new IntVector2(currentX, worldPosition.y) - worldPosition);
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
			//Debug.Log(_boardstr);
		}
	}
}
