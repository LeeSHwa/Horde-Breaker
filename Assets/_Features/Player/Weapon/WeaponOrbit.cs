using UnityEngine;

public class WeaponOrbit : MonoBehaviour
{
    [Tooltip("The player object that will be the center of the orbit")]
    public Transform playerTransform;

    [Tooltip("The distance (radius) between the player and the weapon")]
    [Range(0.5f, 10.0f)]
    public float orbitRadius = 2.0f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Safety check
        if (playerTransform == null || mainCamera == null)
        {
            return;
        }

        // Convert the player's 3D world position to a 2D screen position that we see
        Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(playerTransform.position);

        // Calculate the direction vector by subtracting the player's screen position from the mouse's current screen position
        Vector3 direction = Input.mousePosition - playerScreenPosition;

        // Since a 2D game only uses X and Y, set the Z-axis to 0. Use .normalized for the direction vector
        Vector3 worldOffset = new Vector3(direction.x, direction.y, 0).normalized;

        // Update the final position (Player Position + Direction Vector * Desired Distance)
        transform.position = playerTransform.position + worldOffset * orbitRadius;

        // Calculate the angle from the direction vector (x, y)
        float angle = Mathf.Atan2(worldOffset.y, worldOffset.x) * Mathf.Rad2Deg;

        // Convert the angle to a real rotation value using Quaternion.Euler. Rotate only on the Z-axis by 'angle' (since it's a 2D game)
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}