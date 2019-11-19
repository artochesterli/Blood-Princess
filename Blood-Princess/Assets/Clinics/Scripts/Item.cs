using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	public abstract class Item
	{
		public string Name;
		public int Number = 1;
		public Sprite Sprite;
		protected ItemDatium m_ID;

		protected virtual string theName { get { return this.GetType().Name; } }
		public virtual void OnSelect(GameObject Player) { }
		public GameObject ItemInstance;

		public Item(ItemData id)
		{
			foreach (ItemDatium i in id.Items)
			{
				if (i.Name == theName)
				{
					Name = i.Name;
					Sprite = i.Sprite;
					m_ID = i;
					break;
				}
			}
		}
	}

	public class EmptyItem : Item
	{
		public EmptyItem(ItemData id) : base(id)
		{
		}
	}

	public abstract class MaterialItem : Item
	{
		public MaterialItem(ItemData id) : base(id)
		{
		}
	}

	public abstract class DecorationItem : Item
	{
		/// <summary>
		/// OccupySize.x means rows occupies, y means columns occupies
		/// </summary>
		public virtual string[,] OccupySize { get; }
		public DecorationItem(ItemData id) : base(id)
		{
		}
	}

	public class Rug : DecorationItem
	{
		public Rug(ItemData id) : base(id)
		{
		}

		public override string[,] OccupySize => new string[1, 4] { { "GroundGrid", "GroundGrid", "GroundGrid", "GroundGrid" } };

		public override void OnSelect(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			ItemInstance = GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
		}
	}

	public class Scroll : DecorationItem
	{
		public Scroll(ItemData id) : base(id)
		{
		}

		public override string[,] OccupySize => new string[4, 1] { { "WallGrid" }, { "WallGrid" }, { "WallGrid" }, { "GroundGrid" } };

		public override void OnSelect(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			ItemInstance = GameObject.Instantiate(Resources.Load("D_Scroll") as GameObject);
		}
	}

	public class Wood : MaterialItem
	{
		public Wood(ItemData id) : base(id)
		{
		}
	}

	public class Charcoal : MaterialItem
	{
		public Charcoal(ItemData id) : base(id)
		{
		}
	}
}
