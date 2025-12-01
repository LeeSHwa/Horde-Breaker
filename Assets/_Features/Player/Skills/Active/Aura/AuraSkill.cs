using UnityEngine;

public class AuraSkill : Skills
{
    private AuraDataSO zoneData;

    // Runtime stats
    protected float currentDuration;
    protected float currentArea;
    protected float currentDebuffPercent;

    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        // Cast to AuraDataSO
        if (skillData is AuraDataSO)
        {
            zoneData = (AuraDataSO)skillData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong SkillDataSO assigned. Expected AuraDataSO.");
            return;
        }

        currentDuration = zoneData.baseDuration;
        currentArea = zoneData.baseArea;
        currentDebuffPercent = zoneData.baseSpeedDebuffPercentage;
    }

    protected override void PerformAttack()
    {
        GameObject zoneObject = PoolManager.Instance.GetFromPool(zoneData.zonePrefab.name);
        if (zoneObject == null) return;
        zoneObject.transform.position = ownerStats.transform.position;

        AuraLogic logic = zoneObject.GetComponent<AuraLogic>();

        if (logic != null)
        {
            float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;

            // Apply area bonus from passives
            float finalArea = currentArea + ownerStats.bonusArea;

            // Apply duration bonus from passives
            float finalDuration = currentDuration * (1f + ownerStats.bonusDuration);

            logic.Initialize(
                finalDamage,
                finalDuration,
                finalArea,
                currentDebuffPercent,
                ownerStats.transform,
                zoneData.slowHitSound
            );
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: ApplyStats(zoneData.level2_DamageBonus, zoneData.level2_AreaBonus, zoneData.level2_DurationBonus, zoneData.level2_DebuffBonus); break;
            case 3: ApplyStats(zoneData.level3_DamageBonus, zoneData.level3_AreaBonus, zoneData.level3_DurationBonus, zoneData.level3_DebuffBonus); break;
            case 4: ApplyStats(zoneData.level4_DamageBonus, zoneData.level4_AreaBonus, zoneData.level4_DurationBonus, zoneData.level4_DebuffBonus); break;
            case 5: ApplyStats(zoneData.level5_DamageBonus, zoneData.level5_AreaBonus, zoneData.level5_DurationBonus, zoneData.level5_DebuffBonus); break;
            case 6: ApplyStats(zoneData.level6_DamageBonus, zoneData.level6_AreaBonus, zoneData.level6_DurationBonus, zoneData.level6_DebuffBonus); break;
            case 7: ApplyStats(zoneData.level7_DamageBonus, zoneData.level7_AreaBonus, zoneData.level7_DurationBonus, zoneData.level7_DebuffBonus); break;
            case 8: ApplyStats(zoneData.level8_DamageBonus, zoneData.level8_AreaBonus, zoneData.level8_DurationBonus, zoneData.level8_DebuffBonus); break;
            case 9: ApplyStats(zoneData.level9_DamageBonus, zoneData.level9_AreaBonus, zoneData.level9_DurationBonus, zoneData.level9_DebuffBonus); break;
        }
    }

    private void ApplyStats(float dmg, float area, float duration, float debuff)
    {
        // All stats are applied as flat values
        currentDamage += dmg;
        currentArea += area;
        currentDuration += duration;
        currentDebuffPercent += debuff;
    }
    protected override void InitializeStats()
    {
        base.InitializeStats();

        if (zoneData != null)
        {
            currentDuration = zoneData.baseDuration;
            currentArea = zoneData.baseArea;
            currentDebuffPercent = zoneData.baseSpeedDebuffPercentage;
        }
    }
}