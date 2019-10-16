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
        /// int value means the room type
        /// </summary>
        private int[,] _board;
        private IntVector2 _currentCriticalPathPosition;
        private IntVector2 _lastCriticalPathPosition;
        private int _lastCriticalPathDirection;

        public Board(int width, int height, int seed)
        {
            _seed = seed;
            _width = width;
            _height = height;
            _board = new int[width, height];
            _currentCriticalPathPosition = new IntVector2();
            _setupBoard();
            _fillUpBoard();
            // _fillAroundBoard();
        }

        /// <summary>
        /// 1  Pick first room position
        /// 2  Name the First Room
        /// 3  Find Critical Path
        /// </summary>
        private void _setupBoard()
        {
            // Setup Random using a specific Seed value (default 0)
            System.Random rand = new System.Random(_seed);

            // Pick the first room Position
            _currentCriticalPathPosition.x = rand.Next(0, _width - 1);
            _currentCriticalPathPosition.y = _height - 1;

            // Set the last room position
            _lastCriticalPathPosition.x = _currentCriticalPathPosition.x;
            _lastCriticalPathPosition.y = _currentCriticalPathPosition.y;

            // Name the First Room, usually a room 1 type or room 2 type
            _board[_currentCriticalPathPosition.x, _currentCriticalPathPosition.y] = rand.Next(1, 3);

            do
            {
                // Generate a random number 1..5
                // 1/2 -- Left
                // 3/4 -- Right
                // 5 -- Down
                int pathDirection = rand.Next(1, 6);

                // If the last direction is left/right
                // Then this direction needs to continue left/right
                // Unless it is going down
                if (_lastCriticalPathDirection != 0 && _lastCriticalPathDirection <= 2 && pathDirection != 5)
                {
                    pathDirection = 1;
                }
                else if (_lastCriticalPathDirection != 0 && _lastCriticalPathDirection <= 4 && _lastCriticalPathDirection > 2
                && pathDirection != 5)
                {
                    pathDirection = 3;
                }
                Console.Write(pathDirection);
                // Go to next Position based on pathDirection
                if (pathDirection <= 2)
                {
                    _currentCriticalPathPosition.x--;
                    _lastCriticalPathDirection = 1;
                    // If This position hits a wall, then go down and turn Right
                    if (_currentCriticalPathPosition.x < 0)
                    {
                        _currentCriticalPathPosition.x = 0;
                        _currentCriticalPathPosition.y--;
                        _lastCriticalPathDirection = 3;
                        if (_lastCriticalPathPosition.y + 1 < _height &&
                           (_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 2 ||
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 4))
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 4;
                        else
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 2;
                    }
                }
                else if (pathDirection > 2 && pathDirection <= 4)
                {
                    _currentCriticalPathPosition.x++;
                    _lastCriticalPathDirection = 3;
                    // If this position hits a wall, then go down and turn left
                    if (_currentCriticalPathPosition.x >= _width)
                    {
                        _currentCriticalPathPosition.x = _width - 1;
                        _currentCriticalPathPosition.y--;
                        _lastCriticalPathDirection = 1;
                        // Check if last room was 2
                        if (_lastCriticalPathPosition.y + 1 < _height &&
                           (_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 2 ||
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 4))
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 4;
                        else
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 2;
                    }
                }
                else
                {
                    // Just Go Down
                    _currentCriticalPathPosition.y--;
                    _lastCriticalPathDirection = 5;
                    if (_lastCriticalPathPosition.y + 1 < _height &&
                           (_board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 2 ||
                            _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y + 1] == 4))
                        _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 4;
                    else
                        _board[_lastCriticalPathPosition.x, _lastCriticalPathPosition.y] = 2;
                }

                // On Default, Set the current moved room to room type 1
                _board[_currentCriticalPathPosition.x, _currentCriticalPathPosition.y] = 1;
                // If the last room was on top and was a 2/4
                // Then this one must be a 3 or 4
                if (_lastCriticalPathPosition.x == _currentCriticalPathPosition.x &&
                    _lastCriticalPathPosition.y == _currentCriticalPathPosition.y + 1)
                {
                    _board[_currentCriticalPathPosition.x, _currentCriticalPathPosition.y] = rand.Next(3, 5);

                }
                _lastCriticalPathPosition.x = _currentCriticalPathPosition.x;
                _lastCriticalPathPosition.y = _currentCriticalPathPosition.y;

            } while (_currentCriticalPathPosition.y > 0 && _currentCriticalPathPosition.x >= 0 &&
            _currentCriticalPathPosition.x < _width);
        }

        private void _fillUpBoard()
        {
            for (int i = _height - 1; i >= 0; i--)
            {
                for (int j = 0; j < _width; j++)
                {
                    Room rm = new Room(new IntVector2(j, i), new IntVector2(_width, _height), _seed, _board[j, i]);
                    // BoardDimensions.x += rm.RoomDimension.x;
                    // BoardDimensions.y += rm.RoomDimension.y;
                }
            }
            // BoardDimensions.x /= _width;
            // BoardDimensions.y /= _height;

            // _roomDimensions.x = BoardDimensions.x / _width;
            // _roomDimensions.y = BoardDimensions.y / _height;
        }

        /// <summary>
        /// Fill the Edge of the Board with BlockTiles
        /// </summary>
        private void _fillAroundBoard()
        {
            // Debug.Assert(BoardDimensions != Vector2.zero, "Board Dimension cannot be zero");
            // Sprite sampleTileSprite = (Resources.Load("BlockTile0", typeof(GameObject)) as GameObject).GetComponent<SpriteRenderer>().sprite;
            // Debug.Assert(sampleTileSprite != null, "Sample Tile Sprite Not Found");
            // float halfX = sampleTileSprite.bounds.extents.x;
            // float halfY = sampleTileSprite.bounds.extents.y;
            // float x = 2f * halfX;
            // float y = 2f * halfY;
            // float _leftX = 0f - _roomDimensions.x / 2f;
            // // LeftDown Corner

            // GameObject tile = GameObject.Instantiate(Resources.Load("BlockTile2", typeof(GameObject))) as GameObject;
            // tile.transform.position = new Vector2(_leftX, 0f);
        }

        public void Print()
        {
            Console.WriteLine();
            for (int i = _height - 1; i >= 0; i--)
            {
                for (int j = 0; j < _width; j++)
                {
                    Console.Write(_board[j, i]);
                }
                Console.WriteLine();
            }
        }
    }

    public struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
