
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
public MonsterDB Knight { get { return Get(CompiledTypes.MonsterDB.RowValues.Knight); } } 
public MonsterDB SoulWarrior { get { return Get(CompiledTypes.MonsterDB.RowValues.SoulWarrior); } } 
public MonsterDB Enemy2 { get { return Get(CompiledTypes.MonsterDB.RowValues.Enemy2); } } 
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
 {public ItemDB HEALINGPOTION15 { get { return Get(CompiledTypes.ItemDB.RowValues.HEALINGPOTION15); } } 
public ItemDB MONEY { get { return Get(CompiledTypes.ItemDB.RowValues.MONEY); } } 
public ItemDB BATTLEARTABILITYOBJECT { get { return Get(CompiledTypes.ItemDB.RowValues.BATTLEARTABILITYOBJECT); } } 
public ItemDB PASSIVEABILITYOBJECT { get { return Get(CompiledTypes.ItemDB.RowValues.PASSIVEABILITYOBJECT); } } 
public ItemDB ALLABILITYOBJECT { get { return Get(CompiledTypes.ItemDB.RowValues.ALLABILITYOBJECT); } } 
public ItemDB HEALINGPOTION30 { get { return Get(CompiledTypes.ItemDB.RowValues.HEALINGPOTION30); } } 
public ItemDB HEALINGPOTION100 { get { return Get(CompiledTypes.ItemDB.RowValues.HEALINGPOTION100); } } 
public ItemDB CHARCOAL { get { return Get(CompiledTypes.ItemDB.RowValues.CHARCOAL); } } 
public ItemDB CLOTH { get { return Get(CompiledTypes.ItemDB.RowValues.CLOTH); } } 
public ItemDB WOOD { get { return Get(CompiledTypes.ItemDB.RowValues.WOOD); } } 
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