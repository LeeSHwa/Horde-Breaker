using UnityEngine;
using System.Collections;

// Class name changed from MissileSkill to HellfireSkill
public class HellfireSkill : Skills
{
    private HellfireDataSO hellfireData;

    // Runtime Stats
    private float currentSearchRadius;
    private float currentSpeed;

    public override void Initialize(StatsController owner)
    {
        base.Initialize(owner);

        // Cast to HellfireDataSO
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
            case 2: ApplyStats(hellfireData.level2_DamageBonus, hellfireData.level2_CooldownReduction, hellfireData.level2_ProjectileCountIncrease, hellfireData.level2_SpeedBonus); break;
            case 3: ApplyStats(hellfireData.level3_DamageBonus, hellfireData.level3_CooldownReduction, hellfireData.level3_ProjectileCountIncrease, hellfireData.level3_SpeedBonus); break;
            case 4: ApplyStats(hellfireData.level4_DamageBonus, hellfireData.level4_CooldownReduction, hellfireData.level4_ProjectileCountIncrease, hellfireData.level4_SpeedBonus); break;
            case 5: ApplyStats(hellfireData.level5_DamageBonus, hellfireData.level5_CooldownReduction, hellfireData.level5_ProjectileCountIncrease, hellfireData.level5_SpeedBonus); break;
            case 6: ApplyStats(hellfireData.level6_DamageBonus, hellfireData.level6_CooldownReduction, hellfireData.level6_ProjectileCountIncrease, hellfireData.level6_SpeedBonus); break;
            case 7: ApplyStats(hellfireData.level7_DamageBonus, hellfireData.level7_CooldownReduction, hellfireData.level7_ProjectileCountIncrease, hellfireData.level7_SpeedBonus); break;
            case 8: ApplyStats(hellfireData.level8_DamageBonus, hellfireData.level8_CooldownReduction, hellfireData.level8_ProjectileCountIncrease, hellfireData.level8_SpeedBonus); break;
            case 9: ApplyStats(hellfireData.level9_DamageBonus, hellfireData.level9_CooldownReduction, hellfireData.level9_ProjectileCountIncrease, hellfireData.level9_SpeedBonus); break;
        }

        // Safety cap for cooldown
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
    }

    private void ApplyStats(float dmg, float cooldown, int count, float speed)
    {
        // All stats are applied as flat values
        currentDamage += dmg;
        currentAttackCooldown -= cooldown;
        currentProjectileCount += count;
        currentSpeed += speed;
    }
}