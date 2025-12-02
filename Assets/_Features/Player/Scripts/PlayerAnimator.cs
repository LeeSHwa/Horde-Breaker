using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Camera mainCam;

    // --- [NEWLY ADDED PART] ---
    // Flag to check if facing direction is locked (e.g., by an attack)
    private bool isFacingLocked = false;
    // The direction to be locked to
    private Vector2 lockedDirection;
    // --- [END OF ADDED PART] ---

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector2 direction;

        // --- [MODIFIED PART] ---
        // 1. Check if facing direction is locked
        if (isFacingLocked)
        {
            direction = lockedDirection; // Use the locked direction
        }
        else
        {
            // 2. If not locked, follow the mouse (original logic)
            Vector2 mousePosition = Input.mousePosition;
            Vector2 worldPoint = mainCam.ScreenToWorldPoint(mousePosition);
            direction = (worldPoint - (Vector2)transform.position).normalized;
        }
        // --- [END OF MODIFIED PART] ---

        // The rest of the logic uses 'direction' (original logic)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > -45 && angle <= 45)
        {
            animator.SetFloat("moveX", 1f);
            animator.SetFloat("moveY", 0f);
            spriteRenderer.flipX = false;
        }
        else if (angle > 45 && angle <= 135)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 1f);
        }
        else if (angle > 135 || angle <= -135)
        {
            animator.SetFloat("moveX", -1f);
            animator.SetFloat("moveY", 0f);
            spriteRenderer.flipX = true;
        }
        else if (angle > -135 && angle <= -45)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", -1f);
        }

        if (rb.linearVelocity.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    // --- [NEWLY ADDED FUNCTIONS] ---
    // These will be called by Sword.cs

    /// <summary>
    /// Locks the player's facing direction for animations.
    /// </summary>
    /// <param name="direction">The direction to lock to.</param>
    public void LockFacing(Vector2 direction)
    {
        isFacingLocked = true;
        lockedDirection = direction;
    }

    /// <summary>
    /// Unlocks the player's facing direction, allowing it to follow the mouse again.
    /// </summary>
    public void UnlockFacing()
    {
        isFacingLocked = false;
    }
    // --- [END OF ADDED FUNCTIONS] ---
}