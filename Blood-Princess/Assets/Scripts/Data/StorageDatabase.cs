using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Clinic;

[XmlRoot("StorageData")]
public class StorageDatabase
{
	[XmlArray("ItemList"), XmlArrayItem("Item")]
	public List<Item> Items;

	public StorageDatabase()
	{
	}

	public void SetValue(string _savingPath, int maxSlots)
	{
		Items = new List<Item>();
		if (!File.Exists(_savingPath))
		{
			File.Create(_savingPath);
		}
		else
		{
			var serializer = new XmlSerializer(typeof(StorageDatabase));
			using (var stream = new FileStream(_savingPath, FileMode.Open))
			{
				var _storageDatabase = serializer.Deserialize(stream) as StorageDatabase;
				Items = _storageDatabase.Items;
			}
		}

		if (Items.Count == 0)
		{
			for (int i = 0; i < maxSlots; i++)
			{
				Items.Add(new EmptyItem());
			}
		}
	}

	public void Saveitem(Item item, int num)
	{
		for (int i = 0; i < Items.Count; i++)
		{
			if (Items[i].GetType().Equals(item.GetType()))
			{
				Items[i].Number += num;
				if (Items[i].Number <= 0)
				{
					Items[i] = new EmptyItem();
				}
				return;
			}
		}
		for (int i = 0; i < Items.Count; i++)
		{
			if (Items[i].GetType().Equals(typeof(EmptyItem)))
			{
				Items[i] = item;
				return;
			}
		}
	}
}
