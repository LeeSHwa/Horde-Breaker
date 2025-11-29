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
    }

    protected override void PerformAttack()
    {
        // Calculate total count including passives
        int totalCount = currentProjectileCount + ownerStats.bonusProjectileCount;

        // Distribute projectiles evenly in a circle
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
            case 2:
                currentDamage += moonData.level2_DamageIncrease;
                break;
            case 3:
                currentProjectileCount = moonData.level3_ProjectileCount;
                break;
            case 4:
                currentRotationSpeed += moonData.level4_RotationSpeedIncrease;
                break;
            case 5:
                currentRotationRadius += moonData.level5_RadiusIncrease;
                currentProjectileCount = moonData.level5_ProjectileCount;
                break;
        }
    }
}