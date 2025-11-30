using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float damage;
    private float moveSpeed;
    private float acceleration; // Acceleration value
    private float lifetime;

    // Flag to check if initialized to prevent accidental zero damage
    private bool isInitialized = false;

    // Initialize the projectile with specific stats from the Boss Skill script
    public void Initialize(float dmg, float speed, float accel, float duration, Vector3 direction)
    {
        this.damage = dmg;
        this.moveSpeed = speed;
        this.acceleration = accel; // Set acceleration
        this.lifetime = duration;

        // Rotate projectile to face the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        // Increase speed over time based on acceleration
        moveSpeed += acceleration * Time.deltaTime;

        // Move forward (Right direction relative to rotation)
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

        // Handle Lifetime
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            DisableProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the target is the Player
        if (other.CompareTag("Player"))
        {
            StatsController playerStats = other.GetComponent<StatsController>();
            if (playerStats != null)
            {
                // Apply damage to the player
                playerStats.TakeDamage(damage);
            }

            // Disable projectile on hit
            DisableProjectile();
        }
        //else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        //{
        //    // Optional: Destroy on wall hit
        //    DisableProjectile();
        //}
    }

    private void DisableProjectile()
    {
        // If using Object Pooling, return to pool (implementation depends on your PoolManager)
        // For now, we just set inactive.
        gameObject.SetActive(false);
    }

    // Reset state when reused from pool
    void OnEnable()
    {
        isInitialized = false;
    }
}