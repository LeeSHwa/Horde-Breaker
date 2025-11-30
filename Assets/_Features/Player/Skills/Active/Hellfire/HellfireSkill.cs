using UnityEngine;
using System.Collections;

public class HellfireSkill : Skills
{
    private HellfireDataSO hellfireData;

    // Runtime Stats
    private float currentSearchRadius;
    private float currentSpeed;

    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        if (skillData is HellfireDataSO)
        {
            hellfireData = (HellfireDataSO)skillData;
        }
        else
        {
            Debug.LogError("HellfireSkill has wrong DataSO! Expected HellfireDataSO.");
            return;
        }

        currentSearchRadius = hellfireData.searchRadius;
        currentSpeed = hellfireData.missileSpeed;
    }

    protected override void PerformAttack()
    {
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        // Calculate total count including passives
        int totalCount = currentProjectileCount + ownerStats.bonusProjectileCount;

        for (int i = 0; i < totalCount; i++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnProjectile()
    {
        GameObject obj = PoolManager.Instance.GetFromPool(hellfireData.missilePrefab.name);
        if (obj == null) return;

        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;

        HellfireLogic logic = obj.GetComponent<HellfireLogic>();
        if (logic != null)
        {
            float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;
            logic.Initialize(finalDamage, currentSpeed, currentSearchRadius, skillData.hitSound);
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                currentDamage += hellfireData.level2_DamageIncrease;
                break;
            case 3:
                currentProjectileCount = hellfireData.level3_ProjectileCount;
                break;
            case 4:
                currentAttackCooldown -= hellfireData.level4_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5:
                currentProjectileCount = hellfireData.level5_ProjectileCount;
                break;
        }
    }
}