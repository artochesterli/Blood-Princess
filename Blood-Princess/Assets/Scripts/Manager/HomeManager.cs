using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class HomeManager
{
	public HomeDatabase m_HomeDatabase;
	private string m_SavingPath;

	public HomeManager()
	{
		m_SavingPath = Application.dataPath + "/DesignData/HomeData.xml";
		m_HomeDatabase = new HomeDatabase();
		m_HomeDatabase.SetValue(m_SavingPath);
	}

	public void OnSave()
	{
		var serializer = new XmlSerializer(typeof(HomeDatabase));
		using (var stream = new FileStream(m_SavingPath, FileMode.Create))
		{
			serializer.Serialize(stream, m_HomeDatabase);
		}
	}

	public void OnSave(Vector2Int buildPosition, Clinic.DecorationItem decorationItem)
	{
		m_HomeDatabase.SaveDecortaion(buildPosition, decorationItem);
		OnSave();
	}

	public Clinic.Vector2IntAndDecorationItemList OnLoad()
	{
		return new Clinic.Vector2IntAndDecorationItemList(m_HomeDatabase.BuildPositions, m_HomeDatabase.DecorationItems);
	}

	public void Destroy()
	{

	}
}
