using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerGridMovement : MonoBehaviour
{
    public float moveDuration = 0.15f;  //�̵� �ӵ�
    public Tilemap wallTilemap;   // �� Ÿ�ϸ� ����
    public Tilemap groundTilemap;  // �ٴ� Ÿ�ϸ� ����
    public Tilemap[] doorTilemaps;  // �� Ÿ�ϸʵ� ����
    public float moveDelay = 0.15f;  // �̵� �����
    private bool isMoving = false;  // �̵� ���۰���
    private Vector2Int gridPosition;  //Ÿ�ϸ�󿡼��� ��ǥ
    public Vector2Int CurrentGridPosition => gridPosition;
    public Goal goal;  // ����
    public Enemy[] enemies;  // ����
    public Vector2Int spawnGridPos;  // ���� ����Ʈ ��ǥ
    public SavePoint[] savePoints;  // ���� ����Ʈ��
    public Switch[] switches; // ����ġ��

    void Start()  //���۽� ȣ��
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);  //���� ������ǥ�� �׸��� ��ǥ�� ����
        gridPosition = new Vector2Int(cellPos.x, cellPos.y);
        spawnGridPos = gridPosition;  //���� ����Ʈ�� ���� �׸�����ǥ�� ����
        enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);  // ���� ����
        transform.position = GridToWorld(gridPosition);  //���� ������ǥ�� �׸�����ǥ�� ����
    }



    void Update()  //�����Ӹ��� ȣ��
    {
        if (isMoving) return;  // �̵����̸� ����

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
    
    Vector3 GridToWorld(Vector2Int gridPos)  //�׸�����ǥ�� ������ǥ�� ����
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
    }
    
    void TryMove(Vector2Int dir)  //�̵� �õ�
    {
        Vector2Int targetGridPos = gridPosition + dir;  //���� ��ǥ + �̵� ����

        if (IsWall(targetGridPos))  //���� �̵� ��ǥ�� ���� ������� ����
            return;

        StartCoroutine(MoveRoutine(targetGridPos));  //�̵� ��ƾ ����
    }
    IEnumerator MoveRoutine(Vector2Int targetGridPos)  //�̵� ��ƾ
    {
        isMoving = true;  //�̵� ������

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

        CheckEnemy();  //�� Ȯ��
        CheckGoal();  //�� Ȯ��
        CheckSave();  //���̺�����Ʈ Ȯ��
        CheckSwitch();  //����ġ Ȯ��
        CheckColorZone();
    }
    void CheckColorZone()
    {
        ColorZone[] zones = FindObjectsOfType<ColorZone>();

        foreach (var zone in zones)
        {
            if (gridPosition == zone.gridPos)
            {
                ColorChangeManager.Instance.ChangeColorSmooth();
                return;
            }
        }
    }
    void CheckSwitch()
    {
        foreach (var sw in switches)
        {
            sw.CheckSwitch(gridPosition);
        }
    }
    void CheckGoal()  // �� ��ġ Ȯ��
    {
        if (gridPosition == goal.goalGridPos)  //���� ���� ��ĥ���
        {
            GoalUI.Instance.ShowGoal();  // �� ���ڸ� ǥ��
            StartCoroutine(GoNextStage());  // ���� ���������� �̵�
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
            Debug.Log("��� �������� Ŭ����!");
        }
    }
    void CheckEnemy()  //�� ��ġ Ȯ��
    {
        if (EnemyTilemapManager.Instance.IsEnemyAt(gridPosition))
        {
            Respawn();
        }
    }
    public void Respawn()  // ������
    {
        StopAllCoroutines(); //��� Coroutine �ߴ�
        isMoving = false;  //�̵� ���� ���

        gridPosition = spawnGridPos;  //���� �׸�����ǥ�� ������ǥ�� �̵�
        transform.position = GridToWorld(spawnGridPos);  //���� ������ǥ�� ������ǥ�� �̵�
    }
    void CheckSave()
    {
        foreach (SavePoint savePoint in savePoints)
        {
            if (gridPosition == savePoint.saveGridPos)  //���̺����ε尡 ���� ��ĥ���
            {
                spawnGridPos = gridPosition; //���̺�����Ʈ ����
                return;
            }
        }
    }

}

