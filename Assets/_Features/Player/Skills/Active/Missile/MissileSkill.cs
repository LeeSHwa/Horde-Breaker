using UnityEngine;
using System.Collections;

public class MissileSkill : Skills
{
    private MissileDataSO missileData;

    // Runtime Stats
    private float currentSearchRadius;
    private float currentSpeed;

    protected override void Awake()
    {
        base.Awake();

        if (skillData is MissileDataSO)
        {
            missileData = (MissileDataSO)skillData;
        }
        else
        {
            Debug.LogError("MissileSkill has wrong DataSO!");
        }

        InitializeStats();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();
        currentSearchRadius = missileData.searchRadius;
        currentSpeed = missileData.missileSpeed;
    }

    protected override void PerformAttack()
    {
        StartCoroutine(FireMissilesRoutine());
    }

    private IEnumerator FireMissilesRoutine()
    {
        // Simply fire N missiles. They will find their own targets.
        for (int i = 0; i < currentProjectileCount; i++)
        {
            SpawnMissile();
            yield return new WaitForSeconds(0.1f); // Small delay between shots
        }
    }

    private void SpawnMissile()
    {
        GameObject missileObj = PoolManager.Instance.GetFromPool(missileData.missilePrefab.name);
        if (missileObj == null) return;

        // Start at player's position
        missileObj.transform.position = transform.position;

        // [Important] Reset rotation to prevent flying backwards if pooled
        missileObj.transform.rotation = Quaternion.identity;

        MissileLogic logic = missileObj.GetComponent<MissileLogic>();
        if (logic != null)
        {
            float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;

            // [Modified] Pass 'currentSearchRadius' instead of a specific target
            logic.Initialize(finalDamage, currentSpeed, currentSearchRadius);
        }
    }

    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                currentDamage += missileData.level2_DamageIncrease;
                break;
            case 3:
                currentProjectileCount = missileData.level3_ProjectileCount;
                break;
            case 4:
                currentAttackCooldown -= missileData.level4_CooldownReduction;
                if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
                break;
            case 5:
                currentProjectileCount = missileData.level5_ProjectileCount;
                break;
        }
    }
}
