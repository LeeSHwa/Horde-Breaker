using UnityEngine;

public class Gun : Weapon
{
    // A private variable to hold our specific data.
    private GunDataSO gunData;

    // Gun-specific runtime stats
    private bool currentProjectilePenetration = false;
    private int currentPierceCount = 0;

    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        base.Initialize(aimObj, owner, animator);

        if (weaponData is GunDataSO)
        {
            gunData = (GunDataSO)weaponData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong WeaponDataSO assigned. Expected GunDataSO.");
        }

        currentProjectilePenetration = false;

        // Initialize penetration count from base stats
        currentPierceCount = gunData.basePenetrationCount;

        // Use base count from SO if available, otherwise default to 1
        currentProjectileCount = gunData.baseProjectileCount > 0 ? gunData.baseProjectileCount : 1;
    }

    // PerformAttack
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // Get the prefab from 'gunData'
        if (gunData.bulletPrefab == null) return;

        int count = GetFinalProjectileCount();

        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        float startAngle = baseAngle;

        if (count > 1)
        {
            float totalSpread = (count - 1) * gunData.multiShotSpread;
            startAngle = baseAngle - (totalSpread / 2f);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject bulletObject = PoolManager.Instance.GetFromPool(gunData.bulletPrefab.name);
            if (bulletObject == null) continue;

            bulletObject.transform.position = aim.position;

            float currentAngle = startAngle + (i * gunData.multiShotSpread);
            bulletObject.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            Bullet bullet = bulletObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                float finalDamage = GetFinalDamage(out bool isCrit);
                Vector3 finalScaleVector = Vector3.one * GetFinalAreaScale();

                float finalDuration = GetFinalDuration(bullet.lifetime);

                bullet.Initialize(
                                finalDamage,
                                weaponData.knockback,
                                currentProjectilePenetration,
                                currentPierceCount,
                                ownerStats.transform,
                                finalScaleVector,
                                finalDuration,
                                weaponData.hitSound,
                                isCrit
                                );
            }
        }
    }

    // Implement the level-up logic
    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2:
                ApplyStats(gunData.level2_DamageBonus, gunData.level2_ProjectileCountBonus, gunData.level2_CooldownReduction, gunData.level2_PenetrationBonus);
                break;
            case 3:
                ApplyStats(gunData.level3_DamageBonus, gunData.level3_ProjectileCountBonus, gunData.level3_CooldownReduction, gunData.level3_PenetrationBonus);
                break;
            case 4:
                ApplyStats(gunData.level4_DamageBonus, gunData.level4_ProjectileCountBonus, gunData.level4_CooldownReduction, gunData.level4_PenetrationBonus);
                break;
            case 5:
                ApplyStats(gunData.level5_DamageBonus, gunData.level5_ProjectileCountBonus, gunData.level5_CooldownReduction, gunData.level5_PenetrationBonus);
                break;
            case 6:
                ApplyStats(gunData.level6_DamageBonus, gunData.level6_ProjectileCountBonus, gunData.level6_CooldownReduction, gunData.level6_PenetrationBonus);
                break;
            case 7:
                ApplyStats(gunData.level7_DamageBonus, gunData.level7_ProjectileCountBonus, gunData.level7_CooldownReduction, gunData.level7_PenetrationBonus);
                break;
            case 8:
                ApplyStats(gunData.level8_DamageBonus, gunData.level8_ProjectileCountBonus, gunData.level8_CooldownReduction, gunData.level8_PenetrationBonus);
                break;
            case 9:
                ApplyStats(gunData.level9_DamageBonus, gunData.level9_ProjectileCountBonus, gunData.level9_CooldownReduction, gunData.level9_PenetrationBonus);

                // Unlock Penetration
                if (gunData.level9_UnlockPenetration)
                {
                    currentProjectilePenetration = true;
                }
                break;
        }

        // Safety Cap
        if (currentAttackCooldown < 0.05f) currentAttackCooldown = 0.05f;
    }

    // Helper to reduce code duplication and apply percentage cooldown reduction
    private void ApplyStats(float dmgBonus, int countBonus, float cooldownPercent, int penBonus)
    {
        currentDamage += dmgBonus;
        currentProjectileCount += countBonus;
        currentPierceCount += penBonus;

        // Calculate reduction amount based on percentage of current cooldown
        float reductionAmount = currentAttackCooldown * (cooldownPercent / 100f);
        currentAttackCooldown -= reductionAmount;
    }
}