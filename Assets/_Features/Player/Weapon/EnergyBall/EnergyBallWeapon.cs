using UnityEngine;

// This script manages the weapon itself.
public class EnergyBallWeapon : Weapon
{
    // A reference to the ScriptableObject containing this weapon's unique stats.
    private EnergyBallDataSO energyBallData;

    // The *current* number of [Enemy] bounces
    private int currentMaxBounces;

    // Initialize override to setup specific data
    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        base.Initialize(aimObj, owner, animator);

        if (weaponData is EnergyBallDataSO)
        {
            energyBallData = (EnergyBallDataSO)weaponData;
        }
        else
        {
            Debug.LogError("[EnergyBallWeapon] Wrong DataSO assigned!");
            return;
        }

        // Initialize specific stats
        this.currentMaxBounces = energyBallData.baseBounces;

        // Ensure projectile count starts at 1
        this.currentProjectileCount = 1;
    }

    // This function is called by the parent 'Weapon' class when the attack timer is ready.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        if (energyBallData.projectilePrefab == null) return;

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
            // Get a recycled projectile from the object pool instead of creating a new one.
            GameObject projectileObj = PoolManager.Instance.GetFromPool(energyBallData.projectilePrefab.name);
            if (projectileObj == null) continue;

            // Set the projectile's position to the 'aim' transform
            projectileObj.transform.position = aim.position;

            // Set rotation based on spread
            float currentAngle = startAngle + (i * spreadAngle);
            projectileObj.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            // Get the script component from the projectile object we just spawned.
            EnergyBallProjectile projectile = projectileObj.GetComponent<EnergyBallProjectile>();

            if (projectile != null)
            {
                // Calculate final stats with passives
                float finalDamage = GetFinalDamage(out bool isCrit);

                // Initial lifetime is used for the first shot
                float initialLifetime = GetFinalDuration(energyBallData.lifetime);

                // Call the projectile's Initialize method to set it up.
                projectile.Initialize(
                    finalDamage,
                    energyBallData.projectileSpeed,
                    currentMaxBounces,
                    energyBallData.bounceRange,
                    initialLifetime,
                    energyBallData.bounceSearchDuration,
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
            case 2: ApplyStats(energyBallData.level2_DamageBonus, energyBallData.level2_CooldownReduction, energyBallData.level2_BounceIncrease, energyBallData.level2_ProjectileCountIncrease); break;
            case 3: ApplyStats(energyBallData.level3_DamageBonus, energyBallData.level3_CooldownReduction, energyBallData.level3_BounceIncrease, energyBallData.level3_ProjectileCountIncrease); break;
            case 4: ApplyStats(energyBallData.level4_DamageBonus, energyBallData.level4_CooldownReduction, energyBallData.level4_BounceIncrease, energyBallData.level4_ProjectileCountIncrease); break;
            case 5: ApplyStats(energyBallData.level5_DamageBonus, energyBallData.level5_CooldownReduction, energyBallData.level5_BounceIncrease, energyBallData.level5_ProjectileCountIncrease); break;
            case 6: ApplyStats(energyBallData.level6_DamageBonus, energyBallData.level6_CooldownReduction, energyBallData.level6_BounceIncrease, energyBallData.level6_ProjectileCountIncrease); break;
            case 7: ApplyStats(energyBallData.level7_DamageBonus, energyBallData.level7_CooldownReduction, energyBallData.level7_BounceIncrease, energyBallData.level7_ProjectileCountIncrease); break;
            case 8: ApplyStats(energyBallData.level8_DamageBonus, energyBallData.level8_CooldownReduction, energyBallData.level8_BounceIncrease, energyBallData.level8_ProjectileCountIncrease); break;
            case 9: ApplyStats(energyBallData.level9_DamageBonus, energyBallData.level9_CooldownReduction, energyBallData.level9_BounceIncrease, energyBallData.level9_ProjectileCountIncrease); break;
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