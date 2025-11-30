using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Stats injected by Initialize()
    private float damage;
    private float knockback;

    // "penetration" means Infinite Penetration (Level 9)
    private bool penetration;

    // Finite penetration count
    private int currentPenetrationCount;

    private bool isCritical;
    private Transform attackSource;
    private AudioClip hitSound;

    public float speed = 20f;
    public float lifetime = 2f;
    private float lifetimeTimer;

    public void Initialize(float dmg, float kb, bool isInfinitePen, int pierceCount, Transform source, Vector3 scale, float newLifetime, AudioClip sound = null, bool isCrit = false)
    {
        this.damage = dmg;
        this.knockback = kb;
        this.penetration = isInfinitePen;
        this.currentPenetrationCount = pierceCount;
        this.attackSource = source;

        transform.localScale = scale;

        this.lifetime = newLifetime;
        this.lifetimeTimer = this.lifetime;

        this.hitSound = sound;
        this.isCritical = isCrit;
    }

    void Update()
    {
        // If lifetime expires, deactivate instead of destroying.
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0)
        {
            gameObject.SetActive(false);
        }

        // Logic for moving forward
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatsController enemyStats = other.GetComponent<StatsController>();
            if (enemyStats != null)
            {
                // Attack with the injected damage & crit status.
                enemyStats.TakeDamage(damage, isCritical);

                EnemyMovement enemyMove = other.GetComponent<EnemyMovement>();
                if (enemyMove != null)
                {
                    if (attackSource == null)
                    {
                        Debug.LogWarning("Bullet's attackSource is not set!");
                        return;
                    }

                    Vector2 knockbackDirection = (other.transform.position - attackSource.position).normalized;

                    if (knockbackDirection == Vector2.zero)
                    {
                        knockbackDirection = transform.right;
                    }

                    enemyMove.ApplyKnockback(knockbackDirection, this.knockback, 0.1f);
                }

                if (hitSound != null)
                {
                    SoundManager.Instance.PlaySFX(hitSound, 0.1f);
                }
            }

            // Penetration Logic
            // 1. If Infinite Penetration is active (Lv 9), do nothing (keep flying).
            if (penetration) return;

            // 2. If we have Pierce Count left, decrement and keep flying.
            if (currentPenetrationCount > 0)
            {
                currentPenetrationCount--;
                return;
            }

            // 3. Otherwise, destroy the bullet.
            gameObject.SetActive(false);
        }
    }

    // Called when the object is re-enabled from the pool.
    void OnEnable()
    {
        // Reset the lifetime timer every time it's fired.
        lifetimeTimer = lifetime;
    }
}