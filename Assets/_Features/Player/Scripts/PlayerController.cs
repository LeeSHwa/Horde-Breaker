using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStats))]
public class PlayerController : MonoBehaviour
{
    // Rigidbody component for physics-based movement
    private Rigidbody2D rb;

    // Variable to store the direction of input
    private Vector2 moveInput;

    private CharacterStats stats; // Reference to CharacterStats script

    void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>(); // Get the CharacterStats component
    }

    // Update is called once per frame
    void Update()
    {
        // 1. Get keyboard input
        // GetAxisRaw returns -1, 0, or 1 for immediate response
        float moveX = Input.GetAxisRaw("Horizontal"); // Horizontal input (left/right)
        float moveY = Input.GetAxisRaw("Vertical");   // Vertical input (up/down)

        // 2. Store the input as a Vector2 and normalize it to ensure consistent speed diagonally
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        // 3. Move the player physically using Rigidbody2D
        // This should be done in FixedUpdate for physics consistency
        rb.MovePosition(rb.position + moveInput * stats.moveSpeed * Time.fixedDeltaTime);
    }
}