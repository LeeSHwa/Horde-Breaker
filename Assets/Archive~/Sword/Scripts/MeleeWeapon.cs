using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// MeleeWeapon class inherits from the base Weapon class.
public class MeleeWeapon : Weapon
{
    [Header("Melee Specifics")]
    // The speed at which the weapon swings. 
    public float swingSpeed = 300f;
    // The total angle of the weapon's swing arc.
    public float swingAngle = 90f;
    // Toggles between swinging constantly or only when enemies are in range.
    public bool alwaysSwing = true;
    // Toggles between a one-way or two-way (back and forth) swing.
    public bool isTwoWaySwing = false;

    [Header("Object Assignments")]
    // Assign the child object responsible for the trail effect here. 
    public Transform trailEmitter;

    private bool isSwinging = false;
    private Transform playerTransform;
    private TrailRenderer trailRenderer;
    private Dictionary<CharacterStats, float> lastHitTimes;
    private float singleTargetHitCooldown;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (trailEmitter != null)
        {
            trailRenderer = trailEmitter.GetComponent<TrailRenderer>();
        }

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
        else
        {
            Debug.LogError("TrailEmitter is not assigned or does not have a TrailRenderer component.");
        }

        lastHitTimes = new Dictionary<CharacterStats, float>();

        // Calculate the weapon's hit rate based on its BASE swing properties.

        if (swingSpeed > 0)
        {
            // The cooldown is ALWAYS based on a single pass at the BASE speed. This is the core anti-spam mechanic.
            singleTargetHitCooldown = swingAngle / swingSpeed + 0.05f;
        }
        else
        {
            singleTargetHitCooldown = 0.1f; // Avoid division by zero. 
        }
    }

    // This method is called by PlayerAttack and checks for enemies.
    public override void TryAttack()
    {
        if (isSwinging) return;

        if (alwaysSwing || DetectEnemies())
        {
            base.TryAttack();
        }
    }

    protected override void PerformAttack()
    {
        StartCoroutine(SwingCoroutine());
    }

    // A coroutine that handles the swing animation over multiple frames.  
    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;

        if (trailRenderer != null)
        {
            yield return null;
            trailRenderer.Clear();
            trailRenderer.emitting = true;
        }

        // --- DPS FIX ---
        // If two-way swing is enabled, double the speed for this swing animation.
        // --- DPS ���� ---
 
        float currentSwingSpeed = isTwoWaySwing ? swingSpeed * 2f : swingSpeed;

        float startAngle = -swingAngle / 2;
        float endAngle = swingAngle / 2;
        float time = 0f;

        // Forward swing animation using the calculated speed.
        while (time < 1f)
        {
            time += Time.deltaTime * currentSwingSpeed / swingAngle;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, time);
            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
            CheckForHit();
            yield return null;
        }

        // If it's a two-way swing, animate the backward swing.
        if (isTwoWaySwing)
        {
            time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * currentSwingSpeed / swingAngle;
                float currentAngle = Mathf.Lerp(endAngle, startAngle, time);
                transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
                CheckForHit(); // Check for hits on the way back as well.
                yield return null;
            }
        }

        transform.localRotation = Quaternion.identity;
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
        isSwinging = false;
    }

    // Checks for collision with enemies using the DPS-based system.
    private void CheckForHit()
    {
        if (trailEmitter == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(trailEmitter.position, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                CharacterStats enemyStats = hit.GetComponent<CharacterStats>();
                if (enemyStats != null)
                {
                    // Check if enough time has passed to hit this specific enemy again.
                    if (!lastHitTimes.ContainsKey(enemyStats) || Time.time >= lastHitTimes[enemyStats] + singleTargetHitCooldown)
                    {
                        enemyStats.TakeDamage(damage);
                        ApplyKnockback(enemyStats.transform);
                        // Update the last hit time for this enemy. 
                        lastHitTimes[enemyStats] = Time.time;
                    }
                }
            }
        }
    }

    // Detects if enemies are within the swing arc.
    private bool DetectEnemies()
    {
        float range = 5f;
        if (playerTransform == null) return false;
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(playerTransform.position, range);

        foreach (var col in collidersInRange)
        {
            if (col.CompareTag("Enemy"))
            {
                Vector2 directionToEnemy = (col.transform.position - transform.position).normalized;
                if (Vector2.Angle(transform.parent.up, directionToEnemy) < swingAngle)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // This function now tells the enemy to knock ITSELF back.
    private void ApplyKnockback(Transform enemy)
    {
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();

        if (enemyMovement != null && playerTransform != null)
        {
            Vector2 knockbackDirection = (enemy.position - playerTransform.position).normalized;
            float knockbackDuration = 0.2f; // How long the enemy is stunned. 
            enemyMovement.ApplyKnockback(knockbackDirection, knockback, knockbackDuration);
        }
    }
}


