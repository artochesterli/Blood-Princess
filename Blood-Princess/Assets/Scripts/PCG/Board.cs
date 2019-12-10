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

        /// <summary>
        /// return the TG0 Character
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <returns></returns>
        private string _TG0Placement(IntVector2 boardPosition)
        {
            string[] leftChar = _board[boardPosition.x - 1, boardPosition.y].Split(';');
            string[] rightChar = _board[boardPosition.x + 1, boardPosition.y].Split(';');
            string[] upChar = _board[boardPosition.x, boardPosition.y + 1].Split(';');
            string[] downChar = _board[boardPosition.x, boardPosition.y - 1].Split(';');

            // Check Upper Part
            if (upChar.Length < 2 || upChar[0] != "0")
            {
                // Check upper left corner
                if (leftChar.Length < 2 || leftChar[0] != "0")
                {
                    return "1";
                }

                // Check upper right corner
                if (rightChar.Length < 2 || rightChar[0] != "0")
                {
                    return "4";
                }

                // 2 or 3 
                return Utility.RandomFromArray<string>(new string[] { "2", "3" }, m_Rand);
            }

            // Check Lower Part
            if (downChar.Length < 2 || downChar[0] != "0")
            {
                // Check Down Left Corner
                if (leftChar.Length < 2 || leftChar[0] != "0")
                {
                    return "17";
                }

                // check down right corner
                if (rightChar.Length < 2 || rightChar[0] != "0")
                {
                    return "20";
                }

                //18 or 19
                return Utility.RandomFromArray<string>(new string[] { "18", "19" }, m_Rand);
            }

            // Check for left
            if (leftChar.Length < 2 || leftChar[0] != "0")
            {
                return Utility.RandomFromArray<string>(new string[] { "9", "13" }, m_Rand);
            }

            // Check for right
            if (rightChar.Length < 2 || rightChar[0] != "0")
            {
                return Utility.RandomFromArray<string>(new string[] { "12", "16" }, m_Rand);
            }

            // Only Center can be left now
            return Utility.RandomFromArray<string>(new string[] { "10", "11", "14", "15" }, m_Rand);

        }

        /// <summary>
        /// Return the TB0 Character
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <returns></returns>
        private string _TB0Placement(IntVector2 boardPosition)
        {
            string[] leftChar = _board[boardPosition.x - 1, boardPosition.y].Split(';');
            string[] rightChar = _board[boardPosition.x + 1, boardPosition.y].Split(';');
            string[] upChar = _board[boardPosition.x, boardPosition.y + 1].Split(';');
            string[] downChar = _board[boardPosition.x, boardPosition.y - 1].Split(';');

            // Check Upper Part
            if (upChar.Length < 2 || upChar[0] != "0")
            {
                // Check upper left corner
                if (leftChar.Length < 2 || leftChar[0] != "0")
                {
                    return "1";
                }

                // Check upper right corner
                if (rightChar.Length < 2 || rightChar[0] != "0")
                {
                    return "3";
                }

                // 2
                return "2";
            }

            // Check Lower Part
            if (downChar.Length < 2 || downChar[0] != "0")
            {
                // Check Down Left Corner
                if (leftChar.Length < 2 || leftChar[0] != "0")
                {
                    return "13";
                }

                // check down right corner
                if (rightChar.Length < 2 || rightChar[0] != "0")
                {
                    return "15";
                }

                //18 or 19
                return "14";
            }

            // Check for left
            if (leftChar.Length < 2 || leftChar[0] != "0")
            {
                return Utility.RandomFromArray<string>(new string[] { "7", "10" }, m_Rand);
            }

            // Check for right
            if (rightChar.Length < 2 || rightChar[0] != "0")
            {
                return Utility.RandomFromArray<string>(new string[] { "12", "9" }, m_Rand);
            }

            // Only Center can be left now
            return Utility.RandomFromArray<string>(new string[] { "8", "11" }, m_Rand);
        }

        private string _FR0Placement(IntVector2 boardPosition)
        {
            string[] leftChar = _board[boardPosition.x - 1, boardPosition.y].Split(';');
            string[] rightChar = _board[boardPosition.x + 1, boardPosition.y].Split(';');
            string[] upChar = _board[boardPosition.x, boardPosition.y + 1].Split(';');
            string[] downChar = _board[boardPosition.x, boardPosition.y - 1].Split(';');

            // Check Lower Part
            if (downChar.Length < 2 || downChar[0] != "0")
            {
                // Check Down Left Corner
                if (leftChar.Length < 2 || leftChar[0] != "0")
                {
                    return "7";
                }

                // check down right corner
                if (rightChar.Length < 2 || rightChar[0] != "0")
                {
                    return "9";
                }

                //18 or 19
                return "8";
            }

            // Check for left
            if (leftChar.Length < 2 || leftChar[0] != "0")
            {
                return "4";
            }

            // Check for right
            if (rightChar.Length < 2 || rightChar[0] != "0")
            {
                return "6";
            }

            // Only Center can be left now
            return "5";
        }

        private string _T1Placement(IntVector2 boardPosition)
        {
            string[] leftChar = _board[boardPosition.x - 1, boardPosition.y].Split(';');
            string[] rightChar = _board[boardPosition.x + 1, boardPosition.y].Split(';');
            if (leftChar[0] == "1" && leftChar[1] == "F")
            {
                return "2";
            }
            if (rightChar[0] == "1" && rightChar[1] == "F")
            {
                return "4";
            }
            return "3";
        }

        private string _T2Placement(IntVector2 boardPosition)
        {
            string[] leftChar = _board[boardPosition.x - 1, boardPosition.y].Split(';');
            string[] rightChar = _board[boardPosition.x + 1, boardPosition.y].Split(';');
            if (leftChar[0] != "2")
            {
                return "3";
            }
            if (rightChar[0] != "2")
            {
                return "2";
            }
            return "1";
        }

        private string _FR2Placement(IntVector2 boardPosition)
        {
            string[] leftChar = _board[boardPosition.x - 1, boardPosition.y].Split(';');
            string[] rightChar = _board[boardPosition.x + 1, boardPosition.y].Split(';');
            if (leftChar[0] != "2")
            {
                return "1";
            }
            if (rightChar[0] != "2")
            {
                return "3";
            }
            return "2";
        }

        private string _BW4Placement(IntVector2 boardPosition)
        {
            string[] upChar = _board[boardPosition.x, boardPosition.y + 1].Split(';');
            string[] downChar = _board[boardPosition.x, boardPosition.y - 1].Split(';');
            if (upChar[0] != "4")
            {
                return "1";
            }
            if (downChar[0] != "4")
            {
                return "5";
            }

            return Utility.RandomFromArray<string>(new string[] { "2", "3", "4" }, m_Rand);
        }

        private string _L5Placement(IntVector2 boardPosition)
        {
            string[] upChar = _board[boardPosition.x, boardPosition.y + 1].Split(';');
            string[] downChar = _board[boardPosition.x, boardPosition.y - 1].Split(';');
            if (downChar[0] != "5")
            {
                return "3";
            }
            return "2";
        }

        private bool _D6Placement(IntVector2 boardPosition)
        {
            string[] upChar = _board[boardPosition.x, boardPosition.y + 1].Split(';');
            if (upChar[0] != "6")
            {
                return false;
            }

            return true;
        }

        private void _placeTile(string curChar, IntVector2 worldPosition)
        {
            Vector2 curTileWorldPosition = Vector2.zero +
                new Vector2(worldPosition.x * Utility.TileSize().x, worldPosition.y * Utility.TileSize().y);

            GameObject instantiatedObject = null;
            string curCharacter = curChar;
            if (Utility.IgnorePlacingStrHashSet.Contains(curCharacter)) return;
            string curCharType = "";
            string[] splitChar = curChar.Split(';');
            curCharacter = splitChar[0];
            if (splitChar.Length > 1)
            {
                curCharType = splitChar[1];
            }
            string loadPath = Utility.LoadPath(curCharacter, curCharType);

            switch (curCharacter)
            {
                case "0":
                    if (curCharType == "TG")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _TG0Placement(worldPosition)));
                    }
                    else if (curCharType == "TB")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _TB0Placement(worldPosition)));
                    }
                    else if (curCharType == "FRB" || curCharType == "FRF")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _FR0Placement(worldPosition)));
                    }
                    break;
                case "1":
                    if (curCharType == "F")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + "1"));
                    }
                    else if (curCharType == "T")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _T2Placement(worldPosition)));
                    }
                    break;
                case "2":
                    if (curCharType == "T")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _T2Placement(worldPosition)));
                    }
                    else if (curCharType == "FRB" || curCharType == "FRF")
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _FR2Placement(worldPosition)));
                    }
                    break;
                case "3":
                    instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + "1"));
                    break;
                case "4":
                    instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _BW4Placement(worldPosition)));
                    break;
                case "5":
                    instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + _L5Placement(worldPosition)));
                    break;
                case "6":
                    if (_D6Placement(worldPosition))
                    {
                        instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(loadPath + "1"));
                    }
                    break;
            }

            // if (curCharacter == "1" || curCharacter == "3")
            // {
            //     if (!Utility.EmptyStrHashSet.Contains(_board[worldPosition.x, worldPosition.y - 1].Split(';')[0]))
            //     {
            //         instantiatedObject = GameObject.Instantiate(Resources.Load(Utility.LoadPath(curRoomType) + "WallTile0", typeof(GameObject))) as GameObject;
            //     }
            //     else
            //     {
            //         instantiatedObject = GameObject.Instantiate(Resources.Load(Utility.LoadPath(curRoomType) + "BlockTile" + m_Rand.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;
            //     }

            // }
            // else if (curCharacter == "2")
            // {
            //     if (m_Rand.Next(0, 100) > 50)
            //     {
            //         instantiatedObject = GameObject.Instantiate(Resources.Load(Utility.LoadPath(curRoomType) + "BlockTile" + m_Rand.Next(0, 2).ToString(), typeof(GameObject))) as GameObject;
            //     }
            // }
            // else if (curCharacter == "5")
            // {
            //     instantiatedObject = GameObject.Instantiate(Resources.Load(Utility.LoadPath(curRoomType) + "Ladder", typeof(GameObject))) as GameObject;
            // }
            // else if (curCharacter == "6")
            // {
            //     instantiatedObject = GameObject.Instantiate(Resources.Load(Utility.LoadPath(curRoomType) + "PassablePlatform", typeof(GameObject))) as GameObject;
            // }
            // else if (curCharacter == "a")
            // {
            //     int randInt = m_Rand.Next(0, 100);
            //     if (randInt > 45)
            //         instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
            //     else if (randInt > 15)
            //         instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemy2", typeof(GameObject))) as GameObject;
            //     else
            //         instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Knight", typeof(GameObject))) as GameObject;


            //     // initialize knight's Patrol Point and Engage Point
            //     _initializeAI(instantiatedObject, worldPosition);
            // }
            // else if (curCharacter == "b")
            // {
            //     if (m_Rand.Next(0, 100) > 50)
            //     {
            //         if (m_Rand.Next(0, 100) > 30)
            //             instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
            //         else
            //             instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Knight", typeof(GameObject))) as GameObject;
            //         _initializeAI(instantiatedObject, worldPosition);
            //     }
            // }
            // else if (curCharacter == "p")
            // {
            //     instantiatedObject = GameObject.Instantiate(Resources.Load("Prefabs/Character", typeof(GameObject))) as GameObject;
            // }
            // else if (curCharacter == "dummy")
            // {
            //     instantiatedObject = GameObject.Instantiate(Resources.Load("Dummy", typeof(GameObject))) as GameObject;
            // }
            // else if (curCharacter == "7" && _board[worldPosition.x, worldPosition.y - 1].Split(';')[0] != "7")
            // {
            //     instantiatedObject = GameObject.Instantiate(Resources.Load(Utility.LoadPath(curRoomType) + "Door", typeof(GameObject))) as GameObject;
            // }

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
                leftDownPos = leftDownPos.Split(';')[0];
                string leftPos = _board[currentX - 1, worldPosition.y];
                leftPos = leftPos.Split(';')[0];
                //leftIsWall = (leftPos != "" && leftPos != "0" && leftPos != "6" && leftPos != "e" && leftPos != "a" && leftPos != "b" && leftPos != "7");
                leftIsWall = !Utility.EmptyStrHashSet.Contains(leftPos);
                //leftIsEdge = (leftPos == "" || leftPos == "0") && (leftDownPos == "" || leftDownPos == "0");
                leftIsEdge = Utility.EmptyStrHashSet.Contains(leftPos) && Utility.EmptyStrHashSet.Contains(leftDownPos);
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
                rightDownPos = rightDownPos.Split(';')[0];
                string rightPos = _board[currentX + 1, worldPosition.y];
                rightPos = rightPos.Split(';')[0];
                //rightIsWall = (rightPos != "" && rightPos != "0" && rightPos != "6" && rightPos != "e" && rightPos != "a" && rightPos != "b" && rightPos != "7");
                rightIsWall = !Utility.EmptyStrHashSet.Contains(rightPos);
                //RightIsEdge = (rightPos == "" || rightPos == "0") && (rightDownPos == "" || rightDownPos == "0");
                RightIsEdge = Utility.EmptyStrHashSet.Contains(rightPos) && Utility.EmptyStrHashSet.Contains(rightDownPos);
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
            //string _boardstr = "";
            //for (int i = _height - 1; i >= 0; i--)
            //{
            //	for (int j = 0; j < _width; j++)
            //	{
            //		_boardstr += _board[j, i];
            //	}
            //	_boardstr += "\n";
            //}
            //Debug.Log(_boardstr.Trim());
        }
    }
}
