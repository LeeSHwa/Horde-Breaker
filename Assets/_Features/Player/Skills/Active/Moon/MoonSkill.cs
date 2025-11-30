using UnityEngine;

public class MoonSkill : Skills
{
    private MoonDataSO moonData;

    // Runtime stats
    protected float currentDuration;
    protected float currentRotationSpeed;
    protected float currentRotationRadius;

    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        if (skillData is MoonDataSO)
        {
            moonData = (MoonDataSO)skillData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong SkillDataSO assigned. Expected MoonDataSO.");
            return;
        }

        currentDuration = moonData.baseDuration;
        currentRotationSpeed = moonData.baseRotationSpeed;
        currentRotationRadius = moonData.baseRotationRadius;

        // Initialize projectile count (Default 3)
        currentProjectileCount = moonData.baseProjectileCount;
    }

    protected override void PerformAttack()
    {
        // Calculate total count including passives
        int totalCount = currentProjectileCount + ownerStats.bonusProjectileCount;

        // Distribute projectiles evenly in a circle (e.g., 360 / 4 = 90 degrees)
        float angleStep = 360f / totalCount;

        for (int i = 0; i < totalCount; i++)
        {
            GameObject projObject = PoolManager.Instance.GetFromPool(moonData.projectilePrefab.name);
            if (projObject == null) continue;
            projObject.transform.position = ownerStats.transform.position;

            MoonLogic logic = projObject.GetComponent<MoonLogic>();

            if (logic != null)
            {
                float startAngle = i * angleStep;
                float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;

                // Add bonus duration from passives
                float finalDuration = currentDuration * (1f + ownerStats.bonusDuration);

                logic.Initialize(
                    finalDamage,
                    finalDuration,
                    currentRotationSpeed,
                    currentRotationRadius,
                    ownerStats.transform,
                    startAngle,
                    moonData.knockback,
                    skillData.hitSound
                );
            }
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: ApplyStats(moonData.level2_DamageBonus, moonData.level2_RotationSpeedBonus, moonData.level2_DurationBonus, moonData.level2_RadiusBonus, moonData.level2_ProjectileCountBonus); break;
            case 3: ApplyStats(moonData.level3_DamageBonus, moonData.level3_RotationSpeedBonus, moonData.level3_DurationBonus, moonData.level3_RadiusBonus, moonData.level3_ProjectileCountBonus); break;
            case 4: ApplyStats(moonData.level4_DamageBonus, moonData.level4_RotationSpeedBonus, moonData.level4_DurationBonus, moonData.level4_RadiusBonus, moonData.level4_ProjectileCountBonus); break;
            case 5: ApplyStats(moonData.level5_DamageBonus, moonData.level5_RotationSpeedBonus, moonData.level5_DurationBonus, moonData.level5_RadiusBonus, moonData.level5_ProjectileCountBonus); break;
            case 6: ApplyStats(moonData.level6_DamageBonus, moonData.level6_RotationSpeedBonus, moonData.level6_DurationBonus, moonData.level6_RadiusBonus, moonData.level6_ProjectileCountBonus); break;
            case 7: ApplyStats(moonData.level7_DamageBonus, moonData.level7_RotationSpeedBonus, moonData.level7_DurationBonus, moonData.level7_RadiusBonus, moonData.level7_ProjectileCountBonus); break;
            case 8: ApplyStats(moonData.level8_DamageBonus, moonData.level8_RotationSpeedBonus, moonData.level8_DurationBonus, moonData.level8_RadiusBonus, moonData.level8_ProjectileCountBonus); break;
            case 9: ApplyStats(moonData.level9_DamageBonus, moonData.level9_RotationSpeedBonus, moonData.level9_DurationBonus, moonData.level9_RadiusBonus, moonData.level9_ProjectileCountBonus); break;
        }
    }

    private void ApplyStats(float dmg, float speed, float duration, float radius, int count)
    {
        currentDamage += dmg;
        currentRotationSpeed += speed;
        currentDuration += duration;
        currentRotationRadius += radius;
        currentProjectileCount += count;
    }
}