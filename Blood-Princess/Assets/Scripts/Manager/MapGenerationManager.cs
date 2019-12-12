using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PCG;

public class MapGenerationManager
{
    private MapGenerationScriptableObject MapGenerationData;
    private List<int> MapBag;
    private List<int> S1Bag;
    private List<int> S2Bag;
    private List<int> S3Bag;
    private System.Random m_rand;

    public MapGenerationManager(MapGenerationScriptableObject mapGenerationScriptableObject)
    {
        MapGenerationData = mapGenerationScriptableObject;
        m_rand = new System.Random();
        MapBag = new List<int>(new int[MapGenerationData.MapBag.Count]);
        S1Bag = new List<int>(new int[MapGenerationData.S1Bag.Count]);
        S2Bag = new List<int>(new int[MapGenerationData.S2Bag.Count]);
        S3Bag = new List<int>(new int[MapGenerationData.S3Bag.Count]);
        _fillbag(ref MapBag, MapGenerationData.MapBag);
        _fillbag(ref S1Bag, MapGenerationData.S1Bag);
        _fillbag(ref S2Bag, MapGenerationData.S2Bag);
        _fillbag(ref S3Bag, MapGenerationData.S3Bag);
    }

    private int _getFileFromBag(ref List<int> bag, List<int> bagTemplate)
    {
        // Check if bag is empty, if so full it
        _fillbag(ref bag, bagTemplate);
        // Find a index with higher than 0 element in it
        int nextIndex = m_rand.Next(0, bag.Count);
        while (bag[nextIndex] <= 0)
        {
            nextIndex = m_rand.Next(0, bag.Count);
        }
        // Found it, decrement the value at that index
        bag[nextIndex]--;
        // Return the index
        return nextIndex;

    }

    /// <summary>
    /// Fill Bag if Necessary
    /// </summary>
    /// <param name="bag"></param>
    /// <param name="bagTemplate"></param>
    private void _fillbag(ref List<int> bag, List<int> bagTemplate)
    {
        Debug.Assert(bag.Count == bagTemplate.Count, "Bag and Bagtemplate Count mismatch one has: " + bag.Count + " another has: " + bagTemplate.Count);
        for (int i = 0; i < bag.Count; i++)
        {
            if (bag[i] > 0) return;
        }
        // Fill bag according to template
        for (int i = 0; i < bag.Count; i++)
        {
            bag[i] = bagTemplate[i];
        }
    }

    public string[][] GetNextRoomFromRoomType(string _roomType)
    {
        // Load Stream Reader According to RoomType
        string middlepath = "/PCG/RoomType";
        if (_roomType.Contains("s"))
        {
            middlepath = "/PCG/Expandable/";
        }
        string path = Application.dataPath + middlepath + _roomType.ToString();
        int file = 0;
        switch (_roomType)
        {
            case "0":
                file = _getFileFromBag(ref MapBag, MapGenerationData.MapBag);
                break;
            case "s.1":
                file = _getFileFromBag(ref S1Bag, MapGenerationData.S1Bag);
                break;
            case "s.2":
                file = _getFileFromBag(ref S2Bag, MapGenerationData.S2Bag);
                break;
            case "s.3":
                file = _getFileFromBag(ref S3Bag, MapGenerationData.S3Bag);
                break;
        }
        path += ("/" + file.ToString() + ".csv");
        StreamReader _streamReader = new StreamReader(path);

        // Read File into manipulable string array
        string entireFile = _streamReader.ReadToEnd();
        string[] choppedUpFile = entireFile.Split('\n');
        string[][] result = new string[choppedUpFile.Length][];
        for (int i = 0; i < choppedUpFile.Length; i++)
        {
            string[] oneline = choppedUpFile[i].Split(',');
            result[i] = oneline;
        }

        // Rotatte entireRoomFile ClockWise 90
        int x = result.Length;
        int y = result[0].Length;
        string[][] temp = new string[y][];
        for (int i = 0; i < y; i++)
        {
            temp[i] = new string[x];
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                temp[i][j] = result[x - j - 1][i];
                temp[i][j] = temp[i][j].Trim();
            }
        }
        return temp;
    }

    public void Destroy()
    {

    }
}
