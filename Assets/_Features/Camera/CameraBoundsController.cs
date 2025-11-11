using UnityEngine;

public class CameraBoundsController : MonoBehaviour
{
    [Header("Following Target")]
    public Transform playerTarget;

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;

    private Camera mainCamera;
    private Vector3 offset; // z offset to maintain camera distance

    // --- [Modified] Static screen bound variables that all projectiles can reference ---
    // Brought the contents of CameraBoundsManager.cs here.
    // These variables are accessed by RicochetProjectile.cs like CameraBoundsController.MinBounds.
    public static Vector2 MinBounds { get; private set; }
    public static Vector2 MaxBounds { get; private set; }
    // --- [Modified] ---

    void Start()
    {
        // Find the player GameObject using its tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }

        mainCamera = GetComponent<Camera>();
        offset = new Vector3(0, 0, transform.position.z);
    }

    // LateUpdate is called after all other updates, which is ideal for a camera to prevent jitter.
    void LateUpdate()
    {
        Vector3 targetPosition;

        // Don't move if there is no target.
        if (playerTarget == null)
        {
            // [Modified] Even if there is no target, the camera's current position is fixed,
            // so set targetPosition based on the current position to calculate the bounds.
            targetPosition = transform.position;
        }
        else
        {
            // [Modified] Logic for when a target exists
            targetPosition = playerTarget.position + offset;
            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // [Modified] In case GameManager isn't ready (optional but recommended)
            if (GameManager.Instance != null)
            {
                Rect mapBounds = GameManager.Instance.mapBounds;

                float camHeight = mainCamera.orthographicSize;
                float camWidth = camHeight * mainCamera.aspect;

                float minX = mapBounds.xMin + camWidth;
                float maxX = mapBounds.xMax - camWidth;
                float minY = mapBounds.yMin + camHeight;
                float maxY = mapBounds.yMax - camHeight;

                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            }
        }

        // 1. Set the camera's final position.
        transform.position = targetPosition;

        // 2. Calculate the screen boundaries *immediately after* the camera's final position is determined.
        //    This ensures the Projectile script can reference the accurate, updated boundary values in the next frame.
        MinBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        MaxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }
}