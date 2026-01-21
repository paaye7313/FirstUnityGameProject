using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorZone : MonoBehaviour
{
    public Vector2Int gridPos;
    public Tilemap 기준Tilemap;
    public ColorChangeManager ccm;

    void Start()
    {
        Vector3Int cell = 기준Tilemap.WorldToCell(transform.position);
        gridPos = new Vector2Int(cell.x, cell.y);
    }
}
