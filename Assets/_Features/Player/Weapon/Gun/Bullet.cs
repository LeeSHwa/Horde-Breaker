using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Stats (damage, knockback) will be "injected" by Initialize()
    private float damage;
    private float knockback;
    private bool penetration; // Does this bullet penetrate?

    // [New] Variable to store the attack's source (the player)
    private Transform attackSource;

    // [NEW] Variable to store hit sound
    private AudioClip hitSound;

    // Bullet's own properties
    public float speed = 20f;
    public float lifetime = 2f; // Auto-deactivates after 2 seconds
    private float lifetimeTimer;

    // [Modified] Added 'Transform source' parameter to Initialize method
    // [MODIFIED] Added 'AudioClip sound' parameter
    public void Initialize(float dmg, float kb, bool pen, Transform source, AudioClip sound = null)
    {
        this.damage = dmg;
        this.knockback = kb;
        this.penetration = pen;
        this.attackSource = source; // Store the attacker's info
        this.lifetimeTimer = lifetime; // Reset lifetime
        this.hitSound = sound; // [NEW] Store the hit sound
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
                    // [Modified] Changed knockback calculation basis to 'attackSource' (the player)
                    if (attackSource == null)
                    {
                        // Prevents null reference if attackSource is not assigned
                        Debug.LogWarning("Bullet's attackSource is not set!");
                        return;
                    }

                    Vector2 knockbackDirection = (other.transform.position - attackSource.position).normalized;

                    // Prevents the direction vector from becoming (0,0) if player and enemy overlap
                    if (knockbackDirection == Vector2.zero)
                    {
                        // (Fallback) Knock them back in the bullet's traveling direction (transform.right)
                        knockbackDirection = transform.right;
                    }

                    enemyMove.ApplyKnockback(knockbackDirection, this.knockback, 0.1f);
                }

                // [NEW] Play Hit Sound
                if (hitSound != null)
                {
                    SoundManager.Instance.PlaySFX(hitSound, 0.1f);
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

        // (Optional) Reset attackSource to null to prevent errors
        // attackSource = null; 
        // -> Not recommended to reset in OnEnable, as Initialize overwrites it every time.
    }
}