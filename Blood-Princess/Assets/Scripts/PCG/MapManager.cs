using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG;

public class MapManager : MonoBehaviour
{
	public IntVector2 BoardDimension = new IntVector2(4, 4);
	public int Length;
	// Start is called before the first frame update
	void Start()
	{
		Board board = new Board(BoardDimension.x, BoardDimension.y, Random.Range(0, 100000), Length);
		//Room rm = new Room(new IntVector2(0, 0), new IntVector2(1, 1), 200, 3, gameObject);
	}
}
