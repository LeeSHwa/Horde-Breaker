using UnityEngine;

// [MODIFIED] Class name changed from RotatingSkill to MoonSkill
public class MoonSkill : Skills // Inherits from Skills
{
    // [MODIFIED] Changed type from RotatingSkillDataSO to MoonDataSO
    // [FIX] Renamed this variable to avoid hiding the parent's 'skillData'
    private MoonDataSO rotatingSkillData; // [NOTE] Variable name 'rotatingSkillData' is kept for minimal code changes

    // Runtime stats
    protected float currentDuration;
    protected float currentRotationSpeed;
    protected float currentRotationRadius;

    protected override void Awake()
    {
        base.Awake(); // This now ONLY sets ownerStats

        // [MODIFIED] Now casts to MoonDataSO
        // [FIX] Cast the PARENT'S 'skillData' to our specific variable
        if (skillData is MoonDataSO) // 'skillData' now refers to the parent's (Skills.skillData)
        {
            // Assign to our new, correctly named variable
            rotatingSkillData = (MoonDataSO)skillData;
        }
        else
        {
            // [MODIFIED] Error message updated
            Debug.LogError(gameObject.name + " has the wrong SkillDataSO assigned. Expected MoonDataSO.");
        }

        // [MOVED] Call InitializeStats() AFTER the 'rotatingSkillData' variable is set.
        InitializeStats();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats(); // Initializes baseDamage, baseAttackCooldown, currentProjectileCount(1)

        // [FIX] Read from the correctly casted variable
        // This is now safe because 'rotatingSkillData' is no longer null
        currentDuration = rotatingSkillData.baseDuration;
        currentRotationSpeed = rotatingSkillData.baseRotationSpeed;
        currentRotationRadius = rotatingSkillData.baseRotationRadius;
    }

    // ... (PerformAttack, ApplyLevelUpStats

    protected override void PerformAttack()
    {
        float angleStep = 360f / currentProjectileCount;
        for (int i = 0; i < currentProjectileCount; i++)
        {
            GameObject projObject = PoolManager.Instance.GetFromPool(rotatingSkillData.projectilePrefab.name);
            if (projObject == null) continue;
            projObject.transform.position = ownerStats.transform.position;

            // [MODIFIED] GetComponent call changed from RotatingLogic to MoonLogic
            MoonLogic logic = projObject.GetComponent<MoonLogic>();

            if (logic != null)
            {
                float startAngle = i * angleStep;
                float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;
                logic.Initialize(
                    finalDamage,
                    currentDuration,
                    currentRotationSpeed,
                    currentRotationRadius,
                    ownerStats.transform,
                    startAngle,
                    rotatingSkillData.knockback
                );
            }
        }
    }
    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                currentDamage += rotatingSkillData.level2_DamageIncrease;
                break;
            case 3:
                currentProjectileCount = rotatingSkillData.level3_ProjectileCount;
                break;
            case 4:
                currentRotationSpeed += rotatingSkillData.level4_RotationSpeedIncrease;
                break;
            case 5:
                currentRotationRadius += rotatingSkillData.level5_RadiusIncrease;
                break;
        }
    }
}