using UnityEngine;
using System.Collections.Generic;

// [New Logic Applied Version]
// 1. On Enemy Hit: Consume 'remainingBounces' + Reset 'lifetime'
// 2. On Target Detection Failure: Launch in a random direction
// 3. On Wall (Screen Edge) Hit: No bounce consumption + No 'lifetime' reset
public class RicochetProjectile : MonoBehaviour
{
    // --- Stats passed from the Weapon ---
    // These variables are set by the 'Weapon' script that fires this projectile.
    private float currentDamage;      // The amount of damage this projectile deals on hit.
    private float speed;              // How fast the projectile travels (units per second).
    private int remainingBounces;     // Tracks how many more *enemy* hits this projectile can perform.
    private float bounceRange;        // The maximum radius to search for a new enemy target after a hit.
    private Transform attackSource;   // Reference to the entity that fired this projectile (e.g., the player).

    // --- Internal State ---
    // These variables manage the projectile's own state during its life.
    private float lifetimeTimer;      // The *current* remaining time before this projectile is destroyed.
    private float initialLifetime;    // The *original* lifetime value, used to reset 'lifetimeTimer' on an enemy hit.
    private Vector2 moveDirection;    // The current direction (normalized vector) in which the projectile is moving.

    // A list to "remember" enemies this *single projectile* has already hit.
    // This prevents hitting the same enemy multiple times in one bounce chain.
    private List<Transform> hitEnemies;

    // --- Variables for Screen Bound Bouncing ---
    private float bulletRadius;       // The calculated radius of the projectile, used for accurate wall collision.

    /// <summary>
    /// Called once when the GameObject is first created (or enabled if using an Object Pool).
    /// We calculate the projectile's radius here for wall collision logic.
    /// </summary>
    void Start()
    {
        // Try to get a CircleCollider2D to determine the radius.
        if (TryGetComponent<CircleCollider2D>(out var circleCol))
        {
            // Calculate radius, accounting for the object's scale.
            bulletRadius = circleCol.radius * transform.localScale.x;
        }
        // If no circle collider, try a BoxCollider2D.
        else if (TryGetComponent<BoxCollider2D>(out var boxCol))
        {
            // Use the largest half-dimension as a rough radius, accounting for scale.
            bulletRadius = Mathf.Max(boxCol.size.x / 2f, boxCol.size.y / 2f) * transform.localScale.x;
        }
        // If no collider is found, set a small default and warn the developer.
        else
        {
            bulletRadius = 0.1f;
            Debug.LogWarning("RicochetProjectile: Circle/Box Collider not found for calculating bullet radius.");
        }
    }

    /// <summary>
    /// This public method is called by the 'Weapon' script immediately after creating/activating the projectile.
    /// It "sets up" the projectile with all its necessary stats.
    /// </summary>
    public void Initialize(float damage, float speed, int maxBounces, float range, float lifetime, Transform source)
    {
        this.currentDamage = damage;
        this.speed = speed;
        this.remainingBounces = maxBounces; // Set the total number of *enemy* bounces allowed.
        this.bounceRange = range;

        // Store the original lifetime and also set the current timer to that value.
        this.initialLifetime = lifetime;
        this.lifetimeTimer = lifetime;

        this.attackSource = source;
        this.moveDirection = transform.right; // Set initial direction (forward from the weapon).

        // --- Object Pooling Safety ---
        // If this is the very first time, the list will be null. Create it.
        if (hitEnemies == null)
        {
            hitEnemies = new List<Transform>();
        }
        // If this projectile is being re-used from a pool, clear the "hit list" from its previous life.
        hitEnemies.Clear();
    }

    /// <summary>
    /// Called every single frame. This is the main "heartbeat" of the projectile.
    /// </summary>
    void Update()
    {
        // 1. Move the projectile
        // Use 'Space.World' to ensure movement is consistent regardless of projectile rotation.
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 2. Check for wall bounces
        // This function will handle reflecting the projectile if it hits a screen edge.
        // As per Rule 3, this does NOT consume 'remainingBounces' or reset 'lifetimeTimer'.
        CheckScreenBounds();

        // 3. Handle projectile lifetime
        // Count down the timer.
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            // If time runs out, deactivate the projectile (for object pooling).
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// This function is called by Unity's 2D physics engine whenever our "Trigger" collider
    /// overlaps with another collider.
    /// </summary>
    /// <param name="collision">The collider of the object we hit.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // First, check if the object we hit is tagged "Enemy".
        if (collision.CompareTag("Enemy"))
        {
            // 1. Check if we've *already* hit this enemy with *this specific projectile*.
            if (hitEnemies.Contains(collision.transform))
            {
                // If yes, do nothing. Just pass through. This prevents double-hits.
                return;
            }

            // 2. If it's a new enemy, apply damage.
            if (collision.TryGetComponent<StatsController>(out StatsController enemyStats))
            {
                enemyStats.TakeDamage(currentDamage);
            }

            // 3. "Remember" this enemy.
            // Add it to our "hit list" so we don't hit it (or damage it) again.
            hitEnemies.Add(collision.transform);

            // 4. Handle the Ricochet logic (Rule 1)
            // Check if we still have enemy-bounces left.
            if (remainingBounces > 0)
            {
                // 4a. Consume one bounce.
                remainingBounces--;

                // 4b. [Core Logic] Reset the projectile's lifetime (Rule 1).
                // This gives it a full new duration to find its next target.
                lifetimeTimer = initialLifetime;

                // 4c. Find a new target to bounce towards (Rule 2).
                FindNextTarget(collision.transform.position);
            }
            // 5. [Penetration Logic]
            // If 'remainingBounces' is 0, we do nothing here.
            // The projectile will *not* ricochet or reset its lifetime,
            // but it *did* still deal damage (in step 2) and will continue flying (penetrating).
        }
    }

    /// <summary>
    /// Searches for the next valid enemy target after a successful hit.
    /// </summary>
    /// <param name="currentHitPosition">The position of the enemy we just hit.</param>
    private void FindNextTarget(Vector2 currentHitPosition)
    {
        // Ask the physics engine for all colliders within 'bounceRange' of the hit enemy.
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(currentHitPosition, bounceRange);

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity; // Start with an infinitely large distance.

        // Loop through every object found.
        foreach (var target in potentialTargets)
        {
            // Skip if it's not an "Enemy".
            if (!target.CompareTag("Enemy")) continue;

            // Skip if it's an enemy we've *already hit* during this chain.
            if (hitEnemies.Contains(target.transform)) continue;

            // Calculate distance from *us* (the projectile) to this potential target.
            float distance = Vector2.Distance(transform.position, target.transform.position);

            // If this new target is closer than our current 'bestTarget'...
            if (distance < closestDistance)
            {
                // ...update it as our new best target.
                closestDistance = distance;
                bestTarget = target.transform;
            }
        }

        // After checking all potential targets...
        if (bestTarget != null)
        {
            // We found a valid next target!
            // Calculate the direction vector towards it and normalize it.
            moveDirection = (bestTarget.position - transform.position).normalized;
        }
        else
        {
            // [Rule 2] We couldn't find a new, valid target.
            // So, just pick a random direction to fly in.
            moveDirection = Random.insideUnitCircle.normalized;
        }

        // Point the projectile's sprite (transform.right) in the new direction.
        transform.right = moveDirection;
    }

    /// <summary>
    /// Handles bouncing the projectile off the screen edges (walls). (Rule 3)
    /// This logic is independent of enemy bounces.
    /// </summary>
    /// <returns>True if a bounce occurred, false otherwise.</returns>
    private bool CheckScreenBounds()
    {
        // Get the camera's world-space boundaries from a static controller.
        Vector2 currentMinBounds = CameraBoundsController.MinBounds;
        Vector2 currentMaxBounds = CameraBoundsController.MaxBounds;

        Vector2 pos = transform.position; // Our current position.
        bool bounced = false;             // Flag to track if we bounced.
        Vector2 normal = Vector2.zero;    // The normal vector of the wall we hit.

        // 1. Check Left/Right Bounds
        // Check if we are past the left edge (accounting for our radius).
        if (pos.x < currentMinBounds.x + bulletRadius)
        {
            pos.x = currentMinBounds.x + bulletRadius; // Clamp position to the edge.
            normal = Vector2.right; // The "normal" of the left wall points right.
            bounced = true;
        }
        // Check if we are past the right edge.
        else if (pos.x > currentMaxBounds.x - bulletRadius)
        {
            pos.x = currentMaxBounds.x - bulletRadius;
            normal = Vector2.left; // The "normal" of the right wall points left.
            bounced = true;
        }

        // 2. Check Top/Bottom Bounds
        // Check if we are past the bottom edge.
        if (pos.y < currentMinBounds.y + bulletRadius)
        {
            pos.y = currentMinBounds.y + bulletRadius;
            normal = Vector2.up; // The "normal" of the bottom wall points up.
            bounced = true;
        }
        // Check if we are past the top edge.
        else if (pos.y > currentMaxBounds.y - bulletRadius)
        {
            pos.y = currentMaxBounds.y - bulletRadius;
            normal = Vector2.down; // The "normal" of the top wall points down.
            bounced = true;
        }

        // 3. Apply the Bounce
        if (bounced)
        {
            transform.position = pos; // Apply the clamped position.

            // Calculate the reflection vector. This is the core physics.
            moveDirection = Vector2.Reflect(moveDirection.normalized, normal);

            // Point the sprite in the new reflected direction.
            transform.right = moveDirection;

            // [Rule 3] Note that we do NOT decrease 'remainingBounces' or reset 'lifetimeTimer' here.
            // Wall bounces are "free".

            return true; // Report that a bounce happened.
        }

        return false; // No bounce happened.
    }
}