// RicochetProjectile.cs
using UnityEngine;
using System.Collections.Generic;

// This script controls the behavior of the projectile itself.
// It moves, detects enemies, deals damage, and finds the next target to bounce to.
public class RicochetProjectile : MonoBehaviour
{
    // --- Stats passed from the Weapon ---
    private float currentDamage;
    private float speed;
    private int remainingBounces; // How many more times this projectile can bounce.
    private float bounceRange;    // How far to look for the next target.
    private Transform attackSource; // The player (or whoever shot this).

    // --- Internal State ---
    private float lifetimeTimer;    // Countdown to self-destruct if it doesn't hit anything.
    private Vector2 moveDirection;  // The current direction the projectile is traveling.

    // [CRITICAL] A list to track enemies already hit by this specific projectile instance.
    // This prevents hitting the same enemy twice and infinite bounces between two enemies.
    private List<Transform> hitEnemies;

    // LayerMask was removed, as requested. We now use Tags.

    // This function is called by RicochetWeapon.cs immediately after spawning.
    public void Initialize(float damage, float speed, int maxBounces, float range, float lifetime, Transform source)
    {
        // 1. Set all stats passed in from the weapon.
        this.currentDamage = damage;
        this.speed = speed;
        this.remainingBounces = maxBounces;
        this.bounceRange = range;
        this.lifetimeTimer = lifetime;
        this.attackSource = source;

        // 2. Set the initial direction based on how the projectile was rotated when spawned.
        // 'transform.right' (Vector2) points in the "forward" direction of the 2D object.
        this.moveDirection = transform.right;

        // 3. Reset the hit list. This is VITAL for object pooling.
        // When this object is reused from the pool, we must clear the old list.
        if (hitEnemies == null)
        {
            hitEnemies = new List<Transform>();
        }
        hitEnemies.Clear();
    }

    // Update is called once per frame.
    void Update()
    {
        // 1. Move the projectile
        // We use Space.World to ensure it moves relative to the world, not its own rotation.
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 2. Tick down the lifetime timer
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            // If the timer runs out (e.g., it flew off-screen),
            // deactivate it to return it to the object pool.
            gameObject.SetActive(false);
        }
    }

    // Called automatically by Unity when this object's Trigger Collider overlaps another collider.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if the object we hit has the "Enemy" tag.
        if (collision.CompareTag("Enemy"))
        {
            // 2. Check if we have *already* hit this enemy with this bounce chain.
            if (hitEnemies.Contains(collision.transform))
            {
                // If yes, ignore this collision and pass through.
                return;
            }

            // 3. Apply damage to the enemy.
            if (collision.TryGetComponent<StatsController>(out StatsController enemyStats))
            {
                enemyStats.TakeDamage(currentDamage);
                // TODO: Add knockback logic here if desired, using 'attackSource'.
            }

            // 4. Add this enemy to the "hit list" to prevent re-hitting.
            hitEnemies.Add(collision.transform);

            // 5. Decrement the bounce counter.
            remainingBounces--;

            // 6. Decide what to do next.
            if (remainingBounces > 0)
            {
                // If we still have bounces left, find a new target.
                FindNextTarget(collision.transform.position);
            }
            else
            {
                // If we are out of bounces, deactivate the projectile.
                gameObject.SetActive(false);
            }
        }
    }

    // [MODIFIED] This method now uses Tags instead of Layers to find the next target.
    private void FindNextTarget(Vector2 currentHitPosition)
    {
        // 1. Get ALL colliders within the 'bounceRange' of the enemy we just hit.
        // No LayerMask is used here.
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(currentHitPosition, bounceRange);

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity; // Start with an infinitely large distance.

        // 2. Loop through every collider we found.
        foreach (var target in potentialTargets)
        {
            // 3. [FILTER 1] Check if the collider has the "Enemy" tag.
            if (!target.CompareTag("Enemy"))
            {
                continue; // Not an enemy, skip it.
            }

            // 4. [FILTER 2] Check if this enemy is already in our "hit list".
            if (hitEnemies.Contains(target.transform))
            {
                continue; // Already hit this enemy, skip it.
            }

            // 5. If it's a valid, new enemy, check if it's the closest one so far.
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                // If it is, store it as the new "best" target.
                closestDistance = distance;
                bestTarget = target.transform;
            }
        }

        // 6. After checking all potential targets...
        if (bestTarget != null)
        {
            // If we found a valid new target,
            // calculate the direction to it and update 'moveDirection'.
            moveDirection = (bestTarget.position - transform.position).normalized;
        }
        else
        {
            // If we found no new valid targets within the range,
            // deactivate the projectile. Its journey is over.
            gameObject.SetActive(false);
        }
    }
}