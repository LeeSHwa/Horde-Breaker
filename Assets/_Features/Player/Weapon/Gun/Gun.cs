using UnityEngine;

public class Gun : Weapon2
{
    // A private variable to hold our *specific* data.
    private GunDataSO gunData;

    // Gun-specific runtime stats
    private bool currentProjectilePenetration = false;

    // We use Awake() to get references and cast our data.
    protected override void Awake()
    {
        base.Awake(); // Run the parent's Awake()

        // Cast the generic 'weaponData' (from base class)
        // into our specific 'gunData'.
        if (weaponData is GunDataSO)
        {
            gunData = (GunDataSO)weaponData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong WeaponDataSO assigned. Expected GunDataSO.");
        }
    }

    // PerformAttack (Now references the specific 'gunData')
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // [Core] Get the prefab from 'gunData' (our specific SO)
        // We use the prefab's name as the Tag for the PoolManager.
        // (This requires the PoolManager Tag to match the prefab name)
        GameObject bulletObject = PoolManager.Instance.GetFromPool(gunData.bulletPrefab.name);

        if (bulletObject == null) return;

        bulletObject.transform.position = aim.position;
        bulletObject.transform.rotation = aim.rotation;

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Calculate final damage
            float finalDamage = currentDamage * ownerStats.currentDamageMultiplier;

            // Initialize the bullet
            bullet.Initialize(
                finalDamage,
                weaponData.knockback, // 'knockback' is a common stat from base 'weaponData'
                currentProjectilePenetration, // 'penetration' is a Gun-specific runtime stat
                ownerStats.transform // [New] Pass the owner's transform as the attack source
            );
        }
    }

    // [Core Logic] Implement the level-up logic
    // This function READS data from 'gunData'
    protected override void ApplyLevelUpStats()
    {
        // 'currentLevel' was already incremented by the base Weapon class
        switch (currentLevel)
        {
            case 2:
                // Read Lvl 2 data from our SO
                currentDamage += gunData.level2_DamageBonus;
                break;
            case 3:
                // Read Lvl 3 data from our SO
                currentProjectileCount = gunData.level3_ProjectileCount;
                break;
            case 4:
                // Read Lvl 4 data from our SO
                currentAttackCooldown -= gunData.level4_CooldownReduction;
                break;
            case 5:
                // Read Lvl 5 data from our SO
                currentProjectilePenetration = gunData.level5_UnlocksPenetration;
                break;
        }
    }

    // [Core Logic] Initialize Gun-specific stats
    protected override void InitializeStats()
    {
        base.InitializeStats(); // Runs base logic (resets damage, cooldown from base SO)

        // Reset Gun-specific stats for Level 1
        currentProjectilePenetration = false;
        // 'currentProjectileCount' is already managed by base Weapon.cs
    }
}