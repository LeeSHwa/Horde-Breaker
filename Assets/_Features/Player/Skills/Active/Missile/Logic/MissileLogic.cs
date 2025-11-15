using UnityEngine;

public class MissileLogic : MonoBehaviour
{
    private float damage;
    private float speed;
    private float searchRadius; // Range to find enemies
    private Transform target;   // Currently locked target

    private float safetyTimer = 10f;

    // Optimization: Don't scan every single frame. Scan 10 times a second.
    private float scanTimer = 0f;
    private const float SCAN_INTERVAL = 0.1f;

    // [Modified] Now receives 'radius' instead of 'target'
    public void Initialize(float dmg, float spd, float radius)
    {
        this.damage = dmg;
        this.speed = spd;
        this.searchRadius = radius;
        this.target = null; // Start with no target
        this.safetyTimer = 10f;
        this.scanTimer = 0f;
    }

    void Update()
    {
        // 1. Safety Timer
        safetyTimer -= Time.deltaTime;
        if (safetyTimer <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        // 2. Target Scanning (Find Nearest Enemy relative to ME)
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            FindNearestTarget();
            scanTimer = SCAN_INTERVAL; // Reset timer
        }

        // 3. Homing & Movement
        if (target != null && target.gameObject.activeInHierarchy)
        {
            // Rotate towards current target
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smooth rotation can be added here via Lerp, but using instant turn as requested
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Always move forward
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void FindNearestTarget()
    {
        // Find all colliders in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.gameObject.activeInHierarchy)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = hit.transform;
                }
            }
        }

        // Update target (It might switch to a closer enemy mid-flight)
        this.target = bestTarget;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<StatsController>(out var enemyStats))
            {
                enemyStats.TakeDamage(damage);
            }

            if (other.TryGetComponent<EnemyMovement>(out var enemyMove))
            {
                enemyMove.ApplyKnockback(transform.right, 3f, 0.1f);
            }

            // Destroy on impact
            gameObject.SetActive(false);
        }
    }
}