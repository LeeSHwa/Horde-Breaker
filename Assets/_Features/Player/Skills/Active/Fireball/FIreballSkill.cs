using UnityEngine;

public class FireballSkill : Skills
{
    private FireballDataSO fireballData;

    protected float currentSpeed;
    protected float currentLifetime;
    protected float currentArea = 1f;

    // Initialize Override
    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        if (skillData is FireballDataSO)
        {
            fireballData = (FireballDataSO)skillData;
        }
        else
        {
            Debug.LogError("FireballSkill: Wrong SkillDataSO assigned!");
            return;
        }

        // Specific Init
        currentSpeed = fireballData.baseProjectileSpeed;
        currentLifetime = fireballData.baseProjectileLifetime;
        currentArea = 1f;
    }

    protected override void PerformAttack()
    {
        // Multi-shot spread
        int count = currentProjectileCount + ownerStats.bonusProjectileCount; // Manual calc or new helper
        // Since Skills doesn't have GetFinalProjectileCount helper yet, we do it manually:
        // Or better: Skills.cs should handle stats. Let's assume manual for now.
        int finalCount = currentProjectileCount + ownerStats.bonusProjectileCount;

        Vector2 fireDirection = Random.insideUnitCircle.normalized;
        if (fireDirection == Vector2.zero) fireDirection = Vector2.right;

        float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - (fireballData.level5_SpreadAngle * (finalCount - 1) / 2f);

        for (int i = 0; i < finalCount; i++)
        {
            GameObject proj = PoolManager.Instance.GetFromPool(fireballData.projectilePrefab.name);
            if (proj == null) continue;

            float currentAngle = startAngle + (fireballData.level5_SpreadAngle * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            proj.transform.position = ownerStats.transform.position;
            proj.transform.rotation = rotation;

            FireballLogic logic = proj.GetComponent<FireballLogic>();
            if (logic != null)
            {
                float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;
                float finalArea = currentArea + ownerStats.bonusArea; // Apply passive area

                logic.Initialize(
                    finalDamage,
                    currentSpeed,
                    currentLifetime,
                    fireballData.damageFalloffPercentage,
                    finalArea,
                    skillData.hitSound
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
                currentArea += fireballData.level3_AreaIncrease;
                break;
            case 4:
                currentAttackCooldown -= fireballData.level4_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5:
                currentProjectileCount = fireballData.level5_ProjectileCount;
                break;
        }
    }
}