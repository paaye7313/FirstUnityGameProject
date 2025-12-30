using UnityEngine;
using UnityEngine.Tilemaps;

public class Switch : MonoBehaviour
{
    public Vector2Int switchGridPos;
    public Tilemap tilemap;            // ±‚¡ÿ ≈∏¿œ∏ 
    public DoorController targetDoor;
    private bool isUsed = false;

    void Start()
    {
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        switchGridPos = new Vector2Int(cellPos.x, cellPos.y);
    }

    public void CheckSwitch(Vector2Int playerGridPos)
    {
        if (isUsed) return;

        if (playerGridPos == switchGridPos)
        {
            isUsed = true;
            targetDoor.OpenDoor();
        }
    }
}