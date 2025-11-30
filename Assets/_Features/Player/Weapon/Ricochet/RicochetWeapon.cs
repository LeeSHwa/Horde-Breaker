using UnityEngine;

// This script manages the weapon itself.
public class RicochetWeapon : Weapon
{
    // A reference to the ScriptableObject containing this weapon's unique stats.
    private RicochetDataSO ricochetData;

    // The *current* number of [Enemy] bounces
    private int currentMaxBounces;

    // Initialize override to setup specific data
    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        base.Initialize(aimObj, owner, animator);

        if (weaponData is RicochetDataSO)
        {
            ricochetData = (RicochetDataSO)weaponData;
        }
        else
        {
            Debug.LogError("[RicochetWeapon] Wrong DataSO assigned!");
            return;
        }

        // Initialize Ricochet specific stats
        this.currentMaxBounces = ricochetData.baseBounces;

        // Ensure projectile count starts at 1
        this.currentProjectileCount = 1;
    }

    // This function is called by the parent 'Weapon' class when the attack timer is ready.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        if (ricochetData.projectilePrefab == null) return;

        // Get total projectile count (Base + Passive + Level Up)
        int count = GetFinalProjectileCount();

        // Calculate spread logic for multi-shot (Fan shape)
        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle;
        float spreadAngle = 15f; // Angle between projectiles

        if (count > 1)
        {
            float totalSpread = (count - 1) * spreadAngle;
            startAngle = baseAngle - (totalSpread / 2f);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject projectileObj = PoolManager.Instance.GetFromPool(ricochetData.projectilePrefab.name);
            if (projectileObj == null) continue;

            projectileObj.transform.position = aim.position;

            // Set rotation based on spread
            float currentAngle = startAngle + (i * spreadAngle);
            projectileObj.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            RicochetProjectile projectile = projectileObj.GetComponent<RicochetProjectile>();

            if (projectile != null)
            {
                float finalDamage = GetFinalDamage(out bool isCrit);
                float finalLifetime = GetFinalDuration(ricochetData.lifetime);

                // Call the projectile's Initialize method to set it up.
                projectile.Initialize(
                    finalDamage,
                    ricochetData.projectileSpeed,
                    currentMaxBounces,
                    ricochetData.bounceRange,
                    finalLifetime,
                    ownerStats.transform,
                    weaponData.hitSound,
                    isCrit
                );
            }
        }
    }

    // Called by the parent 'Weapon' class when the weapon levels up.
    protected override void ApplyLevelUpStats()
    {
        // Apply stats based on current level using the flexible data structure
        switch (currentLevel)
        {
            case 2: ApplyStats(ricochetData.level2_DamageBonus, ricochetData.level2_CooldownReduction, ricochetData.level2_BounceIncrease, ricochetData.level2_ProjectileCountIncrease); break;
            case 3: ApplyStats(ricochetData.level3_DamageBonus, ricochetData.level3_CooldownReduction, ricochetData.level3_BounceIncrease, ricochetData.level3_ProjectileCountIncrease); break;
            case 4: ApplyStats(ricochetData.level4_DamageBonus, ricochetData.level4_CooldownReduction, ricochetData.level4_BounceIncrease, ricochetData.level4_ProjectileCountIncrease); break;
            case 5: ApplyStats(ricochetData.level5_DamageBonus, ricochetData.level5_CooldownReduction, ricochetData.level5_BounceIncrease, ricochetData.level5_ProjectileCountIncrease); break;
            case 6: ApplyStats(ricochetData.level6_DamageBonus, ricochetData.level6_CooldownReduction, ricochetData.level6_BounceIncrease, ricochetData.level6_ProjectileCountIncrease); break;
            case 7: ApplyStats(ricochetData.level7_DamageBonus, ricochetData.level7_CooldownReduction, ricochetData.level7_BounceIncrease, ricochetData.level7_ProjectileCountIncrease); break;
            case 8: ApplyStats(ricochetData.level8_DamageBonus, ricochetData.level8_CooldownReduction, ricochetData.level8_BounceIncrease, ricochetData.level8_ProjectileCountIncrease); break;
            case 9: ApplyStats(ricochetData.level9_DamageBonus, ricochetData.level9_CooldownReduction, ricochetData.level9_BounceIncrease, ricochetData.level9_ProjectileCountIncrease); break;
        }

        // Safety cap for cooldown
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
    }

    private void ApplyStats(float dmg, float cdPercent, int bounce, int projCount)
    {
        currentDamage += dmg;
        currentMaxBounces += bounce;
        currentProjectileCount += projCount;

        float reduction = currentAttackCooldown * (cdPercent / 100f);
        currentAttackCooldown -= reduction;
    }
}