using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2Int gridPos;

    public void Init(Vector2Int pos)
    {
        gridPos = pos;
        transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerGridMovement player = other.GetComponent<PlayerGridMovement>();
            if (player != null)
            {
                player.Respawn();
            }
        }
    }
}
