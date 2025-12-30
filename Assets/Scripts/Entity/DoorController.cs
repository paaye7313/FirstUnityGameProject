using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class DoorController : MonoBehaviour
{
    public Tilemap doorTilemap;

    private List<Vector3Int> doorCells = new List<Vector3Int>();
    private bool isOpen = false;

    void Awake()
    {
        CollectDoorCells();
    }

    void CollectDoorCells()
    {
        doorCells.Clear();

        BoundsInt bounds = doorTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (doorTilemap.HasTile(pos))
            {
                doorCells.Add(pos);
            }
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        foreach (var cell in doorCells)
        {
            doorTilemap.SetTile(cell, null);
        }

        isOpen = true;
    }

    public void CloseDoor()
    {
        // 필요 시 구현
    }
}
