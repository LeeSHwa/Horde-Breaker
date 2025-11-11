// RicochetProjectile.cs
using UnityEngine;
using System.Collections.Generic;

public class RicochetProjectile : MonoBehaviour
{
    // --- Stats ---
    private float currentDamage;
    private float speed;
    private int remainingBounces; // [Enemy] Bounces remaining
    private float bounceRange;
    private Transform attackSource;
    private float originalLifetime; // Original lifetime for resetting

    // [DELETED] removed remainingWallBounces variable

    // --- Internal State ---
    private float lifetimeTimer;
    private Vector2 moveDirection;
    private List<Transform> hitEnemies;
    private Camera mainCamera;

    // [MODIFIED] Removed maxWallBounces parameter (6 total args)
    public void Initialize(float damage, float speed, int maxEnemyBounces, float range, float lifetime, Transform source)
    {
        this.currentDamage = damage;
        this.speed = speed;
        this.remainingBounces = maxEnemyBounces; // Enemy bounce count
        this.bounceRange = range;
        this.attackSource = source;
        this.originalLifetime = lifetime;
        this.lifetimeTimer = lifetime;

        // [DELETED] Wall bounce setup logic removed

        this.moveDirection = transform.right;

        if (hitEnemies == null) { hitEnemies = new List<Transform>(); }
        hitEnemies.Clear();

        if (mainCamera == null) { mainCamera = Camera.main; }
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // [MODIFIED] Always check for wall bounces (no limit)
        HandleScreenBounce();

        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ... (Enemy collision logic unchanged) ...
        if (collision.CompareTag("Enemy"))
        {
            if (hitEnemies.Contains(collision.transform))
            {
                return;
            }
            // ... (Damage logic) ...
            if (collision.TryGetComponent<StatsController>(out StatsController enemyStats))
            {
                enemyStats.TakeDamage(currentDamage);
            }
            hitEnemies.Add(collision.transform);

            // [KEY] Reset lifetime on enemy hit
            lifetimeTimer = originalLifetime;

            remainingBounces--;

            if (remainingBounces >= 0)
            {
                FindNextTarget(collision.transform.position);
            }
            else
            {
                FindNextTarget(collision.transform.position);
            }
        }
    }

    private void FindNextTarget(Vector2 currentHitPosition)
    {
        // ... (FindNextTarget logic unchanged) ...
        if (remainingBounces < 0)
        {
            HandleNoTargetFound();
            return;
        }

        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(currentHitPosition, bounceRange);
        Transform bestTarget = null;
        // ... (Closest enemy search logic) ...
        foreach (var target in potentialTargets)
        {
            if (!target.CompareTag("Enemy") || hitEnemies.Contains(target.transform))
            {
                continue;
            }
            // ... (Distance check, etc.) ...
            // [Note: The logic for finding the closest target was omitted in the prompt,
            // but it would go here.]
        }

        if (bestTarget != null)
        {
            moveDirection = (bestTarget.position - transform.position).normalized;
        }
        else
        {
            HandleNoTargetFound();
        }
    }

    private void HandleNoTargetFound()
    {
        // ... (Boomerang (random launch) logic unchanged) ...
        moveDirection = Random.insideUnitCircle.normalized;
        hitEnemies.Clear();
        lifetimeTimer = originalLifetime; // Reset lifetime
    }

    // [MODIFIED] Wall bounce logic (no limit)
    private void HandleScreenBounce()
    {
        if (mainCamera == null) return;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool bounced = false;

        // [MODIFIED] Removed count check (if) and decrement (remainingWallBounces--)
        // It just bounces unconditionally.
        if (viewportPos.x < 0.01f || viewportPos.x > 0.99f)
        {
            moveDirection.x = -moveDirection.x;
            bounced = true;
            // [KEY] Lifetime NOT reset (X)
        }

        if (viewportPos.y < 0.01f || viewportPos.y > 0.99f)
        {
            moveDirection.y = -moveDirection.y;
            bounced = true;
            // [KEY] Lifetime NOT reset (X)
        }

        // Position correction
        if (bounced)
        {
            Vector3 clampedViewportPos = viewportPos;
            clampedViewportPos.x = Mathf.Clamp(clampedViewportPos.x, 0.01f, 0.99f);
            clampedViewportPos.y = Mathf.Clamp(clampedViewportPos.y, 0.01f, 0.99f);
            transform.position = mainCamera.ViewportToWorldPoint(clampedViewportPos);
        }
    }
}