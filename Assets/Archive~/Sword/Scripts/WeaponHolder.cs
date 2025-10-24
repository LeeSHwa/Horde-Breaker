using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform playerTransform;

    public Camera mainCam;

    [Header("Sprite Correction")]
    [Tooltip("Adjust this value to match your weapon sprite's initial orientation. If it points up, try -90.")]
    public float rotationOffset = 0f;

    private Plane gamePlane;

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

            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }
    }
}

