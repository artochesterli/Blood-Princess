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
        /// <summary>
        /// Relative Position to Room File
        /// </summary>
        public IntVector2 RelativePosition;
        public IntVector2 BoardPosition;

        public Exit(string exitType, IntVector2 relativePosition)
        {
            ExitType = exitType;
            RelativePosition = relativePosition;
        }
    }

    public class Room
    {
        public IntVector2 RoomDimension;
        public List<Exit> Exits;
        public Exit RoomExit;

        private StreamReader _streamReader;
        /// <summary>
        /// The Start Position of the room on board
        /// </summary>
        private IntVector2 m_BoardStartPosition;
        /// <summary>
        /// Board should make sure to check this type
        /// of room has this entry type
        /// </summary>
        private string m_EntryType;
        /// <summary>
        /// Get a copy of the current board when creating this room
        /// a 2-d string array, each string with tile type on it
        /// </summary>
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
        private string[][] entireRoomFile;
        private Exit m_ConnectingExit;
        /// <summary>
        /// m_BoardRoomOffset = BoardStartPosition - m_ConnectingExit.RelativePosition
        /// </summary>
        private IntVector2 m_BoardRoomOffset;

        private GameObject _room;
        private GameObject _boardGameObject;

        public Room(IntVector2 BoardStartPosition, string entryType, int seed, int roomType, ref string[,] currentBoard, GameObject boardGameObject)
        {
            m_BoardStartPosition = BoardStartPosition;
            m_EntryType = entryType;
            _roomType = roomType;
            m_CurrentBoard = currentBoard;
            _random = new System.Random(Guid.NewGuid().GetHashCode());
            Exits = new List<Exit>();
            m_ConnectingExit = null;
            RoomExit = null;
            _boardGameObject = boardGameObject;
            _setupRoom();
        }

        private void _setupRoom()
        {
            // 1. Find a random room according to requiredRoomtype
            _findRandomRoom();
            Debug.Assert(entireRoomFile != null, "Something Wrong Finding Random Room");
            // 2. Find the random room's exit position
            _findExitPosition();
            // Find the Room's Exit

            Debug.Assert(Exits.Count != 0, "Something Wrong Loading Room Exits");
            // 3. Find the exit that is same as the entryType
            _findConnectingExit();
            Debug.Assert(m_ConnectingExit != null, "Something wrong finding the exit that has the same type");
            // 4. Check if current placing will not obstruct current board
            if (_canPlaceRoom())
            {
                // 5. Place
                _placeRoom();
                _findRoomExit();
                Debug.Assert(RoomExit != null, "No RoomExit Was Found");
            }
            else
            {
                Debug.LogError("Cannot Place room due to overlapping tiles");
            }
        }

        private void _findRoomExit()
        {
            foreach (Exit e in Exits)
            {
                if (e.ExitType != m_EntryType)
                {
                    RoomExit = e;
                    RoomExit.BoardPosition = RoomExit.RelativePosition + m_BoardRoomOffset;
                    return;
                }
            }
        }

        private void _placeRoom()
        {
            for (int i = 0; i < entireRoomFile.Length; i++)
            {
                for (int j = 0; j < entireRoomFile[0].Length; j++)
                {
                    IntVector2 curTileRelativePosition = new IntVector2(i, j);
                    IntVector2 curTileWorldPosition = curTileRelativePosition + m_BoardRoomOffset;
                    string curChar = entireRoomFile[i][j];
                    // Check if this is a expandable tile
                    // if so replace it now
                    if (Utility.ExpandableStrHashSet.Contains(curChar))
                    {
                        _placeExpandableRoom(curChar, curTileWorldPosition);
                        continue;
                    }
                    // Always Assumes Board zero position is world zero position
                    // add to board
                    if (!Utility.IgnorePlacingStrHashSet.Contains(curChar))
                    {
                        m_CurrentBoard[curTileWorldPosition.x, curTileWorldPosition.y] = entireRoomFile[i][j];
                        if (Utility.ContainsEnemy(curChar))
                        {
                            Services.EnemyGenerationManager.RecordNextEnemyInfo(Utility.GetEnemyType(curChar),
                            curTileWorldPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find a room using roomtype in PCG/Expandable/roomType
        /// place a room using roomType, find it in PCG/Expandable
        /// </summary>
        /// <param name="roomType"></param>
        private void _placeExpandableRoom(string roomType, IntVector2 startWorldPosition)
        {
            // string[][] expandableroom = _getRandomRoomFromPath("/PCG/Expandable/" + roomType);
            string[][] expandableroom = Services.MapGenerationManager.GetNextRoomFromRoomType(roomType);
            for (int i = 0; i < expandableroom.Length; i++)
            {
                for (int j = 0; j < expandableroom[i].Length; j++)
                {
                    IntVector2 curTileWorldPosition = startWorldPosition + new IntVector2(i, j);
                    string curChar = expandableroom[i][j];
                    if (Utility.ExpandableStrHashSet.Contains(curChar))
                    {
                        _placeExpandableRoom(curChar, curTileWorldPosition);
                        continue;
                    }
                    if (!Utility.IgnorePlacingStrHashSet.Contains(curChar))
                    {
                        m_CurrentBoard[curTileWorldPosition.x, curTileWorldPosition.y] = expandableroom[i][j];
                        if (Utility.ContainsEnemy(curChar))
                        {
                            Services.EnemyGenerationManager.RecordNextEnemyInfo(Utility.GetEnemyType(curChar),
                            curTileWorldPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get a random room from _path and return a read file as 2d string array
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        private string[][] _getRandomRoomFromPath(string _path)
        {
            // Load Stream Reader According to RoomType
            string path = Application.dataPath + _path;
            var info = new DirectoryInfo(path);
            int fileRandom = _random.Next(0, info.GetFiles().Length / 2);
            path += ("/" + fileRandom.ToString() + ".csv");
            _streamReader = new StreamReader(path);

            // Read File into manipulable string array
            string entireFile = _streamReader.ReadToEnd();
            string[] choppedUpFile = entireFile.Split('\n');
            string[][] result = new string[choppedUpFile.Length][];
            for (int i = 0; i < choppedUpFile.Length; i++)
            {
                string[] oneline = choppedUpFile[i].Split(',');
                result[i] = oneline;
            }

            // Rotatte entireRoomFile ClockWise 90
            int x = result.Length;
            int y = result[0].Length;
            string[][] temp = new string[y][];
            for (int i = 0; i < y; i++)
            {
                temp[i] = new string[x];
            }

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    temp[i][j] = result[x - j - 1][i];
                    temp[i][j] = temp[i][j].Trim();
                }
            }
            return temp;
        }

        private bool _canPlaceRoom()
        {
            for (int i = 0; i < entireRoomFile.Length; i++)
            {
                for (int j = 0; j < entireRoomFile[i].Length; j++)
                {
                    IntVector2 curTileRelativePosition = new IntVector2(i, j);
                    IntVector2 curTileWorldPosition = curTileRelativePosition + m_BoardRoomOffset;
                    // If the current tile is not empty and 
                    // there is anything there on board

                    if (!Utility.EmptyStrHashSet.Contains(entireRoomFile[i][j]) &&
                        !Utility.EmptyStrHashSet.Contains(m_CurrentBoard[curTileWorldPosition.x, curTileWorldPosition.y])) return false;
                }
            }
            return true;
        }

        private void _findConnectingExit()
        {
            foreach (Exit e in Exits)
            {
                if (e.ExitType == m_EntryType)
                {
                    m_ConnectingExit = e;
                    m_BoardRoomOffset = m_BoardStartPosition - m_ConnectingExit.RelativePosition;
                    return;
                }
            }
        }

        /// <summary>
        /// Find a random room and load the EntireRoomFile
        /// </summary>
        private void _findRandomRoom()
        {
            // entireRoomFile = _getRandomRoomFromPath("/PCG/RoomType" + _roomType.ToString());
            entireRoomFile = Services.MapGenerationManager.GetNextRoomFromRoomType(_roomType.ToString());

            _room = new GameObject("Room" + _roomType.ToString());
            _room.transform.parent = _boardGameObject.transform;

        }

        /// <summary>
        /// Find all exits Position of this room
        /// and load them into Exits
        /// </summary>
        private void _findExitPosition()
        {
            for (int i = 0; i < entireRoomFile.Length; i++)
            {
                for (int j = 0; j < entireRoomFile[i].Length; j++)
                {
                    string curChar = entireRoomFile[i][j];
                    if (Utility.ExitStrHashSet.Contains(curChar))
                    {
                        Exits.Add(new Exit(curChar, new IntVector2(i, j)));
                    }
                }
            }
        }


        /// <summary>
        /// Print the Room
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < entireRoomFile.Length; i++)
            {
                for (int j = 0; j < entireRoomFile[i].Length; j++)
                {
                    str += entireRoomFile[i][j];
                }
                str += "\n";
            }
            return str;
        }
    }

}
