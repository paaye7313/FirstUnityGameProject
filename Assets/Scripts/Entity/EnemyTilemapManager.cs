using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemyTilemapManager : MonoBehaviour
{
    public static EnemyTilemapManager Instance;

    [Header("Tilemap")]
    public Tilemap enemyTilemap;

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    private List<Enemy> enemies = new List<Enemy>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnEnemiesFromTilemap();
    }

    public void SpawnEnemiesFromTilemap()
    {
        ClearEnemies();

        foreach (var cellPos in enemyTilemap.cellBounds.allPositionsWithin)
        {
            if (!enemyTilemap.HasTile(cellPos))
                continue;

            Vector3 worldPos = enemyTilemap.GetCellCenterWorld(cellPos);
            GameObject enemyObj = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.SetGridPosition(cellPos);

            enemies.Add(enemy);
        }
    }

    void ClearEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    public bool IsEnemyAt(Vector2Int gridPos)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.enemyGridPos == gridPos)
                return true;
        }
        return false;
    }
}
