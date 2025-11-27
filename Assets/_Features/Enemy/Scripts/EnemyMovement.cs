using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))] // Necessary Component
public class EnemyMovement : MonoBehaviour
{
    [Header("Weight")] // Name to Display in Inspector Tab
    public float mass = 0.5f; // Weight of the enemy

    [Header("Optimization")]
    public float despawnDistance = 40f; // Distance to disable/return enemy

    public bool isReverseSprite = false;

    private StatsController stats; // Load CharacterStats script

    private Transform player; // Load the player's Transform component
    private Rigidbody2D rb; // Physics Engine(Rigidbody2D)

    private bool canMove = true;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Load Physics Engine(Rigidbody2D)
        stats = GetComponent<StatsController>(); // Get the CharacterStats component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // [NEW] Reset state when respawning from the pool
    void OnEnable()
    {
        canMove = true; // Ensure movement is enabled

        // Reset Physics
        if (rb != null)
        {
            rb.mass = mass;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero; // Stop previous momentum
        }

        // Find Player again (in case player object changed or for safety)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        // 1. Optimization: Return to pool if too far from player
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance > despawnDistance)
            {
                // Return to pool instead of Destroy
                if (EnemySpawnerTemp.Instance != null)
                {
                    EnemySpawnerTemp.Instance.ReturnEnemy(this.gameObject);
                }
                else
                {
                    gameObject.SetActive(false); // Fallback
                }
                return; // Stop execution
            }
        }

        // 2. Movement Logic
        if (canMove)
        {
            if (player == null) { rb.linearVelocity = Vector3.zero; return; }

            Vector2 direction = (player.position - transform.position).normalized;

            // Move using Physics
            rb.linearVelocity = direction * stats.currentMoveSpeed;

            // Sprite Flip Logic
            if (direction.x > 0)
            {
                spriteRenderer.flipX = isReverseSprite;
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = !isReverseSprite;
            }
        }
    }

    // A public function that can be called by other scripts (like MeleeWeapon).
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // Stop the coroutine if it's already running to reset the knockback.
        StopAllCoroutines();
        // Check if object is active before starting coroutine (prevents errors)
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockbackCoroutine(direction, force, duration));
        }
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