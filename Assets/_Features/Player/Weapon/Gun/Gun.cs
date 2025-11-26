using UnityEngine;

public class Gun : Weapon
{
    // A private variable to hold our *specific* data.
    private GunDataSO gunData;

    // Gun-specific runtime stats
    private bool currentProjectilePenetration = false;

    // (1) [INITIALIZATION] [MODIFIED] Added 'PlayerAnimator animator' parameter
    // Replaces Awake() for dependency injection and data casting
    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        // MUST call base.Initialize first to set up common references (aim, ownerStats)
        // Pass 'animator' to the base method
        base.Initialize(aimObj, owner, animator);

        // Cast the generic 'weaponData' (from base class) into our specific 'gunData'.
        if (weaponData is GunDataSO)
        {
            gunData = (GunDataSO)weaponData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong WeaponDataSO assigned. Expected GunDataSO.");
        }

        // Initialize Gun-specific stats
        currentProjectilePenetration = false;

        // Use base count from SO if available, otherwise default to 1
        currentProjectileCount = gunData.baseProjectileCount > 0 ? gunData.baseProjectileCount : 1;
    }

    // PerformAttack (Now uses passed 'aimDirection' for accuracy, though aim.rotation is likely synced)
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // Get the prefab from 'gunData' (our specific SO)
        if (gunData.bulletPrefab == null) return;

        // Calculate base angle from aim direction
        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Calculate start angle for spread (centered)
        float startAngle = baseAngle;
        if (currentProjectileCount > 1)
        {
            float totalSpread = (currentProjectileCount - 1) * gunData.multiShotSpread;
            startAngle = baseAngle - (totalSpread / 2f);
        }

        for (int i = 0; i < currentProjectileCount; i++)
        {
            GameObject bulletObject = PoolManager.Instance.GetFromPool(gunData.bulletPrefab.name);
            if (bulletObject == null) continue;

            bulletObject.transform.position = aim.position;

            // Apply spread angle
            float currentAngle = startAngle + (i * gunData.multiShotSpread);
            bulletObject.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            Bullet bullet = bulletObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;

                // Initialize the bullet
                bullet.Initialize(
                    finalDamage,
                    weaponData.knockback, // 'knockback' is a common stat from base 'weaponData'
                    currentProjectilePenetration, // 'penetration' is a Gun-specific runtime stat
                    ownerStats.transform, // Pass the owner's transform as the attack source
                    weaponData.hitSound // [NEW] Pass hit sound
                );
            }
        }
    }

    // [Core Logic] Implement the level-up logic
    protected override void ApplyLevelUpStats()
    {
        // 'currentLevel' was already incremented by the base Weapon class
        switch (currentLevel)
        {
            case 2:
                ApplyStats(gunData.level2_DamageBonus, gunData.level2_ProjectileCountBonus, gunData.level2_CooldownReduction);
                break;
            case 3:
                ApplyStats(gunData.level3_DamageBonus, gunData.level3_ProjectileCountBonus, gunData.level3_CooldownReduction);
                break;
            case 4:
                ApplyStats(gunData.level4_DamageBonus, gunData.level4_ProjectileCountBonus, gunData.level4_CooldownReduction);
                break;
            case 5:
                ApplyStats(gunData.level5_DamageBonus, gunData.level5_ProjectileCountBonus, gunData.level5_CooldownReduction);
                break;
            case 6:
                ApplyStats(gunData.level6_DamageBonus, gunData.level6_ProjectileCountBonus, gunData.level6_CooldownReduction);
                break;
            case 7:
                ApplyStats(gunData.level7_DamageBonus, gunData.level7_ProjectileCountBonus, gunData.level7_CooldownReduction);
                break;
            case 8:
                ApplyStats(gunData.level8_DamageBonus, gunData.level8_ProjectileCountBonus, gunData.level8_CooldownReduction);
                break;
            case 9: // Max Level
                ApplyStats(gunData.level9_DamageBonus, gunData.level9_ProjectileCountBonus, gunData.level9_CooldownReduction);

                // [Unlock Penetration]
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
    private void ApplyStats(float dmgBonus, int countBonus, float cooldownPercent)
    {
        currentDamage += dmgBonus;
        currentProjectileCount += countBonus;

        // Calculate reduction amount based on percentage of current cooldown
        float reductionAmount = currentAttackCooldown * (cooldownPercent / 100f);
        currentAttackCooldown -= reductionAmount;
    }
}