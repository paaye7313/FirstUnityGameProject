using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerGridMovement : MonoBehaviour
{
    public float moveDuration = 0.15f;
    public Tilemap wallTilemap;   // 벽 타일맵 참조
    public float moveDelay = 0.15f;
    private bool isMoving = false;
    private Vector2Int gridPosition;

    void Start()
    {
        gridPosition = Vector2Int.RoundToInt(transform.position);
        transform.position = GridToWorld(gridPosition);
    }

    void Update()
    {
        if (isMoving) return;

        Vector2Int inputDir = Vector2Int.zero;

        if (Input.GetKey(KeyCode.W)) inputDir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S)) inputDir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A)) inputDir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D)) inputDir = Vector2Int.right;

        if (inputDir != Vector2Int.zero)
        {
            TryMove(inputDir);
        }
    }

    bool IsWall(Vector2Int gridPos)
    {
        Vector3Int cellPos = new Vector3Int(gridPos.x, gridPos.y, 0);
        return wallTilemap.HasTile(cellPos);
    }
    IEnumerator Move(Vector2Int targetGridPos)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = GridToWorld(targetGridPos);

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        gridPosition = targetGridPos;
        isMoving = false;
    }
    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
    }
    Vector2Int GetInputDirection()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return Vector2Int.zero;

        if (kb.wKey.wasPressedThisFrame) return Vector2Int.up;
        if (kb.sKey.wasPressedThisFrame) return Vector2Int.down;
        if (kb.aKey.wasPressedThisFrame) return Vector2Int.left;
        if (kb.dKey.wasPressedThisFrame) return Vector2Int.right;

        return Vector2Int.zero;
    }
    void TryMove(Vector2Int dir)
    {
        Vector2Int targetGridPos = gridPosition + dir;

        if (IsWall(targetGridPos))
            return;

        StartCoroutine(MoveRoutine(targetGridPos));
    }
    IEnumerator MoveRoutine(Vector2Int targetGridPos)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = GridToWorld(targetGridPos);

        float elapsed = 0f;

        while (elapsed < moveDelay)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDelay;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        gridPosition = targetGridPos;
        isMoving = false;
    }

}

