using UnityEngine;
using System.Collections.Generic;

// [New Logic Applied Version]
public class RicochetProjectile : MonoBehaviour
{
    // --- Stats passed from the Weapon ---
    private float currentDamage;
    private float speed;
    private int remainingBounces;
    private float bounceRange;
    private Transform attackSource;

    // [NEW] Variable to store hit sound
    private AudioClip hitSound;
    // [NEW] Is Crit?
    private bool isCritical;

    // --- Internal State ---
    private float lifetimeTimer;
    private float initialLifetime;
    private Vector2 moveDirection;

    private List<Transform> hitEnemies;

    // --- Variables for Screen Bound Bouncing ---
    private float bulletRadius;

    void Start()
    {
        if (TryGetComponent<CircleCollider2D>(out var circleCol))
        {
            bulletRadius = circleCol.radius * transform.localScale.x;
        }
        else if (TryGetComponent<BoxCollider2D>(out var boxCol))
        {
            bulletRadius = Mathf.Max(boxCol.size.x / 2f, boxCol.size.y / 2f) * transform.localScale.x;
        }
        else
        {
            bulletRadius = 0.1f;
            Debug.LogWarning("RicochetProjectile: Circle/Box Collider not found for calculating bullet radius.");
        }
    }

    // [MODIFIED] Added 'AudioClip sound' & 'bool isCrit' parameter
    public void Initialize(float damage, float speed, int maxBounces, float range, float lifetime, Transform source, AudioClip sound = null, bool isCrit = false)
    {
        this.currentDamage = damage;
        this.speed = speed;
        this.remainingBounces = maxBounces;
        this.bounceRange = range;

        this.initialLifetime = lifetime;
        this.lifetimeTimer = lifetime;

        this.attackSource = source;
        this.hitSound = sound; // [NEW] Store hit sound
        this.isCritical = isCrit; // [NEW] Store crit status
        this.moveDirection = transform.right;

        if (hitEnemies == null)
        {
            hitEnemies = new List<Transform>();
        }
        hitEnemies.Clear();
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        CheckScreenBounds();

        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hitEnemies.Contains(collision.transform))
            {
                return;
            }

            if (collision.TryGetComponent<StatsController>(out StatsController enemyStats))
            {
                // [MODIFIED] Pass isCritical
                enemyStats.TakeDamage(currentDamage, isCritical);

                // [NEW] Play Hit Sound
                if (hitSound != null)
                {
                    SoundManager.Instance.PlaySFX(hitSound, 0.1f);
                }
            }

            hitEnemies.Add(collision.transform);

            if (remainingBounces > 0)
            {
                remainingBounces--;
                lifetimeTimer = initialLifetime;
                FindNextTarget(collision.transform.position);
            }
        }
    }

    private void FindNextTarget(Vector2 currentHitPosition)
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(currentHitPosition, bounceRange);

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in potentialTargets)
        {
            if (!target.CompareTag("Enemy")) continue;
            if (hitEnemies.Contains(target.transform)) continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = target.transform;
            }
        }

        if (bestTarget != null)
        {
            moveDirection = (bestTarget.position - transform.position).normalized;
        }
        else
        {
            moveDirection = Random.insideUnitCircle.normalized;
        }

        transform.right = moveDirection;
    }

    private bool CheckScreenBounds()
    {
        Vector2 currentMinBounds = CameraBoundsController.MinBounds;
        Vector2 currentMaxBounds = CameraBoundsController.MaxBounds;

        Vector2 pos = transform.position;
        bool bounced = false;
        Vector2 normal = Vector2.zero;

        if (pos.x < currentMinBounds.x + bulletRadius)
        {
            pos.x = currentMinBounds.x + bulletRadius;
            normal = Vector2.right;
            bounced = true;
        }
        else if (pos.x > currentMaxBounds.x - bulletRadius)
        {
            pos.x = currentMaxBounds.x - bulletRadius;
            normal = Vector2.left;
            bounced = true;
        }

        if (pos.y < currentMinBounds.y + bulletRadius)
        {
            pos.y = currentMinBounds.y + bulletRadius;
            normal = Vector2.up;
            bounced = true;
        }
        else if (pos.y > currentMaxBounds.y - bulletRadius)
        {
            pos.y = currentMaxBounds.y - bulletRadius;
            normal = Vector2.down;
            bounced = true;
        }

        if (bounced)
        {
            transform.position = pos;
            moveDirection = Vector2.Reflect(moveDirection.normalized, normal);
            transform.right = moveDirection;
            return true;
        }

        return false;
    }
}