using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG;

public class EnemyGenerationManager
{
    private List<EnemyInfo> m_EnemyInfo;
    private EnemyGenerationScriptableObject EnemyGenerationData;
    private System.Random m_Rand;
    public EnemyGenerationManager(EnemyGenerationScriptableObject enemygenerationdata)
    {
        EnemyGenerationData = enemygenerationdata;
        m_Rand = new System.Random();
        m_EnemyInfo = new List<EnemyInfo>();
    }

    public void RecordNextEnemyInfo(string type, IntVector2 boardPosition)
    {
        m_EnemyInfo.Add(new EnemyInfo(type, boardPosition));
    }

    public void GenerateEnemies(string[,] _board, GameObject _boardGameObject)
    {
        for (int i = 0; i < m_EnemyInfo.Count; i++)
        {
            EnemyInfo info = m_EnemyInfo[i];
            if (i <= m_EnemyInfo.Count * 0.1f && i % 4 == 0)
            {
                m_GenerateEnemy(info.EnemyType, info.EnemyPosition, _board, _boardGameObject);
                continue;
            }
            if (i >= m_EnemyInfo.Count * 0.1f && i <= m_EnemyInfo.Count * 0.4f && i % 3 == 0)
            {
                m_GenerateEnemy(info.EnemyType, info.EnemyPosition, _board, _boardGameObject);
                continue;
            }
            if (i >= m_EnemyInfo.Count * 0.4f && i <= m_EnemyInfo.Count * 0.6f && i % 2 == 0)
            {
                m_GenerateEnemy(info.EnemyType, info.EnemyPosition, _board, _boardGameObject);
                continue;
            }
            if (i >= m_EnemyInfo.Count * 0.6f && i <= m_EnemyInfo.Count * 0.8f)
            {
                m_GenerateEnemy(info.EnemyType, info.EnemyPosition, _board, _boardGameObject);
                continue;
            }
            if (i >= m_EnemyInfo.Count * 0.8f && i <= m_EnemyInfo.Count && i % 2 == 0)
            {
                m_GenerateEnemy(info.EnemyType, info.EnemyPosition, _board, _boardGameObject);
                continue;
            }
        }
        m_EnemyInfo.Clear();
    }

    private void m_GenerateEnemy(string type, IntVector2 boardPosition, string[,] board, GameObject _boardGameObject)
    {
        Vector2 curTileWorldPosition = Vector2.zero +
                new Vector2(boardPosition.x * PCG.Utility.TileSize().x, boardPosition.y * PCG.Utility.TileSize().y);

        GameObject instantiatedEnmey = null;
        switch (type)
        {
            case "M-F":
                instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
                break;
            case "M-M":
                instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy2", typeof(GameObject))) as GameObject;
                break;
            case "M-S":
                instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/SoulWarrior", typeof(GameObject))) as GameObject;
                break;
            case "M-1":
                int randInt = m_Rand.Next(1, 101);
                if (randInt < 30)
                    instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy1", typeof(GameObject))) as GameObject;
                else if (randInt < 65)
                    instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/Enemy2", typeof(GameObject))) as GameObject;
                else if (randInt < 101)
                    instantiatedEnmey = GameObject.Instantiate(Resources.Load("Prefabs/SoulWarrior", typeof(GameObject))) as GameObject;
                break;
        }


        if (instantiatedEnmey != null)
        {
            _initializeAI(instantiatedEnmey, boardPosition, board);
            instantiatedEnmey.transform.parent = _boardGameObject.transform;
            instantiatedEnmey.transform.position = curTileWorldPosition;
            if (instantiatedEnmey.layer == LayerMask.NameToLayer("Enemy"))
            {
                instantiatedEnmey.GetComponent<SpeedManager>().SetInitInfo();
                instantiatedEnmey.GetComponent<SpeedManager>().MoveToPoint(curTileWorldPosition +
                (Vector2.up * (instantiatedEnmey.GetComponent<BoxCollider2D>().size.y / 2f - PCG.Utility.TileSize().y / 2f)));
            }
        }
    }

    private void _initializeAI(GameObject AI, IntVector2 worldPosition, string[,] _board)
    {
        // Go Left and Check
        bool leftIsWall = false;
        bool leftIsEdge = false;
        int currentX = worldPosition.x;
        while (!leftIsWall && !leftIsEdge)
        {
            string leftDownPos = _board[currentX - 1, worldPosition.y - 1];
            string leftPos = _board[currentX - 1, worldPosition.y];
            //leftIsWall = (leftPos != "" && leftPos != "0" && leftPos != "6" && leftPos != "e" && leftPos != "a" && leftPos != "b" && leftPos != "7");
            leftIsWall = !PCG.Utility.OnlyContainsHashset(leftPos, PCG.Utility.EmptyStrHashSet);
            //leftIsEdge = (leftPos == "" || leftPos == "0") && (leftDownPos == "" || leftDownPos == "0");
            leftIsEdge = PCG.Utility.OnlyContainsHashset(leftPos, PCG.Utility.EmptyStrHashSet) && PCG.Utility.OnlyContainsHashset(leftDownPos, PCG.Utility.EmptyStrHashSet);
            currentX--;
        }
        if (AI.name.Contains("Knight") || AI.name.Contains("SoulWarrior"))
        {
            AI.transform.Find("PatronLeftMark").localPosition = PCG.Utility.BoardPositionToWorldPosition(new IntVector2(currentX + 2, worldPosition.y) - worldPosition);
            AI.transform.Find("DetectLeftMark").localPosition = PCG.Utility.BoardPositionToWorldPosition(new IntVector2(currentX + 1, worldPosition.y) - worldPosition);
        }
        // Go Right and Check
        bool rightIsWall = false;
        bool RightIsEdge = false;
        currentX = worldPosition.x;
        while (!rightIsWall && !RightIsEdge)
        {
            string rightDownPos = _board[currentX + 1, worldPosition.y - 1];
            string rightPos = _board[currentX + 1, worldPosition.y];
            //rightIsWall = (rightPos != "" && rightPos != "0" && rightPos != "6" && rightPos != "e" && rightPos != "a" && rightPos != "b" && rightPos != "7");
            rightIsWall = !PCG.Utility.OnlyContainsHashset(rightPos, PCG.Utility.EmptyStrHashSet);
            //RightIsEdge = (rightPos == "" || rightPos == "0") && (rightDownPos == "" || rightDownPos == "0");
            RightIsEdge = PCG.Utility.OnlyContainsHashset(rightPos, PCG.Utility.EmptyStrHashSet) && PCG.Utility.OnlyContainsHashset(rightDownPos, PCG.Utility.EmptyStrHashSet);
            currentX++;
        }
        if (AI.name.Contains("Knight") || AI.name.Contains("SoulWarrior"))
        {
            AI.transform.Find("PatronRightMark").localPosition = PCG.Utility.BoardPositionToWorldPosition(new IntVector2(currentX - 2, worldPosition.y) - worldPosition);
            AI.transform.Find("DetectRightMark").localPosition = PCG.Utility.BoardPositionToWorldPosition(new IntVector2(currentX - 1, worldPosition.y) - worldPosition);
        }
    }

    public void Destroy()
    {

    }
}
