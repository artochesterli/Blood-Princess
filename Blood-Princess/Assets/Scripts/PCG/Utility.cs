using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace PCG
{
    /// <summary>
    /// A utility Class for PCG
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// A HashSet of strings that indicates exit in rooms
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public static readonly HashSet<string> ExitStrHashSet = new HashSet<string>()
        {
            "8","9"
        };

        /// <summary>
        /// A Hashset of strings that indicats expandable string in rooms
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public static readonly HashSet<string> ExpandableStrHashSet = new HashSet<string>()
        {
            "s.1", "s.2"
        };

        /// <summary>
        /// A Hashset of string that will be ignored when placing on board
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public static readonly HashSet<string> IgnorePlacingStrHashSet = new HashSet<string>()
        {
            "8", "9", "s.1", "s.2"
        };

        /// <summary>
        /// A Hashset of string that acts as empty str on board, such as 0 or *
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public static readonly HashSet<string> EmptyStrHashSet = new HashSet<string>()
        {
            "0", "", "\n", "*"
        };

        public static Vector2 TileSize()
        {
            Sprite sampleTileSprite = (Resources.Load("BlockTile0", typeof(GameObject)) as GameObject).GetComponent<SpriteRenderer>().sprite;
            Debug.Assert(sampleTileSprite != null, "Sample Tile Sprite Not Found");
            float halfX = sampleTileSprite.bounds.extents.x;
            float halfY = sampleTileSprite.bounds.extents.y;
            float x = 2f * halfX;
            float y = 2f * halfY;
            return new Vector2(x, y);
        }

        public static int MaxFileHeight()
        {
            string[] csvFiles = Directory.GetFiles("Assets/PCG", ".csv", SearchOption.AllDirectories);
            Debug.Log(csvFiles.Length);
            foreach (string str in csvFiles)
            {
                Debug.Log(str);
            }
            return 0;
        }

        public static int MaxFileWidth()
        {
            return 0;
        }

        public static Vector2 BoardPositionToWorldPosition(IntVector2 boardPosition)
        {
            return new Vector2(boardPosition.x * TileSize().x, boardPosition.y * TileSize().y);
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
