using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // The player object that this holder will follow. // 이 홀더가 따라다닐 플레이어 오브젝트.
    public Transform playerTransform;

    // The main camera to calculate the mouse position in the world. // 월드 좌표상 마우스 위치를 계산하기 위한 메인 카메라.
    public Camera mainCam;

    [Header("Sprite Correction")] // 스프라이트 보정
    [Tooltip("Adjust this value to match your weapon sprite's initial orientation. If it points up, try -90.")]
    // 무기 스프라이트의 초기 방향에 맞게 이 값을 조절하세요. 위를 본다면 -90을 시도해보세요.
    public float rotationOffset = 0f;

    private Plane gamePlane; // A virtual plane to calculate the precise mouse position. // 정확한 마우스 위치 계산을 위한 가상 평면.

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        gamePlane = new Plane(Vector3.forward, 0);
    }

    void Update()
    {
        if (playerTransform == null) return;

        transform.position = playerTransform.position;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        if (gamePlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector2 direction = ((Vector2)worldPoint - (Vector2)transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Apply the final angle with the offset. // 보정값을 더한 최종 각도를 적용.
            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }
    }
}

