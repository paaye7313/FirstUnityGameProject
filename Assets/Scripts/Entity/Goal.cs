using UnityEngine;
using UnityEngine.Tilemaps;

public class Goal : MonoBehaviour
{
    public Vector2Int goalGridPos;  // 골의 그리드좌표
    public Tilemap tilemap;   // 기준 타일맵

    void Start()
    {
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);  //현재 월드좌표를 그리드좌표로 보정
        goalGridPos = new Vector2Int(cellPos.x, cellPos.y);
    }
}
