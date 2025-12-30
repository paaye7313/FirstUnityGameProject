using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTilemapManager : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap enemyTilemap;

    [Header("Prefab")]
    public GameObject enemyPrefab;

    void Start()
    {
        SpawnEnemiesFromTilemap();
    }

    void SpawnEnemiesFromTilemap()
    {
        // Å¸ÀÏ¸ÊÀÇ ¸ðµç ¼¿À» ¼øÈ¸
        foreach (Vector3Int cellPos in enemyTilemap.cellBounds.allPositionsWithin)
        {
            if (!enemyTilemap.HasTile(cellPos))
                continue;

            Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);

            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.GetComponent<Enemy>().Init(gridPos);
        }

        // Å¸ÀÏ¸ÊÀº ¸¶Ä¿ÀÌ¹Ç·Î ¼û±è
        enemyTilemap.gameObject.SetActive(false);
    }
}
