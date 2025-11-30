using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(StatsController))]
public class EnemyRangeAttackSkill : MonoBehaviour
{
    [Header("Projectile Stats")]
    public string projectileTag = "BossWave";
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;        // Initial speed (recommend starting slow)
    public float projectileAcceleration = 5f; // [ADDED] Speed increase per second
    public float projectileLifetime = 3f;
    public float damageMultiplier = 1.0f;

    [Header("Multi-Shot Settings")]
    [Range(1, 10)]
    public int projectileCount = 1;           // [ADDED] Number of projectiles
    [Range(0f, 45f)]
    public float multiShotAngle = 15f;        // [ADDED] Angle between projectiles

    [Header("Skill Settings")]
    public float triggerRange = 8f;
    public float skillCooldown = 6.0f;

    [Header("Timing Settings")]
    public float chargeTime = 0.5f;    // Time to aim/telegraph attack
    public float fireDelay = 0.2f;     // Short delay during attack anim before spawning projectile
    public float recoveryTime = 0.5f;  // Post-attack freeze

    [Header("Visual Settings")]
    public bool useColorChange = true;
    public Color chargeColor = Color.red;
    public Color fireColor = new Color(1f, 0.5f, 0f); // Orange

    [Header("Animation Settings")]
    public string animChargeTrigger = "doWaveCharge";
    public string animFireTrigger = "doWaveFire";
    public string animIdleTrigger = "doIdle";
    public string animMoveBool = "isMoving";

    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private StatsController myStats;
    private Transform playerTarget;
    private Color originalColor;
    private float lastUsedTime = -999f;

    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        myStats = GetComponent<StatsController>();

        if (sr != null) originalColor = sr.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    // [Manager Call]
    public bool IsReady(float distanceToPlayer)
    {
        bool isCooldownReady = Time.time >= lastUsedTime + skillCooldown;
        bool isRangeReady = distanceToPlayer <= triggerRange;
        bool isNotDisabled = (movementScript != null && movementScript.enabled);

        return isCooldownReady && isRangeReady && isNotDisabled;
    }

    // [Manager Call]
    public void Execute(System.Action onComplete)
    {
        lastUsedTime = Time.time;
        StartCoroutine(AttackRoutine(onComplete));
    }

    private IEnumerator AttackRoutine(System.Action onComplete)
    {
        // --- PHASE 1: CHARGE (Aiming) ---
        if (movementScript != null) movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero;

        FacePlayer();

        if (anim != null)
        {
            anim.SetBool(animMoveBool, false);
            anim.SetTrigger(animChargeTrigger);
        }

        if (useColorChange && sr != null) sr.color = chargeColor;

        yield return new WaitForSeconds(chargeTime);

        // --- PHASE 2: FIRE ---
        if (anim != null) anim.SetTrigger(animFireTrigger);
        if (useColorChange && sr != null) sr.color = fireColor;

        yield return new WaitForSeconds(fireDelay);

        SpawnProjectiles(); // [MODIFIED] Call the multi-shot function

        // --- PHASE 3: RECOVERY ---
        yield return new WaitForSeconds(recoveryTime);

        // Restore State
        if (useColorChange && sr != null) sr.color = originalColor;
        if (anim != null) anim.SetTrigger(animIdleTrigger);

        if (movementScript != null) movementScript.enabled = true;

        // Notify Manager
        onComplete?.Invoke();
    }

    private void FacePlayer()
    {
        if (playerTarget == null || sr == null) return;
        Vector2 direction = playerTarget.position - transform.position;
        if (direction.x < 0) sr.flipX = true;
        else sr.flipX = false;
    }

    // Logic for Fan-Shaped Multi-Shot
    private void SpawnProjectiles()
    {
        if (playerTarget == null) return;

        // Calculate Damage
        float damage = 10f;
        if (myStats != null && myStats.baseStats is EnemyStatsSO enemyStats)
        {
            damage = enemyStats.contactDamage * myStats.currentDamageMultiplier * damageMultiplier;
        }

        // Base direction towards the player
        Vector2 baseDirection = (playerTarget.position - transform.position).normalized;

        // Calculate the starting angle for the fan shape
        // e.g., if count=3 and angle=15, start at -15 degrees relative to base direction
        float startAngle = -((projectileCount - 1) * multiShotAngle) / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            // 1. Calculate current angle offset
            float currentAngleOffset = startAngle + (i * multiShotAngle);

            // 2. Rotate the base direction vector
            Quaternion rotation = Quaternion.AngleAxis(currentAngleOffset, Vector3.forward);
            Vector2 fireDirection = rotation * baseDirection;

            // 3. Create and initialize the projectile
            CreateSingleProjectile(damage, fireDirection);
        }
    }

    private void CreateSingleProjectile(float damage, Vector2 dir)
    {
        GameObject projObj = null;
        if (PoolManager.Instance != null)
        {
            projObj = PoolManager.Instance.GetFromPool(projectileTag);
        }

        if (projObj == null && projectilePrefab != null)
        {
            projObj = Instantiate(projectilePrefab);
        }

        if (projObj != null)
        {
            projObj.transform.position = transform.position;
            projObj.SetActive(true);

            EnemyProjectile projectileScript = projObj.GetComponent<EnemyProjectile>();
            if (projectileScript != null)
            {
                // Pass 'projectileAcceleration' to initialization
                projectileScript.Initialize(damage, projectileSpeed, projectileAcceleration, projectileLifetime, dir);
            }
        }
    }
}