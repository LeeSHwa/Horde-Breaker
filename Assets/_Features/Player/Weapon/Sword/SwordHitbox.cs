using UnityEngine;

// This script controls the behavior of the sword's hitbox.
// It is managed by the Sword.cs coroutine for position and rotation.
public class SwordHitbox : MonoBehaviour
{
    // The total time the slash hitbox remains active before disappearing.
    [SerializeField]
    private float slashDuration = 0.3f;

    // A countdown timer to track the active duration.
    private float timer;

    // Stores the damage value passed from Sword.cs.
    private float currentDamage;
    private float currentKnockback;

    private Transform attackSource;

    // Called automatically by the PoolManager when this object is activated.
    void OnEnable()
    {
        // Reset the timer every time the hitbox is spawned.
        timer = slashDuration;
    }

    // Public method called by Sword.cs right after spawning.
    // It passes in the weapon's current stats.
    public void Initialize(float damage, float knockback, float area, Quaternion rotation, Transform source)
    {
        this.currentDamage = damage;
        this.currentKnockback = knockback;
        this.attackSource = source; 

        // Note: The scale is set in the Sword.cs coroutine,
        // but this could be a way to set an initial size if needed.
        // transform.localScale = Vector3.one * area; 

        // Note: The rotation is also set in the Sword.cs coroutine.
        // transform.rotation = rotation;
    }

    // Update is called once per frame.
    void Update()
    {
        // Count down the timer.
        timer -= Time.deltaTime;

        // When the timer runs out, deactivate the object (return it to the pool).
        if (timer <= 0f)
        {
            // We use SetActive(false) for object pooling, not Destroy().
            gameObject.SetActive(false);
        }

        // All movement and rotation logic has been removed from Update.
        // The Sword.cs 'SwingSwordHitbox' coroutine now handles all transform changes.
    }

    // Called when this hitbox's collider (set as Trigger) overlaps another collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Log any collision for debugging.
        // Debug.Log("Sword collided with: " + collision.tag);

        // Check if the collided object has the "Enemy" tag.
        if (collision.CompareTag("Enemy"))
        {
            // Debug.Log("Hit an Enemy: " + collision.name);

            // Try to find the StatsController component on the enemy.
            if (collision.TryGetComponent<StatsController>(out StatsController enemyStats))
            {
                // If found, apply damage.
                // Debug.Log("Dealing " + currentDamage + " damage to " + collision.name);
                enemyStats.TakeDamage(currentDamage);

                EnemyMovement enemyMove = collision.GetComponent<EnemyMovement>();
                if (enemyMove != null)
                {
                    if (attackSource == null)
                    {
                        Debug.LogWarning("SwordHitbox's attackSource is not set!");
                        return;
                    }

                    Vector2 knockbackDirection = (collision.transform.position - attackSource.position).normalized;

                    if (knockbackDirection == Vector2.zero)
                    {
                        knockbackDirection = (collision.transform.position - transform.position).normalized;
                    }

                    enemyMove.ApplyKnockback(knockbackDirection, this.currentKnockback, 0.15f);
                }
            }
            else
            {
                // Log an error if the enemy is missing the required script.
                Debug.LogError("Could not find StatsController on " + collision.name);
            }
        }
    }
}