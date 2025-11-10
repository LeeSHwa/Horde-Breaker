using UnityEngine;

// [MODIFIED] Class name changed from GroundZoneSkill to AuraSkill
public class AuraSkill : Skills // Inherits from Skills
{
    // [MODIFIED] Changed type from GroundZoneDataSO to AuraDataSO
    private AuraDataSO zoneData;

    // Runtime stats
    protected float currentDuration;
    protected float currentArea;
    protected float currentDebuffPercent;

    protected override void Awake()
    {
        base.Awake(); // This now ONLY sets ownerStats

        // Cast the generic 'skillData' into our specific 'zoneData'
        // [MODIFIED] Now casts to AuraDataSO
        if (skillData is AuraDataSO)
        {
            zoneData = (AuraDataSO)skillData;
        }
        else
        {
            // [MODIFIED] Error message updated
            Debug.LogError(gameObject.name + " has the wrong SkillDataSO assigned. Expected AuraDataSO.");
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

    protected override void PerformAttack()
    {
        GameObject zoneObject = PoolManager.Instance.GetFromPool(zoneData.zonePrefab.name);
        if (zoneObject == null) return;
        zoneObject.transform.position = ownerStats.transform.position;

        // [MODIFIED] GetComponent call changed from ZoneLogic to AuraLogic
        AuraLogic logic = zoneObject.GetComponent<AuraLogic>();

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