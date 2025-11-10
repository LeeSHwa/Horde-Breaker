// RicochetWeapon.cs
using UnityEngine;

// This class manages the logic for the Ricochet weapon, inheriting from Weapon.
// Its main job is to spawn a projectile; the projectile itself handles the bouncing logic.
public class RicochetWeapon : Weapon
{
    // Caches the Ricochet-specific ScriptableObject data.
    private RicochetDataSO ricochetData;

    // 'ownerStats' (from the base Weapon class) is set during base.Initialize.

    // (1) Override Initialize to handle Ricochet-specific data
    public override void Initialize(Transform aimObj, StatsController owner)
    {
        // MUST call base.Initialize first to set up common stats (damage, cooldown, etc.)
        base.Initialize(aimObj, owner);

        // Cast the generic 'weaponData' to the specific 'RicochetDataSO' type.
        ricochetData = (RicochetDataSO)weaponData;
    }

    // (2) Called by the Weapon base class when the attack cooldown is ready.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // 1. Get a recycled projectile object from the PoolManager.
        // (This requires 'projectilePrefab' to be correctly set in the RicochetDataSO.)
        GameObject projectileObj = PoolManager.Instance.GetFromPool(ricochetData.projectilePrefab.name);

        // 2. Set the projectile's starting position and rotation based on the 'aim' transform.
        projectileObj.transform.position = aim.position;
        projectileObj.transform.rotation = aim.rotation; // Sets the firing direction

        // 3. Get the projectile's script component.
        RicochetProjectile projectile = projectileObj.GetComponent<RicochetProjectile>();

        // 4. Initialize the projectile with all the necessary stats.
        if (projectile != null)
        {
            // Pass the data from this weapon (Weapon.cs) and its SO (RicochetDataSO)
            // to the projectile, so it can act independently.
            projectile.Initialize(
                currentDamage, // The final calculated damage from the base Weapon class.
                ricochetData.projectileSpeed,
                ricochetData.maxBounces,
                ricochetData.bounceRange,
                ricochetData.lifetime,
                ownerStats.transform // Pass the owner (Player) as the attack source (for knockback, etc.)
            );
        }
    }

    // (3) Applies stat changes when the weapon levels up.
    protected override void ApplyLevelUpStats()
    {
        // This is where you would add level-up bonuses.
        // switch (currentLevel)
        // {
        //    case 3:
        //        currentPierce += 1; // Example
        //        ricochetData.maxBounces += 1; // Example
        //        break;
        // }
    }
}