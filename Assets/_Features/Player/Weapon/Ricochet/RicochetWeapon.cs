using UnityEngine;

// This script manages the weapon itself.
// Its main role is to spawn projectiles and pass the correct stats to them.
public class RicochetWeapon : Weapon
{
    // A reference to the ScriptableObject containing this weapon's unique stats.
    private RicochetDataSO ricochetData;

    // The *current* number of [Enemy] bounces, which can be modified by level-ups.
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

        // Ensure projectile count starts at 1 (or value from SO if added)
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
            // Get a recycled projectile from the object pool instead of creating a new one.
            GameObject projectileObj = PoolManager.Instance.GetFromPool(ricochetData.projectilePrefab.name);
            if (projectileObj == null) continue;

            // Set the projectile's position to the 'aim' transform (e.g., player's hand or gun muzzle).
            projectileObj.transform.position = aim.position;

            // Set rotation based on spread
            float currentAngle = startAngle + (i * spreadAngle);
            projectileObj.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            // Get the script component from the projectile object we just spawned.
            RicochetProjectile projectile = projectileObj.GetComponent<RicochetProjectile>();

            // If the script was found successfully, pass all necessary stats to it.
            if (projectile != null)
            {
                // Calculate final stats with passives
                float finalDamage = GetFinalDamage(out bool isCrit);
                float finalLifetime = GetFinalDuration(ricochetData.lifetime);

                // Call the projectile's Initialize method to set it up.
                projectile.Initialize(
                    finalDamage,
                    ricochetData.projectileSpeed,
                    currentMaxBounces,
                    ricochetData.bounceRange,
                    finalLifetime,
                    ownerStats.transform,    // Pass the player (owner) as the attack source
                    weaponData.hitSound,     // Pass hit sound
                    isCrit                   // Pass crit status
                );
            }
        }
    }

    // Called by the parent 'Weapon' class when the weapon levels up.
    protected override void ApplyLevelUpStats()
    {
        switch (currentLevel)
        {
            case 2: // Damage +2
                currentDamage += ricochetData.level2_DamageBonus;
                break;
            case 3: // Cooldown -10%, Bounce +1
                ApplyCooldownReduction(ricochetData.level3_CooldownReduction);
                currentMaxBounces += ricochetData.level3_BounceIncrease;
                break;
            case 4: // Damage +4
                currentDamage += ricochetData.level4_DamageBonus;
                break;
            case 5: // Projectile +1
                currentProjectileCount += ricochetData.level5_ProjectileCountIncrease;
                break;
            case 6: // Cooldown -10%, Bounce +1
                ApplyCooldownReduction(ricochetData.level6_CooldownReduction);
                currentMaxBounces += ricochetData.level6_BounceIncrease;
                break;
            case 7: // Damage +10
                currentDamage += ricochetData.level7_DamageBonus;
                break;
            case 8: // Projectile +1
                currentProjectileCount += ricochetData.level8_ProjectileCountIncrease;
                break;
            case 9: // Projectile +1
                currentProjectileCount += ricochetData.level9_ProjectileCountIncrease;
                break;
        }

        // Safety cap for cooldown
        if (currentAttackCooldown < 0.1f) currentAttackCooldown = 0.1f;
    }

    // Helper for percentage cooldown reduction
    private void ApplyCooldownReduction(float percent)
    {
        float reduction = currentAttackCooldown * (percent / 100f);
        currentAttackCooldown -= reduction;
    }
}