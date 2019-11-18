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
		public virtual Vector2 OccupySize { get; }
		public DecorationItem(ItemData id) : base(id)
		{
		}
	}

	public class Rug : DecorationItem
	{
		public Rug(ItemData id) : base(id)
		{
		}

		public override Vector2 OccupySize => base.OccupySize;

		public override void OnSelect(GameObject Player)
		{
			Debug.Log("On Selected Rug");
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
