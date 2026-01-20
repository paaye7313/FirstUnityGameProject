using UnityEngine;
using UnityEngine.Tilemaps;

public class Goal : MonoBehaviour
{
    public Vector2Int goalGridPos;  // 그리드 좌표
    public Tilemap tilemap;   // 기준 타일맵

    void Start()
    {
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);  //월드 좌표를 그리드 좌표로 변경
        goalGridPos = new Vector2Int(cellPos.x, cellPos.y);
    }
}
