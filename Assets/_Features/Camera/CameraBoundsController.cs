using UnityEngine;

public class CameraBoundsController : MonoBehaviour
{
    [Header("Camera Boundaries")]
    public float minX; // The minimum X position for the camera.
    public float maxX; // The maximum X position for the camera.
    public float minY; // The minimum Y position for the camera.
    public float maxY; // The maximum Y position for the camera.

    // The target the camera will follow (assigned automatically if tagged "Player").
    private Transform playerTarget;

    // Stores the initial Z-axis offset of the camera.
    private Vector3 offset;

    void Start()
    {
        // Find the player GameObject using its tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }

        // Maintain the original Z position.
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

        // Use Mathf.Clamp to restrict the camera's position within the boundaries.
        float clampedX = Mathf.Clamp(playerTarget.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(playerTarget.position.y, minY, maxY);

        // Apply the new, clamped position while keeping the original Z offset.
        transform.position = new Vector3(clampedX, clampedY, offset.z);
    }

    // Draws a yellow box in the Scene view to visualize the camera boundaries.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}