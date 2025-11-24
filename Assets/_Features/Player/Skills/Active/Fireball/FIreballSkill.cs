using UnityEngine;

// [NEW] The skill logic that 'shoots' the fireball
public class FireballSkill : Skills // Inherits from Skills
{
    private FireballDataSO fireballData;

    // [REMOVED] Rigidbody2D is no longer needed to determine direction
    // private Rigidbody2D ownerRb; 

    // [REMOVED] We no longer need to store the last movement direction
    // private Vector2 lastMoveDirection;

    // Runtime stats
    protected float currentSpeed;
    protected float currentLifetime;
    protected float currentArea = 1f;

    protected override void Awake()
    {
        base.Awake(); // Sets ownerStats

        // [REMOVED] No longer need to get the Rigidbody2D
        // ownerRb = ownerStats.GetComponent<Rigidbody2D>();

        if (skillData is FireballDataSO)
        {
            fireballData = (FireballDataSO)skillData;
        }
        else
        {
            Debug.LogError("FireballSkill has the wrong SkillDataSO assigned!");
        }

        // [REMOVED] No longer need to set a default direction
        // lastMoveDirection = Vector2.right; 

        InitializeStats();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats(); // Sets base damage, cooldown, projectile count (1)
        currentSpeed = fireballData.baseProjectileSpeed;
        currentLifetime = fireballData.baseProjectileLifetime;
        currentArea = 1f; // 100% scale

        // [TESTING NOTE] If you are still testing 3-way shot, keep this line:
        // currentProjectileCount = 3; 
    }


    protected override void PerformAttack()
    {
        // [MODIFIED] This is the core change.
        // Generate a new random direction every time PerformAttack is called.
        Vector2 fireDirection = Random.insideUnitCircle.normalized;

        // Fallback in case the random vector is (0,0)
        if (fireDirection == Vector2.zero)
        {
            fireDirection = Vector2.right;
        }

        // Calculate the base angle from the new random fire direction
        float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        // Calculate the starting angle for the 3-way shot (if count is 3)
        // e.g., if count=3, angle=15: -15, 0, +15
        float startAngle = baseAngle - (fireballData.level5_SpreadAngle * (currentProjectileCount - 1) / 2f);

        for (int i = 0; i < currentProjectileCount; i++)
        {
            GameObject proj = PoolManager.Instance.GetFromPool(fireballData.projectilePrefab.name);
            if (proj == null) continue;

            // Calculate the angle for this specific projectile
            float currentAngle = startAngle + (fireballData.level5_SpreadAngle * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            proj.transform.position = ownerStats.transform.position;
            proj.transform.rotation = rotation;

            FireballLogic logic = proj.GetComponent<FireballLogic>();
            if (logic != null)
            {
                float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;
                logic.Initialize(
                    finalDamage,
                    currentSpeed,
                    currentLifetime,
                    fireballData.damageFalloffPercentage,
                    currentArea, // Send the current size
                    skillData.hitSound // [NEW] Pass hit sound
                );
            }
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                currentDamage += fireballData.level2_DamageIncrease;
                break;
            case 3:
                currentArea += fireballData.level3_AreaIncrease; // e.g., 1.0 -> 1.2
                break;
            case 4:
                currentAttackCooldown -= fireballData.level4_CooldownReduction;
                // [Added] Safety Cap
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5:
                currentProjectileCount = fireballData.level5_ProjectileCount;
                break;
        }
    }
}