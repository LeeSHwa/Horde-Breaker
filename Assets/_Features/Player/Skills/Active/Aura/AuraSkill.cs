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
        currentDebuffPercent = zoneData.speedDebuffPercentage;
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
            case 2:
                currentDamage += zoneData.level2_DamageIncrease;
                break;
            case 3:
                currentArea += zoneData.level3_AreaIncrease;
                break;
            case 4:
                currentDebuffPercent = zoneData.level4_SlowValue;
                break;
            case 5:
                currentDamage += zoneData.level5_DamageIncrease;
                currentArea += zoneData.level5_AreaIncrease;
                currentDebuffPercent += zoneData.level5_DebuffIncrease;
                break;
        }
    }
}