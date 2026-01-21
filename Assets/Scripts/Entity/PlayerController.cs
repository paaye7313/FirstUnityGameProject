using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerGridMovement : MonoBehaviour
{
    public float moveDuration = 0.15f;  //�̵� �ӵ�
    public Tilemap wallTilemap;   // 벽 타일 맵
    public Tilemap groundTilemap;  // 바닥 타일 맵
    public Tilemap[] doorTilemaps;  // 문 타일 맵
    public float moveDelay = 0.15f;  // 이동 딜레이
    private bool isMoving = false;  // 이동중 확인
    private Vector2Int gridPosition;  // 그리드 좌표
    public Vector2Int CurrentGridPosition => gridPosition;
    public Goal goal;  // 골
    public Enemy[] enemies;  // 적 들
    public Vector2Int spawnGridPos;  // 스폰 포인트
    public SavePoint[] savePoints;  // 세이브 포인트 객체
    public Switch[] switches; // 스위치 객체
    public ColorZone[] zones; // 색변경 객체

    void Start()  //시작시 실행
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);  //���� ������ǥ�� �׸��� ��ǥ�� ����
        gridPosition = new Vector2Int(cellPos.x, cellPos.y);
        spawnGridPos = gridPosition;  //���� ����Ʈ�� ���� �׸�����ǥ�� ����
        enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);  // ���� ����
        zones = Object.FindObjectsByType<ColorZone>(FindObjectsSortMode.None);
        transform.position = GridToWorld(gridPosition);  //���� ������ǥ�� �׸�����ǥ�� ����
    }



    void Update()  //스텝
    {
        if (isMoving) return;  // 이동중이라면 리턴

        Vector2Int inputDir = Vector2Int.zero;  //�̵� ���� �ʱ�ȭ
        // ����Ű�� �´� �̵����� ����
        if (Input.GetKey(KeyCode.W)) inputDir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S)) inputDir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A)) inputDir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D)) inputDir = Vector2Int.right;

        if (inputDir != Vector2Int.zero)  //�̵������� �����Ǿ��� ���
        {
            TryMove(inputDir);  //�̵� �õ�
        }
    }

    bool IsWall(Vector2Int gridPos)  //���� �ִ��� Ȯ��
    {
        Vector3Int cellPos = new Vector3Int(gridPos.x, gridPos.y, 0);  // ��ǥ�� 3���� ��ǥ�� ����ȯ
        if (wallTilemap.HasTile(cellPos)) return true;  // ���� ���� ��� Ʈ��
        foreach (var doorMap in doorTilemaps)
        {
            if (doorMap != null && doorMap.HasTile(cellPos))
                return true;
        }  // ���� ���� ��� Ʈ��

        return false;
    }
    
    Vector3 GridToWorld(Vector2Int gridPos)  //그리드 좌표를 월드 좌표로 변환
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
    }
    
    void TryMove(Vector2Int dir)  //이동 시도
    {
        Vector2Int targetGridPos = gridPosition + dir;  //���� ��ǥ + �̵� ����

        if (IsWall(targetGridPos))  //���� �̵� ��ǥ�� ���� ������� ����
            return;

        StartCoroutine(MoveRoutine(targetGridPos));  //�̵� ��ƾ ����
    }
    IEnumerator MoveRoutine(Vector2Int targetGridPos)  //이동 루틴
    {
        isMoving = true;  //이동중 표시

        Vector3 startPos = transform.position;  //������ǥ�� ������ǥ
        Vector3 targetPos = GridToWorld(targetGridPos);  //����ǥ�� ��ǥ��ǥ

        float elapsed = 0f;  //�̵� ���൵

        while (elapsed < moveDelay)  //�̵� ���൵�� �̵� ����̿� �����Ҷ�����
        {
            elapsed += Time.deltaTime; //�̵� ���൵ +1
            float t = elapsed / moveDelay;  //���൵ ����
            transform.position = Vector3.Lerp(startPos, targetPos, t);  // ���൵ ���ؿ� ���� ��ġ����
            yield return null;  //���� ���������� �Ѿ
        }

        transform.position = targetPos;  //���� ������ǥ�� ��ǥ ��ǥ�� ����
        gridPosition = targetGridPos;  //���� �׸�����ǥ�� ��ǥ ��ǥ�� ����
        isMoving = false;  //�̵� ���� ��

        CheckEnemy();  //적 확인
        CheckGoal();  //골 확인
        CheckSave();  //세이브 포인트 확인
        CheckSwitch();  //스위치 확인
        CheckColorZone();  //색변경 확인
    }
    void CheckColorZone()
    {
        foreach (var zone in zones)
        {
            if (gridPosition == zone.gridPos)
            {
                zone.ccm.ChangeColorSmooth();
                //ColorChangeManager.Instance.ChangeColorSmooth();
                return;
            }
        }
    }
    void CheckSwitch()  // 스위치 확인
    {
        foreach (var sw in switches)
        {
            sw.CheckSwitch(gridPosition);
        }
    }
    void CheckGoal()  // 골 확인
    {
        if (gridPosition == goal.goalGridPos)  //골에 도달한 경우
        {
            GoalUI.Instance.ShowGoal();  // 골 UI 표시
            StartCoroutine(GoNextStage());  // 다음 스테이지로
        }
    }
    IEnumerator GoNextStage() // 다음 스테이지로 이동
    {
        yield return new WaitForSeconds(2f);  // 2초 기다림

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1; // 현재 스테이지 에서 1더함

        if (nextIndex < SceneManager.sceneCountInBuildSettings)  //다음 스테이지가 있는 경우
        {
            SceneManager.LoadScene(nextIndex);  // 씬 불러오기
        }
        else  // 없으면
        {
            Debug.Log("마지막 스테이지 입니다!");
        }
    }
    void CheckEnemy()  // 적 확인
    {
        if (EnemyTilemapManager.Instance.IsEnemyAt(gridPosition))
        {
            Respawn();
        }
    }
    public void Respawn()  // 리스폰
    {
        StopAllCoroutines(); //모든 Coroutine 정지
        isMoving = false;  //움직임 해제

        gridPosition = spawnGridPos;  //스폰 포인트로 이동
        transform.position = GridToWorld(spawnGridPos);  //월드그리드좌표 실제로 이동
    }
    void CheckSave()
    {
        foreach (SavePoint savePoint in savePoints)
        {
            if (gridPosition == savePoint.saveGridPos)  //세이브 포인트와 겹칠 경우
            {
                spawnGridPos = gridPosition; //스폰 포인트 갱신
                return;
            }
        }
    }

}

