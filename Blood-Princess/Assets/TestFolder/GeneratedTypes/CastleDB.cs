
using UnityEngine;
using CastleDBImporter;
using System.Collections.Generic;
using System;

namespace CompiledTypes
{
    public class CastleDB
    {
        static CastleDBParser parsedDB;
        public MonsterDBType MonsterDB;
public ItemDBType ItemDB;

        public CastleDB(TextAsset castleDBAsset)
        {
            parsedDB = new CastleDBParser(castleDBAsset);
            MonsterDB = new MonsterDBType();ItemDB = new ItemDBType();
        }
        public class MonsterDBType 
 {public MonsterDB Enemy1 { get { return Get(CompiledTypes.MonsterDB.RowValues.Enemy1); } } 
private MonsterDB Get(CompiledTypes.MonsterDB.RowValues line) { return new MonsterDB(parsedDB.Root, line); }

                public MonsterDB[] GetAll() 
                {
                    var values = (CompiledTypes.MonsterDB.RowValues[])Enum.GetValues(typeof(CompiledTypes.MonsterDB.RowValues));
                    MonsterDB[] returnList = new MonsterDB[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF MonsterDB 
public class ItemDBType 
 {public ItemDB HEALINGPOTION { get { return Get(CompiledTypes.ItemDB.RowValues.HEALINGPOTION); } } 
public ItemDB MONEY { get { return Get(CompiledTypes.ItemDB.RowValues.MONEY); } } 
private ItemDB Get(CompiledTypes.ItemDB.RowValues line) { return new ItemDB(parsedDB.Root, line); }

                public ItemDB[] GetAll() 
                {
                    var values = (CompiledTypes.ItemDB.RowValues[])Enum.GetValues(typeof(CompiledTypes.ItemDB.RowValues));
                    ItemDB[] returnList = new ItemDB[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        returnList[i] = Get(values[i]);
                    }
                    return returnList;
                }
 } //END OF ItemDB 

    }
}