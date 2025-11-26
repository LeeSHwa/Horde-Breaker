using UnityEngine;
using System.Collections;

// Class name changed from MissileSkill to HellfireSkill
public class HellfireSkill : Skills
{
    private HellfireDataSO hellfireData;

    // Runtime Stats
    private float currentSearchRadius;
    private float currentSpeed;

    protected override void Awake()
    {
        base.Awake();

        // Cast to HellfireDataSO
        if (skillData is HellfireDataSO)
        {
            hellfireData = (HellfireDataSO)skillData;
        }
        else
        {
            Debug.LogError("HellfireSkill has wrong DataSO! Expected HellfireDataSO.");
        }

        InitializeStats();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();
        currentSearchRadius = hellfireData.searchRadius;
        currentSpeed = hellfireData.missileSpeed;
    }

    protected override void PerformAttack()
    {
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        for (int i = 0; i < currentProjectileCount; i++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnProjectile()
    {
        // Make sure the prefab name in PoolManager matches
        GameObject obj = PoolManager.Instance.GetFromPool(hellfireData.missilePrefab.name);
        if (obj == null) return;

        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;

        // Get HellfireLogic component
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