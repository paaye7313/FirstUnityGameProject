using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerGridMovement : MonoBehaviour
{
    public float moveDuration = 0.15f;  //이동 속도
    public Tilemap wallTilemap;   // 벽 타일맵 참조
    public Tilemap groundTilemap;  // 바닥 타일맵 참조
    public float moveDelay = 0.15f;  // 이동 딜레이
    private bool isMoving = false;  // 이동 조작감지
    private Vector2Int gridPosition;  //타일멥상에서의 좌표
    public Vector2Int CurrentGridPosition => gridPosition;
    public Goal goal;  // 골인
    public Enemy[] enemies;  // 적
    public Vector2Int spawnGridPos;  // 스폰 포인트
    public SavePoint[] savePoints;

    void Start()  //시작시 호출
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);  //현재 월드좌표를 그리드 좌표로 보정
        gridPosition = new Vector2Int(cellPos.x, cellPos.y);
        spawnGridPos = gridPosition;  //스폰 포인트를 현재 그리드좌표로 설정

        transform.position = GridToWorld(gridPosition);  //현재 월드좌표를 그리드좌표로 보정
    }



    void Update()  //프레임마다 호출
    {
        if (isMoving) return;  // 이동중이면 리턴

        Vector2Int inputDir = Vector2Int.zero;  //이동 방향 초기화
        // 조작키에 맞는 이동방향 설정
        if (Input.GetKey(KeyCode.W)) inputDir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S)) inputDir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A)) inputDir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D)) inputDir = Vector2Int.right;

        if (inputDir != Vector2Int.zero)  //이동방향이 설정되었을 경우
        {
            TryMove(inputDir);  //이동 시도
        }
    }

    bool IsWall(Vector2Int gridPos)  //벽이 있는지 확인
    {
        Vector3Int cellPos = new Vector3Int(gridPos.x, gridPos.y, 0);  // 좌표를 3차원 좌표로 형변환
        return wallTilemap.HasTile(cellPos);  // 벽 타일이 있을경우 true
    }
    //IEnumerator Move(Vector2Int targetGridPos)  // 현재는 사용하지 않는 메소드
    //{
    //    isMoving = true;

    //    Vector3 startPos = transform.position;
    //    Vector3 targetPos = GridToWorld(targetGridPos);

    //    float elapsed = 0f;

    //    while (elapsed < moveDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / moveDuration;
    //        transform.position = Vector3.Lerp(startPos, targetPos, t);
    //        yield return null;
    //    }

    //    transform.position = targetPos;
    //    gridPosition = targetGridPos;
    //    isMoving = false;
    //}
    Vector3 GridToWorld(Vector2Int gridPos)  //그리드좌표를 월드좌표로 보정
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
    }
    //Vector2Int GetInputDirection()  // 현재는 사용하지 않는 메소드
    //{
    //    Keyboard kb = Keyboard.current;
    //    if (kb == null) return Vector2Int.zero;

    //    if (kb.wKey.wasPressedThisFrame) return Vector2Int.up;
    //    if (kb.sKey.wasPressedThisFrame) return Vector2Int.down;
    //    if (kb.aKey.wasPressedThisFrame) return Vector2Int.left;
    //    if (kb.dKey.wasPressedThisFrame) return Vector2Int.right;

    //    return Vector2Int.zero;
    //}
    void TryMove(Vector2Int dir)  //이동 시도
    {
        Vector2Int targetGridPos = gridPosition + dir;  //현재 좌표 + 이동 방향

        if (IsWall(targetGridPos))  //만약 이동 좌표에 벽이 있을경우 리턴
            return;

        StartCoroutine(MoveRoutine(targetGridPos));  //이동 루틴 시작
    }
    IEnumerator MoveRoutine(Vector2Int targetGridPos)  //이동 루틴
    {
        isMoving = true;  //이동 조작중

        Vector3 startPos = transform.position;  //시작좌표는 현재좌표
        Vector3 targetPos = GridToWorld(targetGridPos);  //끝좌표는 목표좌표

        float elapsed = 0f;  //이동 진행도

        while (elapsed < moveDelay)  //이동 진행도가 이동 딜레이에 도달할때까지
        {
            elapsed += Time.deltaTime; //이동 진행도 +1
            float t = elapsed / moveDelay;  //진행도 수준
            transform.position = Vector3.Lerp(startPos, targetPos, t);  // 진행도 수준에 따른 위치설정
            yield return null;  //다음 프레임으로 넘어감
        }

        transform.position = targetPos;  //현재 월드좌표를 목표 좌표로 변경
        gridPosition = targetGridPos;  //현재 그리드좌표를 목표 좌표로 변경
        isMoving = false;  //이동 조작 끝

        CheckEnemy();  //적 확인
        CheckGoal();  //골 확인
        CheckSave();  //세이브포인트 확인
    }

    void CheckGoal()  // 골 위치 확인
    {
        if (gridPosition == goal.goalGridPos)  //골이 나와 겹칠경우
        {
            GoalUI.Instance.ShowGoal();  // 골 글자를 표시
        }
    }
    void CheckEnemy()  //적 위치 확인
    {
        foreach (Enemy enemy in enemies)
        {
            if (gridPosition == enemy.enemyGridPos)  //적이 나와 겹칠경우
            {
                Respawn();
                return;
            }
        }
    }
    void Respawn()  // 리스폰
    {
        StopAllCoroutines(); //모든 Coroutine 중단
        isMoving = false;  //이동 조작 취소

        gridPosition = spawnGridPos;  //현재 그리드좌표를 스폰좌표로 이동
        transform.position = GridToWorld(spawnGridPos);  //현재 월드좌표를 스폰좌표로 이동
    }
    void CheckSave()
    {
        foreach (SavePoint savePoint in savePoints)
        {
            if (gridPosition == savePoint.saveGridPos)  //세이브포인드가 나와 겹칠경우
            {
                spawnGridPos = gridPosition; //세이브포인트 저장
                return;
            }
        }
    }

}

