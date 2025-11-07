using UnityEngine;

public class GroundZoneSkill : Skills // Inherits from Skills
{
    private GroundZoneDataSO zoneData;

    // Runtime stats
    protected float currentDuration;
    protected float currentArea;
    protected float currentDebuffPercent;

    protected override void Awake()
    {
        base.Awake(); // This now ONLY sets ownerStats

        // Cast the generic 'skillData' into our specific 'zoneData'
        if (skillData is GroundZoneDataSO)
        {
            zoneData = (GroundZoneDataSO)skillData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong SkillDataSO assigned.");
        }

        // [MOVED] Call InitializeStats() AFTER the 'zoneData' variable is set.
        InitializeStats();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats(); // Initializes baseDamage, baseAttackCooldown

        // This is now safe because 'zoneData' is no longer null
        currentDuration = zoneData.baseDuration;
        currentArea = zoneData.baseArea;
        currentDebuffPercent = zoneData.speedDebuffPercentage;
    }

    // ... (PerformAttack, ApplyLevelUpStats
    //     ... (이하 코드는 이전과 동일) ...
    protected override void PerformAttack()
    {
        GameObject zoneObject = PoolManager.Instance.GetFromPool(zoneData.zonePrefab.name);
        if (zoneObject == null) return;
        zoneObject.transform.position = ownerStats.transform.position;
        ZoneLogic logic = zoneObject.GetComponent<ZoneLogic>();
        if (logic != null)
        {
            float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;
            logic.Initialize(
                finalDamage,
                currentDuration,
                currentArea,
                currentDebuffPercent,
                ownerStats.transform
            );
        }
    }
    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                currentDamage += zoneData.level2_DamageIncrease;
                break;
            case 3:
                currentArea += zoneData.level3_AreaIncrease;
                break;
            case 4:
                currentDuration += zoneData.level4_DurationIncrease;
                break;
            case 5:
                currentDebuffPercent += zoneData.level5_DebuffIncrease;
                break;
        }
    }
}