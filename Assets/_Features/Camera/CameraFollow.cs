using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라다닐 대상 (플레이어)
    public float smoothSpeed = 0.125f; // 카메라 이동 속도
    public Vector3 offset; // 플레이어로부터 떨어질 거리

    // 플레이어의 움직임이 모두 끝난 후에 카메라를 이동시키기 위해 LateUpdate를 사용합니다.
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}