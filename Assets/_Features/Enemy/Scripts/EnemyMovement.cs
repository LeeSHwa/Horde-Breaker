using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))] // Necessary Component
[RequireComponent(typeof(CircleCollider2D))] // Necessary Component
public class EnemyMovement : MonoBehaviour
{
    [Header("Weight")] // Name to Display in Inspector Tab
    public float mass = 0.5f; // Weight of the enemy

    private StatsController stats; // Load CharacterStats script

    private Transform player; // Load the player's Transform component
    private Rigidbody2D rb; // Physics Engine(Rigidbody2D)

    private bool canMove = true;

    // --- [Modified] 1. Add SpriteRenderer variable ---
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Load Physics Engine(Rigidbody2D)
        rb.mass = mass; // Set the weight of the enemy(from Hearder "Weight")
        rb.gravityScale = 0; // Top Down 2D game(no gravity)

        stats = GetComponent<StatsController>(); // Get the CharacterStats component

        // --- [Modified] 2. Get the SpriteRenderer component ---
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Finding "player" tag
        if (playerObject != null)
        {
            player = playerObject.transform; // Load the player's Transform component
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (player == null)
            {
                rb.linearVelocity = Vector3.zero;
                return; // If no player survives on the map, it will stop.
            }

            Vector2 direction = (player.position - transform.position).normalized; // Get the direction from enemy to player
            rb.linearVelocity = direction * stats.currentMoveSpeed; // Move the enemy towards the player

            // --- [Modified] 3. Sprite flipping logic ---
            // (Assumption: Original sprite is facing right)
            if (direction.x > 0)
            {
                // When moving right (x is positive)
                spriteRenderer.flipX = false; // Don't flip (original direction)
            }
            else if (direction.x < 0)
            {
                // When moving left (x is negative)
                spriteRenderer.flipX = true; // Flip horizontally
            }
        }
    }
    // --- KNOCKBACK FIX ---
    // A public function that can be called by other scripts (like MeleeWeapon).

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // Stop the coroutine if it's already running to reset the knockback.
        StopAllCoroutines();
        StartCoroutine(KnockbackCoroutine(direction, force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force, float duration)
    {
        // 1. Disable AI movement.
        canMove = false;

        // 2. Apply the knockback force.
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // 3. Wait for the knockback duration.
        yield return new WaitForSeconds(duration);

        // 4. Re-enable AI movement.
        canMove = true;
    }
}