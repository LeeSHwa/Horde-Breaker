using UnityEngine;

public class FireballSkill : Skills
{
    private FireballDataSO fireballData;

    protected float currentSpeed;
    protected float currentLifetime;
    protected float currentArea = 1f;

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

        currentSpeed = fireballData.baseProjectileSpeed;
        currentLifetime = fireballData.baseProjectileLifetime;
        currentArea = 1f;
    }

    protected override void PerformAttack()
    {
        // Calculate total count including passives
        int finalCount = currentProjectileCount + ownerStats.bonusProjectileCount;

        // Random fire direction
        Vector2 fireDirection = Random.insideUnitCircle.normalized;
        if (fireDirection == Vector2.zero) fireDirection = Vector2.right;

        // Calculate spread logic
        float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        float spread = fireballData.spreadAngle;

        // Adjust start angle to center the fan shape
        float startAngle = baseAngle - (spread * (finalCount - 1) / 2f);

        for (int i = 0; i < finalCount; i++)
        {
            GameObject proj = PoolManager.Instance.GetFromPool(fireballData.projectilePrefab.name);
            if (proj == null) continue;

            float currentAngle = startAngle + (spread * i);
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
            case 2: ApplyStats(fireballData.level2_DamageBonus, fireballData.level2_AreaBonus, fireballData.level2_CooldownReduction, fireballData.level2_ProjectileCountIncrease); break;
            case 3: ApplyStats(fireballData.level3_DamageBonus, fireballData.level3_AreaBonus, fireballData.level3_CooldownReduction, fireballData.level3_ProjectileCountIncrease); break;
            case 4: ApplyStats(fireballData.level4_DamageBonus, fireballData.level4_AreaBonus, fireballData.level4_CooldownReduction, fireballData.level4_ProjectileCountIncrease); break;
            case 5: ApplyStats(fireballData.level5_DamageBonus, fireballData.level5_AreaBonus, fireballData.level5_CooldownReduction, fireballData.level5_ProjectileCountIncrease); break;
            case 6: ApplyStats(fireballData.level6_DamageBonus, fireballData.level6_AreaBonus, fireballData.level6_CooldownReduction, fireballData.level6_ProjectileCountIncrease); break;
            case 7: ApplyStats(fireballData.level7_DamageBonus, fireballData.level7_AreaBonus, fireballData.level7_CooldownReduction, fireballData.level7_ProjectileCountIncrease); break;
            case 8: ApplyStats(fireballData.level8_DamageBonus, fireballData.level8_AreaBonus, fireballData.level8_CooldownReduction, fireballData.level8_ProjectileCountIncrease); break;
            case 9: ApplyStats(fireballData.level9_DamageBonus, fireballData.level9_AreaBonus, fireballData.level9_CooldownReduction, fireballData.level9_ProjectileCountIncrease); break;
        }

        // Safety cap for cooldown
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
    }

    private void ApplyStats(float dmg, float area, float cooldown, int count)
    {
        currentDamage += dmg;
        currentArea += area;
        currentAttackCooldown -= cooldown;
        currentProjectileCount += count;
    }
}