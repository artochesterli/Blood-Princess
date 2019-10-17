using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG;

public class MapManager : MonoBehaviour
{
    public IntVector2 BoardDimension = new IntVector2(4, 4);
    // Start is called before the first frame update
    void Start()
    {
        Board board = new Board(BoardDimension.x, BoardDimension.y, Random.Range(0, 100000));
    }
}
