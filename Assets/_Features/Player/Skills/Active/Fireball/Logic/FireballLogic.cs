using UnityEngine;
using System.Collections.Generic;

// Logic for the fireball projectile itself
public class FireballLogic : MonoBehaviour
{
    // Stats injected by FireballSkill.cs
    private float damage;
    private float speed;
    private float lifetime;
    private float falloffPercentage;

    private float lifetimeTimer;

    // [NEW] Variable to store hit sound
    private AudioClip hitSound;

    // [NEW] List to track which enemies we've already hit
    private List<Collider2D> hitEnemies;

    // Called by the PoolManager when recycled
    void OnEnable()
    {
        lifetimeTimer = lifetime;

        // Initialize list if it's null (first time)
        if (hitEnemies == null)
        {
            hitEnemies = new List<Collider2D>();
        }
        // Clear the list for this new shot
        hitEnemies.Clear();
    }

    // Called by FireballSkill.cs when spawned
    // [MODIFIED] Added 'AudioClip sound' parameter
    public void Initialize(float dmg, float spd, float life, float falloff, float area, AudioClip sound = null)
    {
        this.damage = dmg;
        this.speed = spd;
        this.lifetime = life;
        this.falloffPercentage = falloff;
        this.hitSound = sound; // [NEW] Store hit sound
        this.lifetimeTimer = life;

        // Apply area (size) increase
        transform.localScale = Vector3.one * area;
    }

    void Update()
    {
        // 1. Move forward (assuming prefab faces right)
        // We use transform.right which is the (X) axis in local space.
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 2. Lifetime check
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0)
        {
            gameObject.SetActive(false); // Return to pool
        }
    }

    // The core logic for penetration and damage falloff
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // [NEW] Check if we have already hit this enemy
        if (hitEnemies.Contains(other))
        {
            return; // Already hit, do nothing
        }

        if (other.TryGetComponent<StatsController>(out var stats))
        {
            float damageToDeal = 0;

            // [NEW] Apply damage falloff logic
            if (hitEnemies.Count == 0)
            {
                // First hit: 100% damage
                damageToDeal = this.damage;
            }
            else
            {
                // Subsequent hits: 50% damage (or whatever falloff is)
                damageToDeal = this.damage * (falloffPercentage / 100f);
            }

            stats.TakeDamage(damageToDeal);

            // [NEW] Add enemy to the "hit" list so we don't hit them again
            hitEnemies.Add(other);

            // [NEW] Play Hit Sound
            if (hitSound != null)
            {
                SoundManager.Instance.PlaySFX(hitSound, 0.1f);
            }

            // Note: We do NOT deactivate the projectile. This is penetration.
            // Note: We do NOT apply knockback, as requested.
        }
    }
}