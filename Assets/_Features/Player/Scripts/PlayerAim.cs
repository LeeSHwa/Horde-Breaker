using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Camera mainCam; // Variable to hold the main camera
    private Vector2 mousePosition; // Variable to hold the mouse position

    // Executed once when the game starts.
    void Start()
    {
        // For performance, we cache the main camera once at the start.
        mainCam = Camera.main;
    }

    // Executed every frame.
    void Update()
    {
        // 1. Get the mouse position from Input.
        mousePosition = Input.mousePosition;

        // 2. Convert the mouse position from screen coordinates to world coordinates.
        Vector2 worldPoint = mainCam.ScreenToWorldPoint(mousePosition);

        // 3. Calculate the direction from the player's position to the mouse's position.
        // Subtract the player's position from the mouse's world position to get the direction vector.
        // .normalized makes its length 1, so we only get the direction.
        Vector2 direction = (worldPoint - (Vector2)transform.position).normalized;

        // 4. Convert the direction vector to an angle.
        // Use Atan2 to get the angle in radians from the direction's y and x values,
        // then multiply by Rad2Deg to convert it to degrees.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. Apply the angle to the object's Z-axis rotation.
        // Quaternion.Euler creates a rotation around the Z-axis by the amount of 'angle'.
        //transform.rotation = Quaternion.Euler(0, 0, angle); // Delete for PlayerAnimator
    }
}