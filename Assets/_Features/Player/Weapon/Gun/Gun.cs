using UnityEngine;

public class Gun : Weapon
{
    // A private variable to hold our *specific* data.
    private GunDataSO gunData;

    // Gun-specific runtime stats
    private bool currentProjectilePenetration = false;

    // (1) [INITIALIZATION] Override base.Initialize to handle Gun-specific setup
    // Replaces Awake() for dependency injection and data casting
    public override void Initialize(Transform aimObj, StatsController owner)
    {
        // MUST call base.Initialize first to set up common references (aim, ownerStats)
        base.Initialize(aimObj, owner);

        // Cast the generic 'weaponData' (from base class) into our specific 'gunData'.
        if (weaponData is GunDataSO)
        {
            gunData = (GunDataSO)weaponData;
        }
        else
        {
            Debug.LogError(gameObject.name + " has the wrong WeaponDataSO assigned. Expected GunDataSO.");
        }

        // Initialize Gun-specific stats (moved from InitializeStats for clarity, or keep it there)
        currentProjectilePenetration = false;
    }

    // PerformAttack (Now uses passed 'aimDirection' for accuracy, though aim.rotation is likely synced)
    protected override void PerformAttack(Vector2 aimDirection)
    {
        // [Core] Get the prefab from 'gunData' (our specific SO)
        GameObject bulletObject = PoolManager.Instance.GetFromPool(gunData.bulletPrefab.name);

        if (bulletObject == null) return;

        // Set position to the aim object's position
        bulletObject.transform.position = aim.position;

        // ★ Use aimDirection to determine rotation, ensuring accuracy even if aim.rotation is slightly off
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        bulletObject.transform.rotation = Quaternion.Euler(0, 0, angle);

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
                ownerStats.transform // Pass the owner's transform as the attack source
            );
        }
    }

    // [Core Logic] Implement the level-up logic
    protected override void ApplyLevelUpStats()
    {
        // 'currentLevel' was already incremented by the base Weapon class
        switch (currentLevel)
        {
            case 2:
                currentDamage += gunData.level2_DamageBonus;
                break;
            case 3:
                currentProjectileCount = gunData.level3_ProjectileCount;
                break;
            case 4:
                currentAttackCooldown -= gunData.level4_CooldownReduction;
                break;
            case 5:
                currentProjectilePenetration = gunData.level5_UnlocksPenetration;
                break;
        }
    }

    // (Optional) If you want to keep InitializeStats separate, you can.
    // But putting it in Initialize() is often cleaner when you have overrides.
    // If you remove this, make sure the logic is in Initialize() above.
    /*
    protected override void InitializeStats()
    {
        base.InitializeStats();
        currentProjectilePenetration = false;
    }
    */
}