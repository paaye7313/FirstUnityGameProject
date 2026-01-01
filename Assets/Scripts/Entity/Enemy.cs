using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public Vector2Int enemyGridPos;

    public void SetGridPosition(Vector3Int cellPos)
    {
        enemyGridPos = new Vector2Int(cellPos.x, cellPos.y);
    }
}
