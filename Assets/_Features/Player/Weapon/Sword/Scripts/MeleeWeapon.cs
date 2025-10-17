using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// MeleeWeapon class inherits from the base Weapon class. // MeleeWeapon 클래스는 기본 Weapon 클래스를 상속받음.
public class MeleeWeapon : Weapon
{
    [Header("Melee Specifics")]
    // The speed at which the weapon swings. // 무기가 휘둘러지는 속도.
    public float swingSpeed = 300f;
    // The total angle of the weapon's swing arc. // 무기 스윙 궤적의 총 각도.
    public float swingAngle = 90f;
    // Toggles between swinging constantly or only when enemies are in range. // 항상 스윙할지, 적이 범위 내에 있을 때만 스윙할지 토글함.
    public bool alwaysSwing = true;
    // Toggles between a one-way or two-way (back and forth) swing. // 단방향 또는 왕복(앞뒤) 스윙을 토글함.
    public bool isTwoWaySwing = false;

    [Header("Object Assignments")]
    // Assign the child object responsible for the trail effect here. // 잔상 효과를 담당하는 자식 오브젝트를 여기에 할당.
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
        // 무기의 '기본' 스윙 속성을 기반으로 타격 속도를 계산함.
        if (swingSpeed > 0)
        {
            // The cooldown is ALWAYS based on a single pass at the BASE speed. This is the core anti-spam mechanic.
            // 쿨다운은 '항상' 기본 속도에서의 단일 스윙을 기준으로 함. 이것이 핵심적인 스팸 방지 기능임.
            singleTargetHitCooldown = swingAngle / swingSpeed + 0.05f;
        }
        else
        {
            singleTargetHitCooldown = 0.1f; // Avoid division by zero. // 0으로 나누기 방지.
        }
    }

    // This method is called by PlayerAttack and checks for enemies. // 이 메소드는 PlayerAttack에 의해 호출되며 적을 확인함.
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

    // A coroutine that handles the swing animation over multiple frames. // 여러 프레임에 걸쳐 스윙 애니메이션을 처리하는 코루틴.
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
        // --- DPS 수정 ---
        // 만약 왕복 스윙이 활성화되었다면, 이번 스윙 애니메이션의 속도를 두 배로 함.
        float currentSwingSpeed = isTwoWaySwing ? swingSpeed * 2f : swingSpeed;

        float startAngle = -swingAngle / 2;
        float endAngle = swingAngle / 2;
        float time = 0f;

        // Forward swing animation using the calculated speed. // 계산된 속도를 사용한 정방향 스윙 애니메이션.
        while (time < 1f)
        {
            time += Time.deltaTime * currentSwingSpeed / swingAngle;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, time);
            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
            CheckForHit();
            yield return null;
        }

        // If it's a two-way swing, animate the backward swing. // 만약 왕복 스윙이라면, 역방향 스윙을 애니메이트함.
        if (isTwoWaySwing)
        {
            time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * currentSwingSpeed / swingAngle;
                float currentAngle = Mathf.Lerp(endAngle, startAngle, time);
                transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
                CheckForHit(); // Check for hits on the way back as well. // 돌아오는 길에도 충돌을 확인.
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

    // Checks for collision with enemies using the DPS-based system. // DPS 기반 시스템을 사용하여 적과의 충돌을 확인.
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
                    // Check if enough time has passed to hit this specific enemy again. // 이 특정 적을 다시 때릴 만큼 충분한 시간이 지났는지 확인.
                    if (!lastHitTimes.ContainsKey(enemyStats) || Time.time >= lastHitTimes[enemyStats] + singleTargetHitCooldown)
                    {
                        enemyStats.TakeDamage(damage);
                        ApplyKnockback(enemyStats.transform);
                        // Update the last hit time for this enemy. // 이 적의 마지막 타격 시간을 갱신.
                        lastHitTimes[enemyStats] = Time.time;
                    }
                }
            }
        }
    }

    // Detects if enemies are within the swing arc. // 적이 스윙 궤적 내에 있는지 감지하는 함수.
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

    // This function now tells the enemy to knock ITSELF back. // 이 함수는 이제 적에게 스스로 넉백하라고 명령함.
    private void ApplyKnockback(Transform enemy)
    {
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();

        if (enemyMovement != null && playerTransform != null)
        {
            Vector2 knockbackDirection = (enemy.position - playerTransform.position).normalized;
            float knockbackDuration = 0.2f; // How long the enemy is stunned. // 적이 스턴되는 시간.
            enemyMovement.ApplyKnockback(knockbackDirection, knockback, knockbackDuration);
        }
    }
}


