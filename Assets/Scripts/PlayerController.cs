using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement speed
    public float moveSpeed = 5f;

    // Rigidbody component for physics-based movement
    private Rigidbody2D rb;

    // Variable to store the direction of input
    private Vector2 moveInput;

    void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1. Get keyboard input
        float moveX = Input.GetAxisRaw("Horizontal"); // Horizontal input (left/right)
        float moveY = Input.GetAxisRaw("Vertical");   // Vertical input (up/down)

        // 2. Store the input as a Vector2 and normalize it
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        // 3. Move the player by setting the velocity
        // This should be done in FixedUpdate for physics consistency

        // --- THIS IS THE CHANGED LINE ---
        rb.velocity = moveInput * moveSpeed;

        // The old method is commented out below for comparison
        // rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}