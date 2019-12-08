using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clinic;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// A Save/Load Manager for Storage
/// </summary>
public class StorageManager
{
	public int MaxSlots = 15;
	private string m_SavingPath;
	private StorageDatabase m_StorageDatabase;

	public StorageManager()
	{
		m_SavingPath = Application.dataPath + "/DesignData/StorageData.xml";
		m_StorageDatabase = new StorageDatabase();
		m_StorageDatabase.SetValue(m_SavingPath, MaxSlots);
	}

	/// <summary>
	/// Save the Item just got
	/// </summary>
	/// <param name="item"></param>
	/// <param name="num">if negative, then remove the item just got</param>
	public void SaveItem(Item item, int num = 1)
	{
		m_StorageDatabase.Saveitem(item, num);
		var serializer = new XmlSerializer(typeof(StorageDatabase));
		using (var stream = new FileStream(m_SavingPath, FileMode.Create))
		{
			serializer.Serialize(stream, m_StorageDatabase);
		}
	}

	/// <summary>
	/// Load the current items in storage
	/// </summary>
	/// <returns></returns>
	public List<Item> LoadItem()
	{
		return m_StorageDatabase.Items;
	}

	public void Destroy()
	{

	}
}
