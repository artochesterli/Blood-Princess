using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace Clinic
{
    public static class Utility
    {
        public static Vector2 WallTileSize()
        {
            Sprite sampleTileSprite = (Resources.Load("WallGrid", typeof(GameObject)) as GameObject).GetComponent<SpriteRenderer>().sprite;
            Debug.Assert(sampleTileSprite != null, "Sample Tile Sprite Not Found");
            float halfX = sampleTileSprite.bounds.extents.x;
            float halfY = sampleTileSprite.bounds.extents.y;
            float x = 2f * halfX;
            float y = 2f * halfY;
            return new Vector2(x, y);
        }

        public static Vector2 GroundTileSize()
        {
            Sprite sampleTileSprite = (Resources.Load("GroundGrid", typeof(GameObject)) as GameObject).GetComponent<SpriteRenderer>().sprite;
            Debug.Assert(sampleTileSprite != null, "Sample Tile Sprite Not Found");
            float halfX = sampleTileSprite.bounds.extents.x;
            float halfY = sampleTileSprite.bounds.extents.y;
            float x = 2f * halfX;
            float y = 2f * halfY;
            return new Vector2(x, y);
        }

        public static Item NewItemFromString(string Name)
        {
            switch (Name.ToLower())
            {
                case "wood":
                    return new Wood(Resources.Load<ItemData>("ItemData"));
                case "cloth":
                    return new Cloth(Resources.Load<ItemData>("ItemData"));
                case "charcoal":
                    return new Charcoal(Resources.Load<ItemData>("ItemData"));
                case "rug":
                    return new Rug(Resources.Load<ItemData>("ItemData"));
                case "tub":
                    return new Tub(Resources.Load<ItemData>("ItemData"));
                case "scroll":
                    return new Scroll(Resources.Load<ItemData>("ItemData"));
            }
            return null;
        }
    }

    [System.Serializable]
    public class ItemDatium
    {
        public string Name;
        public Sprite Sprite;
        public string Description;
    }

    [System.Serializable]
    public class CraftableItemDatium : ItemDatium
    {
        public List<CraftMaterial> CraftMaterials;
    }

    [System.Serializable]
    public class SeedItemDatium : ItemDatium
    {

    }

    [System.Serializable]
    public class CraftMaterial
    {
        public string Name;
        public Sprite Sprite;
        public int Amount = 1;
    }

    [System.Serializable]
    public class Vector2IntAndDecorationItemList
    {
        public List<Vector2Int> BuildPositions;
        public List<DecorationItem> DecorationItems;

        public Vector2IntAndDecorationItemList(List<Vector2Int> buildPositions, List<DecorationItem> decorationItems)
        {
            BuildPositions = buildPositions;
            DecorationItems = decorationItems;
        }
    }

    public abstract class Grid
    {
        public Vector2Int BoardPosition;
        public Vector2 WorldPosition;
        public GameObject gameObject;
        public string GridName => GetType().Name;
        public GridState GridState
        {
            get { return m_GridState; }
            set
            {
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                switch (value)
                {
                    case GridState.Empty:
                        sr.color = Color.white;
                        break;
                    case GridState.Occupied:
                        sr.color = Color.black;
                        break;
                    case GridState.CannotPlace:
                        sr.color = Color.red;
                        break;
                    case GridState.CanPlace:
                        sr.color = Color.green;
                        break;
                }
                m_GridState = value;
            }
        }

        private GridState m_GridState;

        public Grid(Vector2Int boardPosition, GameObject go)
        {
            BoardPosition = boardPosition;
            gameObject = go;
            GridState = GridState.Empty;
        }

        public Grid(Vector2Int boardPosition, GameObject go, GridState gridState)
        {
            BoardPosition = boardPosition;
            gameObject = go;
            GridState = gridState;
        }
    }

    public enum GridState
    {
        Empty,
        Occupied,
        CannotPlace,
        CanPlace,
    }

    public class WallGrid : Grid
    {
        public WallGrid(Vector2Int boardPosition, GameObject go) : base(boardPosition, go)
        {
        }

        public WallGrid(Vector2Int boardPosition, GameObject go, GridState gridState) : base(boardPosition, go, gridState)
        {
        }
    }

    public class GroundGrid : Grid
    {
        public GroundGrid(Vector2Int boardPosition, GameObject go) : base(boardPosition, go)
        {
        }

        public GroundGrid(Vector2Int boardPosition, GameObject go, GridState gridState) : base(boardPosition, go, gridState)
        {
        }
    }
}


