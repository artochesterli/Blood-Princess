
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace CompiledTypes
{ 
    public class Drops
    {
        public ItemDB Item;
public float Weight;
public int Quantity;

         
        public Drops (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            Item = new CompiledTypes.ItemDB(root,CompiledTypes.ItemDB.GetRowValue(node["Item"]));
Weight = node["Weight"].AsFloat;
Quantity = node["Quantity"].AsInt;

        }  
        
    }
}