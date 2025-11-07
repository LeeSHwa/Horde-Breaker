using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Stats (damage, knockback) will be "injected" by Initialize()
    private float damage;
    private float knockback;
    private bool penetration; // Does this bullet penetrate?

    // Bullet's own properties
    public float speed = 20f;
    public float lifetime = 2f; // Auto-deactivates after 2 seconds
    private float lifetimeTimer;

    // (1) [Core API] Initialization method called by Gun.cs when firing.
    public void Initialize(float dmg, float kb, bool pen)
    {
        this.damage = dmg;
        this.knockback = kb;
        this.penetration = pen;
        this.lifetimeTimer = lifetime; // Reset lifetime
    }

    void Update()
    {
        // (2) If lifetime expires, deactivate instead of destroying.
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0)
        {
            gameObject.SetActive(false); // Changed from Destroy(gameObject)
        }

        // (Note) Logic for moving forward (assuming bullet faces right by default)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatsController enemyStats = other.GetComponent<StatsController>();
            if (enemyStats != null)
            {
                // (3) Attack with the injected damage.
                enemyStats.TakeDamage(damage);
                EnemyMovement enemyMove = other.GetComponent<EnemyMovement>();
                if (enemyMove != null)
                {                  
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                    enemyMove.ApplyKnockback(knockbackDirection, this.knockback, 0.1f);
                }
            }

            // (4) If this bullet does not penetrate, deactivate self on hit.
            if (!penetration)
            {
                gameObject.SetActive(false); // Changed from Destroy(gameObject)
            }
        }
    }

    // (5) [Important] Called when the object is re-enabled from the pool.
    void OnEnable()
    {
        // Reset the lifetime timer every time it's fired.
        lifetimeTimer = lifetime;
    }
}