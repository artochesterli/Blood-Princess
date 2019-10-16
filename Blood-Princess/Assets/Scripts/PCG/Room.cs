using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace PCG
{
    public class Room
    {
        private StreamReader _streamReader;
        private IntVector2 _boardPosition;
        private IntVector2 _boardDimension;
        private int _seed;
        /// <summary>
        /// 0 means the room is secondary
        /// 1 means the room is left/right
        /// 2 means the room is left/right/down
        /// 3 means the room is left/right/up
        /// 4 means the room is 2 w/ up eliminated
        /// </summary>
        private int _roomType;
        private System.Random _random;
        public IntVector2 RoomDimension;

        public Room(IntVector2 boardPosition, IntVector2 boardDimension, int seed, int roomType)
        {
            // Debug.Assert(_streamReader != null, "Text File Assets Not Found when constructing a room");
            _boardPosition = boardPosition;
            _seed = seed;
            _roomType = roomType;
            _random = new System.Random(_seed);
            RoomDimension = new IntVector2();
            _boardDimension = boardDimension;
            _setupRoom();
        }

        private void _setupRoom()
        {
            // Load Stream Reader According to RoomType
            string path = "Assets/PCG/RoomType" + _roomType.ToString();
            var info = new DirectoryInfo(path);
            int fileRandom = _random.Next(0, info.GetFiles().Length / 2);
            path += ("/" + fileRandom.ToString());
            _streamReader = new StreamReader(path);

            string entireFile = _streamReader.ReadToEnd();
            string[] choppedUpFile = entireFile.Split('\n');
            int roomHeight = choppedUpFile.Length;
            int roomWidth = choppedUpFile[0].Length - 1;
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
            Vector2 roomCenterPosition = Vector2.zero + new Vector2(x * roomWidth * _boardPosition.x, y * roomHeight * _boardPosition.y);

            // Place Tiles based on file reader
            for (int i = 0; i < choppedUpFile.Length; i++)
            {
                for (int j = 0; j < choppedUpFile[i].Length; j++)
                {
                    char curChar = choppedUpFile[i][j];
                    Vector2 curTilePosition = roomCenterPosition - new Vector2(((roomWidth - 1) / 2 - j) * x, 0);
                    curTilePosition += new Vector2(0, ((roomHeight - 1) / 2 - i) * y);

                    if (curChar == '1')
                    {
                        GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile" + _random.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;
                        tile.transform.position = curTilePosition;
                    }
                }
            }

            // Place Extra Tiles if the Room is in the corner or edge
            if (_boardPosition.x == 0)
            {
                float LeftPosition = roomCenterPosition.x - ((roomWidth - 1) / 2 + 1) * x;
                float topPosition = roomCenterPosition.y + ((roomHeight - 1) / 2 + 1) * y;
                for (int i = 0; i < roomHeight + 1; i++)
                {
                    float yPos = topPosition - i * (y);
                    GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
                    tile.transform.position = new Vector2(LeftPosition, yPos);
                }
            }
            if (_boardPosition.x == _boardDimension.x - 1)
            {
                float rightPosition = roomCenterPosition.x + ((roomWidth - 1) / 2 + 2) * x;
                float topPosition = roomCenterPosition.y + ((roomHeight - 1) / 2 + 1) * y;
                for (int i = 0; i < roomHeight + 1; i++)
                {
                    float yPos = topPosition - i * (y);
                    GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
                    tile.transform.position = new Vector2(rightPosition, yPos);
                }
            }
            if (_boardPosition.y == 0)
            {
                float downPosition = roomCenterPosition.y - ((roomHeight - 1) / 2 + 2) * y;
                float LeftPosition = roomCenterPosition.x - ((roomWidth - 1) / 2 + 1) * x;
                for (int i = 0; i < roomWidth + 1; i++)
                {
                    float xPos = LeftPosition + i * (x);
                    GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
                    tile.transform.position = new Vector2(xPos, downPosition);
                }
            }
            if (_boardPosition.y == _boardDimension.y - 1)
            {
                float topPosition = roomCenterPosition.y + ((roomHeight - 1) / 2 + 1) * y;
                float LeftPosition = roomCenterPosition.x - ((roomWidth - 1) / 2 + 1) * x;
                for (int i = 0; i < roomWidth + 1; i++)
                {
                    float xPos = LeftPosition + i * (x);
                    GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
                    tile.transform.position = new Vector2(xPos, topPosition);
                }
            }
        }
    }

}
