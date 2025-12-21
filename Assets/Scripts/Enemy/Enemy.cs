using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public Vector2Int enemyGridPos;
    public Tilemap tilemap;   // ±‚¡ÿ ≈∏¿œ∏ 

    void Start()
    {
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        enemyGridPos = new Vector2Int(cellPos.x, cellPos.y);
    }
}
