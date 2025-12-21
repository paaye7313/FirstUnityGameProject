using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // 플레이어
    public float smoothSpeed = 10f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPos;
    }
}
