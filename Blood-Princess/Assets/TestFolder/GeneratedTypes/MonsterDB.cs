
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class MonsterDB
    {
        public string id;
public int HitPoints;
public string Sprite;
public List<Drops> DropsList = new List<Drops>();

        public enum RowValues { 
Enemy1
 } 
        public MonsterDB (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("MonsterDB").Rows[(int)line];
id = node["id"];
HitPoints = node["HitPoints"].AsInt;
Sprite = node["Sprite"];
foreach(var item in node["Drops"]) { DropsList.Add(new Drops(root, item));}

        }  
        
public static MonsterDB.RowValues GetRowValue(string name)
{
    var values = (RowValues[])Enum.GetValues(typeof(RowValues));
    for (int i = 0; i < values.Length; i++)
    {
        if(values[i].ToString() == name)
        {
            return values[i];
        }
    }
    return values[0];
}
    }
}