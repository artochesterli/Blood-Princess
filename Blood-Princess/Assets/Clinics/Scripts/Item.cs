using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace Clinic
{
	[XmlInclude(typeof(Rug))]
	[XmlInclude(typeof(EmptyItem))]
	[XmlInclude(typeof(OakSeed))]
	[XmlInclude(typeof(AppleSeed))]
	[XmlInclude(typeof(Scroll))]
	[XmlInclude(typeof(Wood))]
	[XmlInclude(typeof(Charcoal))]
	[XmlInclude(typeof(Cloth))]
	[XmlInclude(typeof(Tub))]
	public abstract class Item
	{
		public string Name = "";
		public int Number = 1;
		//public Sprite Sprite;
		protected ItemDatium m_ID;

		protected virtual string theName { get { return this.GetType().Name; } }
		public virtual void OnSelect(GameObject Player) { }
		//public GameObject ItemInstance;

		public Item(ItemData id)
		{
			ItemDatium i = id.GetItem(theName);
			if (i != null)
			{
				Name = i.Name;
				m_ID = i;
			}
		}

		public Item()
		{
			ItemDatium i = Resources.Load<ItemData>("ItemData").GetItem(theName);
			if (i != null)
			{
				Name = i.Name;
				m_ID = i;
			}
		}

		public ItemDatium GetID()
		{
			return m_ID;
		}
	}

	public class EmptyItem : Item
	{
		public EmptyItem(ItemData id) : base(id)
		{
		}
		public EmptyItem() : base() { }
	}

	public abstract class SeedItem : Item
	{
		public SeedItem()
		{
		}

		public SeedItem(ItemData id) : base(id)
		{
		}
	}

	public class OakSeed : SeedItem
	{
		public OakSeed()
		{
		}

		public OakSeed(ItemData id) : base(id)
		{
		}
	}

	public class AppleSeed : SeedItem
	{
		public AppleSeed()
		{
		}

		public AppleSeed(ItemData id) : base(id)
		{
		}
	}

	public abstract class MaterialItem : Item
	{
		public MaterialItem()
		{
		}

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

		public DecorationItem()
		{
		}

		public abstract GameObject OnSelectObject(GameObject Player);

		public abstract GameObject GetInstantiatedObject();
	}

	public class Rug : DecorationItem
	{
		public Rug()
		{
		}

		public Rug(ItemData id) : base(id)
		{
		}

		public override string[,] OccupySize => new string[1, 4] { { "GroundGrid", "GroundGrid", "GroundGrid", "GroundGrid" } };

		public override GameObject GetInstantiatedObject()
		{
			return GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
		}

		public override void OnSelect(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			//ItemInstance = GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
			GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
		}

		public override GameObject OnSelectObject(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			//ItemInstance = GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
			return GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
		}
	}

	public class Tub : DecorationItem
	{
		public Tub(ItemData id) : base(id)
		{
		}

		public Tub()
		{
		}

		public override string[,] OccupySize => new string[1, 4] { { "GroundGrid", "GroundGrid", "GroundGrid", "GroundGrid" } };


		public override GameObject GetInstantiatedObject()
		{
			return GameObject.Instantiate(Resources.Load("D_Tub") as GameObject);
		}

		public override GameObject OnSelectObject(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			//ItemInstance = GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
			return GameObject.Instantiate(Resources.Load("D_Tub") as GameObject);
		}

		public override void OnSelect(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			//ItemInstance = GameObject.Instantiate(Resources.Load("D_Rug") as GameObject);
			GameObject.Instantiate(Resources.Load("D_Tub") as GameObject);
		}
	}


	public class Scroll : DecorationItem
	{
		public Scroll()
		{
		}

		public Scroll(ItemData id) : base(id)
		{
		}

		public override string[,] OccupySize => new string[4, 1] { { "WallGrid" }, { "WallGrid" }, { "WallGrid" }, { "GroundGrid" } };

		public override GameObject GetInstantiatedObject()
		{
			return GameObject.Instantiate(Resources.Load("D_Scroll") as GameObject);
		}

		public override void OnSelect(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			//ItemInstance = GameObject.Instantiate(Resources.Load("D_Scroll") as GameObject);
			GameObject.Instantiate(Resources.Load("D_Scroll") as GameObject);
		}

		public override GameObject OnSelectObject(GameObject Player)
		{
			Player.GetComponent<InventoryUI>().OnUseDecoration(this);
			//ItemInstance = GameObject.Instantiate(Resources.Load("D_Scroll") as GameObject);
			return GameObject.Instantiate(Resources.Load("D_Scroll") as GameObject);
		}
	}

	public class Wood : MaterialItem
	{
		public Wood()
		{
		}

		public Wood(ItemData id) : base(id)
		{
		}
	}

	public class Cloth : MaterialItem
	{
		public Cloth()
		{
		}

		public Cloth(ItemData id) : base(id)
		{
		}
	}


	public class Charcoal : MaterialItem
	{
		public Charcoal()
		{
		}

		public Charcoal(ItemData id) : base(id)
		{
		}
	}
}
