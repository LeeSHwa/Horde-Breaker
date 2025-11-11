// RicochetWeapon.cs
using UnityEngine;

// This class manages the logic for the Ricochet weapon, inheriting from Weapon.
// Its main job is to spawn a projectile; the projectile itself handles the bouncing logic.
public class RicochetWeapon : Weapon
{
    // Caches the Ricochet-specific ScriptableObject data.
    private RicochetDataSO ricochetData;

    // 'ownerStats' (from the base Weapon class) is set during base.Initialize.
    // [MODIFIED] 'playerAnimator' is also now set in the base class.

    // (1) [MODIFIED] Override Initialize to handle Ricochet-specific data
    // The signature MUST match the new base.Initialize signature.
    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        // MUST call base.Initialize first to set up common stats (damage, cooldown, etc.)
        // [MODIFIED] Pass 'animator' to the base method as well.
        base.Initialize(aimObj, owner, animator);

        // Cast the generic 'weaponData' to the specific 'RicochetDataSO' type.
        // This part remains the same.
        if (weaponData is RicochetDataSO)
        {
            ricochetData = (RicochetDataSO)weaponData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong WeaponDataSO assigned. Expected RicochetDataSO.");
        }
    }

    // (2) Called by the Weapon base class when the attack cooldown is ready.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // 1. Get a recycled projectile object from the PoolManager.
        // (This requires 'projectilePrefab' to be correctly set in the RicochetDataSO.)

        // [NOTE] Check if ricochetData is null, in case Initialize failed
        if (ricochetData == null || ricochetData.projectilePrefab == null)
        {
            Debug.LogError("RicochetDataSO or its projectilePrefab is not set!");
            return;
        }

        GameObject projectileObj = PoolManager.Instance.GetFromPool(ricochetData.projectilePrefab.name);
        if (projectileObj == null) return; // PoolManager might return null

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
        // 'currentLevel' is already incremented by the base class
        // This is where you would add level-up bonuses.
        // switch (currentLevel)
        // {
        //    case 2:
        //        currentDamage += 5; // Example
        //        break;
        //    case 3:
        //        ricochetData.maxBounces += 1; // Example
        //        break;
        // }
    }
}