using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // 플레이어
    public float smoothSpeed = 10f;  //부드러운 움직임
    public Vector3 offset;  // 카메라가 플레이어를 어떤 위치에서 따라볼 것인지

    void LateUpdate()
    {
        if (target == null) return;  // 플레이어가 없으면 리턴

        Vector3 desiredPos = target.position + offset;  // 기본 위치 설정
        // 부드럽게 움직이는 위치 설정
        Vector3 smoothedPos = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );
        // 월드좌표를 목표 좌표로 설정
        transform.position = smoothedPos;
    }
}
