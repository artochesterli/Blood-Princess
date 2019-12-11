using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using Clinic;

[XmlRoot("HomeData")]
public class HomeDatabase
{
	public List<Vector2Int> BuildPositions;
	public List<DecorationItem> DecorationItems;

	public HomeDatabase()
	{

	}

	public void SetValue(string _savingPath)
	{
		if (!File.Exists(_savingPath))
		{
			var myFile = File.Create(_savingPath);
			myFile.Close();
			BuildPositions = new List<Vector2Int>();
			DecorationItems = new List<DecorationItem>();
		}
		else
		{
			var serializer = new XmlSerializer(typeof(HomeDatabase));
			using (var stream = new FileStream(_savingPath, FileMode.Open))
			{
				var _homeDatabase = serializer.Deserialize(stream) as HomeDatabase;
				BuildPositions = _homeDatabase.BuildPositions;
				DecorationItems = _homeDatabase.DecorationItems;
			}
		}
	}

	public void SaveDecortaion(Vector2Int position, DecorationItem decorationItem)
	{
		// Need to make sure StartPosition is not the same
		foreach (var buildPos in BuildPositions)
		{
			if (buildPos.Equals(position)) return;
		}

		BuildPositions.Add(position);
		DecorationItems.Add(decorationItem);
	}
}
