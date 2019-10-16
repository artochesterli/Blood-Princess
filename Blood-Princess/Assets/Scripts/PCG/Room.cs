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

        public Room(IntVector2 boardPosition, int seed, int roomType)
        {
            // Debug.Assert(_streamReader != null, "Text File Assets Not Found when constructing a room");
            _boardPosition = boardPosition;
            _seed = seed;
            _roomType = roomType;
            _random = new System.Random(_seed);

            _setupRoom();
        }

        private void _setupRoom()
        {
            // Load Stream Reader According to RoomType
            string path = "Assets/PCG/RoomType" + _roomType.ToString();
            var info = new DirectoryInfo(path);
            int fileRandom = _random.Next(0, info.GetFiles().Length / 2);
            Debug.Log(info.GetFiles().Length / 2);
            path += ("/" + fileRandom.ToString());
            _streamReader = new StreamReader(path);

            string entireFile = _streamReader.ReadToEnd();
            string[] choppedUpFile = entireFile.Split('\n');
            int fileLength = choppedUpFile.Length;
            Sprite sampleTileSprite = (Resources.Load("BlockTile", typeof(GameObject)) as GameObject).GetComponent<SpriteRenderer>().sprite;
            Debug.Assert(sampleTileSprite != null, "Sample Tile Sprite Not Found");
            // Debug.Log(sampleTileSprite.name);
            float halfX = sampleTileSprite.bounds.extents.x;
            // float halfX = 0f;
            float x = 2f * halfX;

            // Calculate Room Center Position
            // Assuming Board Position 0, 0 is World Position 0, 0
            Vector2 roomCenterPosition = Vector2.zero + new Vector2(x * fileLength * _boardPosition.x, x * fileLength * _boardPosition.y);

            // Place Tiles based on file reader
            for (int i = 0; i < choppedUpFile.Length; i++)
            {
                for (int j = 0; j < choppedUpFile[i].Length; j++)
                {
                    char curChar = choppedUpFile[i][j];
                    Vector2 curTilePosition = roomCenterPosition - new Vector2(((fileLength - 1) / 2 - j) * x, 0);
                    curTilePosition += new Vector2(0, ((fileLength - 1) / 2 - i) * x);

                    if (curChar == '1')
                    {
                        GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile", typeof(GameObject))) as GameObject;
                        tile.transform.position = curTilePosition;
                    }
                }
            }
        }
    }

}
