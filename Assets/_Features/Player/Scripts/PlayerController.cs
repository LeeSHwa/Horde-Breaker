using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(StatsController))]
public class PlayerController : MonoBehaviour
{
    // Rigidbody component for physics-based movement
    private Rigidbody2D rb;

    // Variable to store the direction of input
    private Vector2 moveInput;

    private Camera mainCamera;

    private StatsController stats;

    public Vector2 AimDirection { get; private set; }

    void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<StatsController>(); // Get the CharacterStats component

        mainCamera = Camera.main;

        AimDirection = Vector2.right; // Default aim direction
    }


    void Update()
    {
        // 1. Get keyboard input
        // GetAxisRaw returns -1, 0, or 1 for immediate response
        float moveX = Input.GetAxisRaw("Horizontal"); // Horizontal input (left/right)
        float moveY = Input.GetAxisRaw("Vertical");   // Vertical input (up/down)

        // 2. Store the input as a Vector2 and normalize it to ensure consistent speed diagonally
        moveInput = new Vector2(moveX, moveY).normalized;

        UpdateAimDirection();
    }

    private void FixedUpdate()
    {
        // 3. Move the player physically using Rigidbody2D
        // This should be done in FixedUpdate for physics consistency
        //rb.MovePosition(rb.position + moveInput * stats.moveSpeed * Time.fixedDeltaTime); // chaged for Animation
        rb.linearVelocity = moveInput * stats.currentMoveSpeed;
    }


    private void UpdateAimDirection()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

    Vector2 direction = new Vector2(
        mousePosition.x - transform.position.x,
        mousePosition.y - transform.position.y
        ).normalized;

    AimDirection = direction;
    }
}


