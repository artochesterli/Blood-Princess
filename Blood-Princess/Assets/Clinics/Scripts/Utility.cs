using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
					return new Rug(Resources.Load<ItemData>("ItemData"));
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

	public abstract class Grid
	{
		public Vector2Int BoardPosition;
		public Vector2 WorldPosition;
		public GameObject gameObject;
		public string GridName => GetType().Name;

		public Grid(Vector2Int boardPosition)
		{
			BoardPosition = boardPosition;
		}
	}

	public class WallGrid : Grid
	{
		public WallGrid(Vector2Int boardPosition) : base(boardPosition)
		{
		}
	}

	public class GroundGrid : Grid
	{
		public GroundGrid(Vector2Int boardPosition) : base(boardPosition)
		{
		}
	}
}


