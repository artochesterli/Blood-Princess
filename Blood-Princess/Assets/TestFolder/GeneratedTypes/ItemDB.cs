
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class ItemDB
    {
        public string id;
public int Price;

        public enum RowValues { 
HEALINGPOTION15, 
MONEY, 
BATTLEARTABILITYOBJECT, 
PASSIVEABILITYOBJECT, 
ALLABILITYOBJECT, 
HEALINGPOTION30, 
HEALINGPOTION100, 
CHARCOAL, 
CLOTH, 
WOOD
 } 
        public ItemDB (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("ItemDB").Rows[(int)line];
id = node["id"];
Price = node["Price"].AsInt;

        }  
        
public static ItemDB.RowValues GetRowValue(string name)
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