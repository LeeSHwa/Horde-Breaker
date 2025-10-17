using UnityEngine;

public class CameraBoundsController : MonoBehaviour
{
    [Header("Following Target")]
    public Transform playerTarget;

    [Header("Camera Settings")]
    public float smoothSpeed = 5f; 

    private Camera mainCamera;
    private Vector3 offset; // z offset to maintain camera distance

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
        // Don't move if there is no target.
        if (playerTarget == null)
        {
            return;
        }

        Vector3 targetPosition = playerTarget.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        Rect mapBounds = GameManager.Instance.mapBounds;

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float minX = mapBounds.xMin + camWidth;
        float maxX = mapBounds.xMax - camWidth;
        float minY = mapBounds.yMin + camHeight;
        float maxY = mapBounds.yMax - camHeight;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = targetPosition;
    }
}