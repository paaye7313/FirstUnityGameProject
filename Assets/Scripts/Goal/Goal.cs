using UnityEngine;
using UnityEngine.Tilemaps;

public class Goal : MonoBehaviour
{
    public Vector2Int goalGridPos;
    public Tilemap tilemap;   // ±‚¡ÿ ≈∏¿œ∏ 

    void Start()
    {
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        goalGridPos = new Vector2Int(cellPos.x, cellPos.y);
    }
}
