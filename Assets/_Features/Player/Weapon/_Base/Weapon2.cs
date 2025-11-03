using UnityEngine;

// Base class for all weapon logic (MonoBehaviours)
public abstract class Weapon2 : MonoBehaviour
{
    [Header("Data Source")]
    // (1) The COMMON data SO. This will be cast by the child.
    public WeaponDataSO weaponData;

    [Header("References")]
    public Transform aim;
    public float offset = 1.0f;

    [Header("Runtime Stats")]
    // (2) Stats common to all weapons
    protected int currentLevel = 1;
    protected float currentDamage;
    protected float currentAttackCooldown;
    protected int currentProjectileCount = 1; // Default
    protected float lastAttackTime;
    protected StatsController ownerStats;

    protected virtual void Awake()
    {
        // (3) Find the owner
        ownerStats = GetComponentInParent<StatsController>();
        if (ownerStats == null)
        {
            Debug.LogError("Weapon cannot find StatsController in parent!");
        }

        // (4) Initialize stats on Awake
        InitializeStats();
    }

    // (5) TryAttack logic is common
    public virtual void TryAttack(Vector2 aimDirection)
    {
        if (Time.time >= lastAttackTime + currentAttackCooldown)
        {
            aim.position = (Vector2)transform.position + aimDirection * offset;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aim.rotation = Quaternion.Euler(0, 0, angle);

            PerformAttack(aimDirection);
            lastAttackTime = Time.time;
        }
    }

    // (6) [ABSTRACT] The child (Gun.cs) MUST implement this
    protected abstract void PerformAttack(Vector2 aimDirection);

    // (7) [Core API] Level up logic (called by Level Up UI)
    public virtual void LevelUp()
    {
        if (currentLevel >= 5) return;

        currentLevel++;
        ApplyLevelUpStats(); // Call the child's specific logic
    }

    // (8) [ABSTRACT] The child (Gun.cs) MUST implement this
    protected abstract void ApplyLevelUpStats();

    // (9) [Core Logic] Initialize common stats from the base SO
    protected virtual void InitializeStats()
    {
        currentLevel = 1;
        currentDamage = weaponData.baseDamage;
        currentAttackCooldown = weaponData.baseAttackCooldown;
        currentProjectileCount = 1; // Reset to 1
        lastAttackTime = 0;
    }
}