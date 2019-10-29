using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG;

public class MapManager : MonoBehaviour
{
    public IntVector2 BoardDimension = new IntVector2(4, 4);
    public int Length;
    public GameObject Zone;
    // Start is called before the first frame update
    void Awake()
    {
        int seed = Random.Range(0, 100000);
        Debug.Log(seed);
        // Set Zone according to the board Dimension
        Zone.GetComponent<BoxCollider>().size = new Vector3(BoardDimension.x * PCG.Utility.TileSize().x, BoardDimension.y * PCG.Utility.TileSize().y);
        Zone.GetComponent<BoxCollider>().center = new Vector3(BoardDimension.x * PCG.Utility.TileSize().x, BoardDimension.y * PCG.Utility.TileSize().y) / 2f;
        Board board = new Board(BoardDimension.x, BoardDimension.y, seed, Length);
        //Utility.MaxFileHeight();
    }
}
