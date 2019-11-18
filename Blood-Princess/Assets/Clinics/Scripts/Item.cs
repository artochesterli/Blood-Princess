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

		protected virtual string theName { get; }
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

	public abstract class DecorationItem : Item
	{
		public DecorationItem(ItemData id) : base(id)
		{
		}
	}

	public class Rug : DecorationItem
	{
		public Rug(ItemData id) : base(id)
		{
		}

		protected override string theName => "Rug";

		public override void OnSelect(GameObject Player)
		{

		}
	}
}
