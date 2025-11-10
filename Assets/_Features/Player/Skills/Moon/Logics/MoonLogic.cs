using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
// [MODIFIED] Class name changed from RotatingLogic to MoonLogic
public class MoonLogic : MonoBehaviour
{
    private Transform centerPoint; // Rotation center (Player)
    private float damage;
    private float radius;          // Rotation radius (skillDistance)
    private float rotationSpeed;   // Rotation speed (speed)
    private float knockbackForce;
    private float currentAngle;

    // [New] Duration timer for pooling
    private float durationTimer;

    // Cooldown management to hit the same enemy multiple times (like the 'Bible')
    private Dictionary<Collider2D, float> hitCooldowns = new Dictionary<Collider2D, float>();
    private float hitCooldownDuration = 0.5f; // Can only hit the same enemy once per 0.5s
    private List<Collider2D> cooldownKeysToRemove = new List<Collider2D>();

    // [Modified] Changed original Setup to Initialize and added duration/knockback
    public void Initialize(float dmg, float duration, float speed, float radius, Transform center, float startAngle, float knockback)
    {
        this.damage = dmg;
        this.durationTimer = duration; // [New] Set duration
        this.rotationSpeed = speed;
        this.radius = radius;
        this.centerPoint = center;
        this.currentAngle = startAngle;
        this.knockbackForce = knockback;

        // [Removed] Original Destroy(gameObject, duration) line. Using pooling.
        // Destroy(gameObject, duration); 
    }

    // [New] Reset in OnEnable for pooling (see Bullet.cs)
    void OnEnable()
    {
        // Clear the hit enemy list for reuse
        hitCooldowns.Clear();
    }

    void Update()
    {
        // [New] Duration check (pool return logic)
        durationTimer -= Time.deltaTime;
        if (durationTimer <= 0)
        {
            gameObject.SetActive(false); // Return to pool instead of Destroy
            return;
        }

        if (centerPoint == null)
        {
            // [Modified] Return to pool instead of original Destroy(gameObject)
            gameObject.SetActive(false); // Self-destruct if player is gone
            return;
        }

        // 1. Rotation Logic
        currentAngle += rotationSpeed * Time.deltaTime;

        // 2. Position Calculation
        Vector2 offset = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * radius;

        transform.position = (Vector2)centerPoint.position + offset;

        // 3. Clean up the hit cooldown dictionary
        cooldownKeysToRemove.Clear();
        foreach (var pair in hitCooldowns)
        {
            // Remove if cooldown has expired, or if the enemy is null/disabled
            if (Time.time > pair.Value || pair.Key == null || !pair.Key.gameObject.activeInHierarchy)
            {
                cooldownKeysToRemove.Add(pair.Key);
            }
        }
        foreach (var key in cooldownKeysToRemove)
        {
            hitCooldowns.Remove(key);
        }
    }

    // Use both Enter and Stay to detect collisions reliably
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleHit(other);
    }

    private void HandleHit(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Can only hit if NOT in the cooldown dictionary (i.e., not hit recently)
        if (!hitCooldowns.ContainsKey(other))
        {
            // 1. Apply Damage
            // [Modified] Use StatsController instead of original CharacterStats
            if (other.TryGetComponent<StatsController>(out var stats))
            {
                stats.TakeDamage(damage);
            }
            // [Removed] Original CharacterStats logic
            // if (other.TryGetComponent<CharacterStats>(out var stats))
            // {
            //     stats.TakeDamage(damage);
            // }

            // 2. Apply Knockback (referencing Bullet.cs logic)
            // [Modified] Use teammate's EnemyMovement.cs function instead of Rigidbody.AddForce
            if (other.TryGetComponent<EnemyMovement>(out var enemyMove))
            {
                // Knockback direction = away from player (center)
                Vector2 knockbackDir = (other.transform.position - centerPoint.position).normalized;
                // [New] Call the dedicated knockback function in EnemyMovement
                enemyMove.ApplyKnockback(knockbackDir, knockbackForce, 0.1f); // 0.1s knockback
            }
            // [Removed] Original AddForce logic
            // if (other.TryGetComponent<Rigidbody2D>(out var enemyRb))
            // {
            //     // Knockback direction = away from player (center)
            //     Vector2 knockbackDir = (other.transform.position - centerPoint.position).normalized;
            //     enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            // }

            // 3. Register to cooldown
            hitCooldowns[other] = Time.time + hitCooldownDuration;
        }
    }
}