using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG;

public class MapManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Room newRoom = new Room(new IntVector2(0, 0), Random.Range(0, 10000), 1);
        Board board = new Board(4, 4, Random.Range(0, 100000));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
