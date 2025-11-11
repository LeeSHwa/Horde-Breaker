// RicochetWeapon.cs
using UnityEngine;

// This script manages the weapon itself.
// Its main role is to spawn projectiles and pass the correct stats to them.
public class RicochetWeapon : Weapon
{
    // A reference to the ScriptableObject containing this weapon's unique stats.
    private RicochetDataSO ricochetData;

    // The *current* number of [Enemy] bounces, which can be modified by level-ups.
    private int currentMaxBounces;
    // (Wall bounce logic is no longer managed by the weapon)

    // Called when the weapon is first created or equipped.
    public override void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        // Call base.Initialize() to set up common properties
        // like the owner, damage, cooldown, etc.
        base.Initialize(aimObj, owner, animator);

        // Cast the parent class's 'weaponData' to the specific
        // 'RicochetDataSO' type to access its unique stats.
        ricochetData = (RicochetDataSO)weaponData;

        // Set the weapon's starting bounce count
        // based on the data from the ScriptableObject.
        this.currentMaxBounces = ricochetData.maxBounces;
    }

    // This function is called by the parent 'Weapon' class when the attack timer is ready.
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // Get a recycled projectile from the object pool instead of creating a new one.
        GameObject projectileObj = PoolManager.Instance.GetFromPool(ricochetData.projectilePrefab.name);

        // Set the projectile's position to the 'aim' transform (e.g., player's hand or gun muzzle).
        projectileObj.transform.position = aim.position;
        // Rotate the projectile to face the direction the player is aiming.
        projectileObj.transform.rotation = aim.rotation;

        // Get the script component from the projectile object we just spawned.
        RicochetProjectile projectile = projectileObj.GetComponent<RicochetProjectile>();

        // If the script was found successfully, pass all necessary stats to it.
        if (projectile != null)
        {
            // Call the projectile's Initialize method to set it up.
            // Only stats related to enemy bounces are needed now, not wall bounces.
            projectile.Initialize(
                currentDamage,          // The final calculated damage (from the base Weapon class)
                ricochetData.projectileSpeed,
                currentMaxBounces,      // The current (potentially leveled-up) enemy bounce count
                ricochetData.bounceRange,
                ricochetData.lifetime,
                ownerStats.transform    // Pass the player (owner) as the attack source
            );
        }
    }

    // Called by the parent 'Weapon' class when the weapon levels up.
    protected override void ApplyLevelUpStats()
    {
        // In this game's logic, "bounce count" now only refers to "enemy" bounce count.
        switch (currentLevel)
        {
            case 2:
                // As a level 2 bonus, increase the number of times it can bounce between enemies.
                currentMaxBounces += 1;
                break;
            case 4:
                // As a level 4 bonus, increase the weapon's damage.
                currentDamage *= 1.1f; // Example: 10% damage increase
                break;
        }
    }
}