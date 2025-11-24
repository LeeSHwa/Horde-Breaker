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
        // [MODIFIED] Set DebuffPercent based on the SO data (likely 0 if Lvl 4 adds it)
        currentDebuffPercent = zoneData.speedDebuffPercentage;
    }

    // ... (ApplyLevelUpStats is updated as per recent logic)

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

            // [MODIFIED] Pass 'zoneData.slowHitSound' as the last argument
            logic.Initialize(
                finalDamage,
                currentDuration,
                currentArea,
                currentDebuffPercent,
                ownerStats.transform,
                zoneData.slowHitSound // [NEW] Pass the sound clip
            );
        }
    }

    // [MOD..."ApplyLevelUpStats" METHOD FULLY REPLACED]...
    protected override void ApplyLevelUpStats()
    {
        // [NEW] This function's logic is updated for the new Level 4 (Slow) and Level 5 (Mastery) design.
        switch (currentLevel)
        {
            case 2: // Level 2: Damage Increase (Unchanged)
                currentDamage += zoneData.level2_DamageIncrease;
                break;
            case 3: // Level 3: Area Increase (Unchanged)
                currentArea += zoneData.level3_AreaIncrease;
                break;
            case 4: // Level 4: [NEW] Apply Slow Effect
                // Set the slow percentage directly from the SO's new 'level4_SlowValue' field.
                currentDebuffPercent = zoneData.level4_SlowValue;
                break;
            case 5: // Level 5: [NEW] Apply Mastery (All stats up)
                // Apply Lvl 2 effect (Damage) again
                currentDamage += zoneData.level5_DamageIncrease;

                // Apply Lvl 3 effect (Area) again
                currentArea += zoneData.level5_AreaIncrease;

                // Apply Lvl 4 effect (Slow) again (adds a negative value)
                currentDebuffPercent += zoneData.level5_DebuffIncrease;
                break;
        }
    }
}