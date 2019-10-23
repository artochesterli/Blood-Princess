using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace PCG
{
	/// <summary>
	/// A Class for exit in room
	/// Exit Type:
	/// 8: Left End of Room (Dowm)
	/// 9: Right End of Room (Down)
	/// </summary>
	public class Exit
	{
		public string ExitType;
		public IntVector2 Position;

		public Exit(string exitType, IntVector2 position)
		{
			ExitType = exitType;
			Position = position;
		}
	}

	public class Room
	{
		public IntVector2 RoomDimension;
		public List<Exit> Exits;

		private StreamReader _streamReader;
		private IntVector2 _boardPosition;
		private IntVector2 _boardDimension;
		private IntVector2 m_BoardStartPosition;
		private string m_EntryType;
		private string[,] m_CurrentBoard;
		private int _seed;
		/// <summary>
		/// 0 means the room is secondary
		/// 1 means the room is left/right
		/// 2 means the room is left/right/down
		/// 3 means the room is left/right/up
		/// 4 means the room is left/right/up/down
		/// 5 means the room is up/down
		/// </summary>
		private int _roomType;
		private System.Random _random;
		private GameObject _room;
		private GameObject _boardGameObject;
		private Vector2 _roomCenterPosition;

		public Room(IntVector2 boardPosition, IntVector2 boardDimension, int seed, int roomType, GameObject boardGameObject)
		{
			// Debug.Assert(_streamReader != null, "Text File Assets Not Found when constructing a room");
			_boardPosition = boardPosition;
			_seed = seed;
			_roomType = roomType;
			_random = new System.Random(Guid.NewGuid().GetHashCode());
			RoomDimension = new IntVector2();
			Exits = new List<Exit>();
			_boardDimension = boardDimension;
			_boardGameObject = boardGameObject;
			_setupRoom();
		}

		public Room(IntVector2 BoardStartPosition, string entryType, int seed, int roomType, string[,] currentBoard)
		{
			m_BoardStartPosition = BoardStartPosition;
			m_EntryType = entryType;
			_roomType = roomType;
			m_CurrentBoard = currentBoard;
			_random = new System.Random(Guid.NewGuid().GetHashCode());
			Exits = new List<Exit>();
			_setupRoom2();
		}

		private void _setupRoom2()
		{

		}

		private void _setupRoom()
		{
			// Load Stream Reader According to RoomType
			string path = "Assets/PCG/RoomType" + _roomType.ToString();
			var info = new DirectoryInfo(path);
			int fileRandom = _random.Next(0, info.GetFiles().Length / 2);
			path += ("/" + fileRandom.ToString() + ".csv");
			_streamReader = new StreamReader(path);
			Debug.Assert(_streamReader != null, "Could not read file from path:" + path);

			// Read File into manipulable string array
			string entireFile = _streamReader.ReadToEnd();
			string[] choppedUpFile = entireFile.Split('\n');
			string[][] entireFileRead = new string[choppedUpFile.Length][];
			for (int i = 0; i < choppedUpFile.Length; i++)
			{
				string[] oneline = choppedUpFile[i].Split(',');
				entireFileRead[i] = oneline;
			}

			// Generate Block Tiles 
			int roomHeight = entireFileRead.Length;
			int roomWidth = entireFileRead[0].Length - 1;
			Sprite sampleTileSprite = (Resources.Load("BlockTile0", typeof(GameObject)) as GameObject).GetComponent<SpriteRenderer>().sprite;
			Debug.Assert(sampleTileSprite != null, "Sample Tile Sprite Not Found");
			float halfX = sampleTileSprite.bounds.extents.x;
			float halfY = sampleTileSprite.bounds.extents.y;
			float x = 2f * halfX;
			float y = 2f * halfY;
			// Set Room's Dimensions
			RoomDimension.x = roomWidth;
			RoomDimension.y = roomHeight;
			// Calculate Room Center Position
			// Assuming Board Position 0, 0 is World Position 0, 0
			_roomCenterPosition = Vector2.zero + new Vector2(x * roomWidth * _boardPosition.x, y * roomHeight * _boardPosition.y);
			_room = new GameObject("Room(" + _boardPosition.x + "," + _boardPosition.y + ")" + _roomType.ToString() + " - " + fileRandom.ToString());
			_room.transform.parent = _boardGameObject.transform;
			_room.transform.position = _roomCenterPosition;

			// Place Tiles based on file reader
			// 0 is 100% Empty
			// 1 is 100% Solid Tile
			// 2 is 50% Solid, 50% Empty

			for (int i = 0; i < entireFileRead.Length; i++)
			{
				for (int j = 0; j < entireFileRead[i].Length; j++)
				{
					string curChar = entireFileRead[i][j];
					Vector2 curTilePosition = _roomCenterPosition - new Vector2(((roomWidth - 1) / 2 - j) * x, 0);
					curTilePosition += new Vector2(0, ((roomHeight - 1) / 2 - i) * y);

					GameObject instantiatedObject = null;
					if (curChar == "1")
					{
						instantiatedObject = GameObject.Instantiate(Resources.Load("BlockTile" + _random.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;

					}
					else if (curChar == "2")
					{
						if (_random.Next(0, 100) > 50)
						{
							instantiatedObject = GameObject.Instantiate(Resources.Load("BlockTile" + _random.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;
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
						instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Knight", typeof(GameObject))) as GameObject;
					}
					else if (curChar == "b")
					{
						if (_random.Next(0, 100) > 50)
						{
							instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Knight", typeof(GameObject))) as GameObject;
						}
					}
					// Store the exit of the room, but do not generate anything there
					else if (curChar == "8" || curChar == "9")
					{
						Exits.Add(new Exit(curChar, new IntVector2()));
					}
					if (instantiatedObject != null)
					{
						instantiatedObject.transform.parent = _room.transform;
						instantiatedObject.transform.position = curTilePosition;
					}

				}
			}

			//// Place Extra Tiles if the Room is in the corner or edge
			//GameObject edgeHolder = new GameObject("EdgeHolder");
			//edgeHolder.transform.parent = _boardGameObject.transform;
			//edgeHolder.transform.position = Vector2.zero;
			//if (_boardPosition.x == 0)
			//{
			//	float LeftPosition = _roomCenterPosition.x - ((roomWidth - 1) / 2 + 1) * x;
			//	float topPosition = _roomCenterPosition.y + ((roomHeight - 1) / 2 + 1) * y;
			//	for (int i = 0; i < roomHeight + 1; i++)
			//	{
			//		float yPos = topPosition - i * (y);
			//		GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
			//		tile.transform.parent = edgeHolder.transform;
			//		tile.transform.position = new Vector2(LeftPosition, yPos);
			//	}
			//}
			//if (_boardPosition.x == _boardDimension.x - 1)
			//{
			//	float rightPosition = _roomCenterPosition.x + ((roomWidth - 1) / 2 + 2) * x;
			//	float topPosition = _roomCenterPosition.y + ((roomHeight - 1) / 2 + 1) * y;
			//	for (int i = 0; i < roomHeight + 1; i++)
			//	{
			//		float yPos = topPosition - i * (y);
			//		GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
			//		tile.transform.parent = edgeHolder.transform;
			//		tile.transform.position = new Vector2(rightPosition, yPos);
			//	}
			//}
			//if (_boardPosition.y == 0)
			//{
			//	float downPosition = _roomCenterPosition.y - ((roomHeight - 1) / 2 + 2) * y;
			//	float LeftPosition = _roomCenterPosition.x - ((roomWidth - 1) / 2 + 1) * x;
			//	for (int i = 0; i < roomWidth + 1; i++)
			//	{
			//		float xPos = LeftPosition + i * (x);
			//		GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
			//		tile.transform.parent = edgeHolder.transform;
			//		tile.transform.position = new Vector2(xPos, downPosition);
			//	}
			//}
			//if (_boardPosition.y == _boardDimension.y - 1)
			//{
			//	float topPosition = _roomCenterPosition.y + ((roomHeight - 1) / 2 + 1) * y;
			//	float LeftPosition = _roomCenterPosition.x - ((roomWidth - 1) / 2 + 1) * x;
			//	for (int i = 0; i < roomWidth + 1; i++)
			//	{
			//		float xPos = LeftPosition + i * (x);
			//		GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
			//		tile.transform.parent = edgeHolder.transform;
			//		tile.transform.position = new Vector2(xPos, topPosition);
			//	}
			//}
		}

		public void GeneratePlayer()
		{
			GameObject Player = GameObject.Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;
			Player.transform.position = _roomCenterPosition;
		}
	}

}
