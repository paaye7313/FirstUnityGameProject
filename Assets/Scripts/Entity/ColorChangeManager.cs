using UnityEngine;

public class ColorChangeManager : MonoBehaviour
{
    public static ColorChangeManager Instance;

    public ColorGroup[] groups;
    public float speed = 3f;

    bool changing;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!changing) return;

        foreach (var group in groups)
        {
            // SpriteRenderer
            foreach (var sr in group.spriteRenderers)
            {
                if (sr == null) continue;
                sr.color = Color.Lerp(
                    sr.color,
                    group.targetColor,
                    Time.deltaTime * speed
                );
            }

            // Tilemap
            foreach (var tm in group.tilemaps)
            {
                if (tm == null) continue;
                tm.color = Color.Lerp(
                    tm.color,
                    group.targetColor,
                    Time.deltaTime * speed
                );
            }
        }
    }

    public void ChangeColorSmooth()
    {
        changing = true;
    }
}
