using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
	public string Name;
	public int Number = 1;
	public Sprite Sprite;
	public float Interval;
	public float VitalityDrain;
	protected virtual string theName { get; }
	public virtual bool NeedSelected { get; }
	protected ItemDatium ID;
	protected ItemData ItemData;
	public abstract void OnLeftClickUse(GameObject Player);
	public abstract void OnRightClickUse(GameObject Player);
	public Item(ItemData id)
	{
		ItemData = id;
		foreach (ItemDatium i in id.Items)
		{
			if (i.Name == theName)
			{
				Name = i.Name;
				Sprite = i.Sprite;
				Interval = i.Interval;
				VitalityDrain = i.VitalityDrain;
				ID = i;
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

	public override void OnLeftClickUse(GameObject Player)
	{
		throw new System.NotImplementedException();
	}

	public override void OnRightClickUse(GameObject Player)
	{
		throw new System.NotImplementedException();
	}
}
