using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerGridMovement : MonoBehaviour
{
    public float moveDuration = 0.15f;  //이동 속도
    public Tilemap wallTilemap;   // 벽 타일맵 참조
    public Tilemap groundTilemap;  // 바닥 타일맵 참조
    public Tilemap[] doorTilemaps;  // 문 타일맵들 참조
    public float moveDelay = 0.15f;  // 이동 딜레이
    private bool isMoving = false;  // 이동 조작감지
    private Vector2Int gridPosition;  //타일멥상에서의 좌표
    public Vector2Int CurrentGridPosition => gridPosition;
    public Goal goal;  // 골인
    public Enemy[] enemies;  // 적들
    public Vector2Int spawnGridPos;  // 스폰 포인트 좌표
    public SavePoint[] savePoints;  // 스폰 포인트들
    public Switch[] switches; // 스위치들

    void Start()  //시작시 호출
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);  //현재 월드좌표를 그리드 좌표로 보정
        gridPosition = new Vector2Int(cellPos.x, cellPos.y);
        spawnGridPos = gridPosition;  //스폰 포인트를 현재 그리드좌표로 설정
        enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);  // 적들 감지
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
        if (wallTilemap.HasTile(cellPos)) return true;  // 벽이 있을 경우 트루
        foreach (var doorMap in doorTilemaps)
        {
            if (doorMap != null && doorMap.HasTile(cellPos))
                return true;
        }  // 문이 있을 경우 트루

        return false;
    }
    
    Vector3 GridToWorld(Vector2Int gridPos)  //그리드좌표를 월드좌표로 보정
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
    }
    
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
        CheckSwitch();  //스위치 확인
    }
    void CheckSwitch()
    {
        foreach (var sw in switches)
        {
            sw.CheckSwitch(gridPosition);
        }
    }
    void CheckGoal()  // 골 위치 확인
    {
        if (gridPosition == goal.goalGridPos)  //골이 나와 겹칠경우
        {
            GoalUI.Instance.ShowGoal();  // 골 글자를 표시
            StartCoroutine(GoNextStage());  // 다음 스테이지로 이동
        }
    }
    IEnumerator GoNextStage()
    {
        yield return new WaitForSeconds(2f);

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("모든 스테이지 클리어!");
        }
    }
    void CheckEnemy()  //적 위치 확인
    {
        if (EnemyTilemapManager.Instance.IsEnemyAt(gridPosition))
        {
            Respawn();
        }
    }
    public void Respawn()  // 리스폰
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

