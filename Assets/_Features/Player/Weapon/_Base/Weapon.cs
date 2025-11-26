using Unity.VisualScripting;
using UnityEngine;

// Base class for all weapon logic (MonoBehaviours)
public abstract class Weapon : MonoBehaviour, AttackInterface
{
    [Header("Data Source")]
    // (1) The COMMON data SO. This will be cast by the child.
    public WeaponDataSO weaponData;

    [Header("References")]
    // Initialized via code, not Inspector
    protected Transform aim;
    public float offset = 1.0f;

    [Header("Runtime Stats")]
    // (2) Stats common to all weapons
    protected int currentLevel = 1;
    protected float currentDamage;
    protected float currentAttackCooldown;
    protected int currentProjectileCount = 1; // Default
    protected float lastAttackTime;

    // Added references for PlayerAnimator and StatsController
    protected StatsController ownerStats;
    protected PlayerAnimator playerAnimator; // Reference to control player animations

    public int CurrentLevel { get { return currentLevel; } }
    public int MaxLevel { get { return weaponData.maxLevel; } }

    public string GetName()
    {
        // Example: Get name from the Scriptable Object
        return weaponData.weaponName;
    }
    public string GetNextLevelDescription()
    {
        // currentLevel starts at 1. Level 2 description is at index 0.
        int index = currentLevel - 1;
        if (weaponData.levelDescriptions != null && index < weaponData.levelDescriptions.Count)
        {
            return weaponData.levelDescriptions[index];
        }
        return "Max Level Reached";
    }

    public Sprite GetIcon()
    {
        // Example: Get icon from the Scriptable Object
        return weaponData.icon;
    }

    // (3) [INITIALIZATION] Added 'PlayerAnimator animator' parameter
    public virtual void Initialize(Transform aimObj, StatsController owner, PlayerAnimator animator)
    {
        // Inject external dependencies
        this.aim = aimObj;
        this.ownerStats = owner;
        this.playerAnimator = animator; // [ADDED] Store the reference

        // Initialize runtime stats from SO
        currentLevel = 1;
        currentDamage = weaponData.baseDamage;
        currentAttackCooldown = weaponData.baseAttackCooldown;
        currentProjectileCount = 1;
        lastAttackTime = -currentAttackCooldown; // Ready to attack immediately
    }

    // (4) [COMMON LOGIC] Updates the 'aim' object's position and rotation
    public void UpdateAim(Vector2 aimDirection)
    {
        if (aim == null) return;

        // Position: Offset from player center towards aim direction
        aim.position = (Vector2)transform.position + aimDirection * offset;

        // Rotation: Rotate to face the aim direction
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aim.rotation = Quaternion.Euler(0, 0, angle);
    }

    // (5) TryAttack logic is common (cooldown check)
    public virtual void TryAttack(Vector2 aimDirection)
    {
        if (Time.time >= lastAttackTime + currentAttackCooldown)
        {
            PerformAttack(aimDirection);
            lastAttackTime = Time.time;

            // [NEW] Attack Sound Logic
            if (weaponData.attackSound != null)
            {
                // Pitch variation 0.1 added for natural feel
                SoundManager.Instance.PlaySFX(weaponData.attackSound, 0.1f);
            }
        }
    }

    // (6) [ABSTRACT] The child (Gun.cs, Sword.cs) MUST implement this specific attack logic
    protected abstract void PerformAttack(Vector2 aimDirection);

    // (7) [Core API] Level up logic (called by Level Up UI)
    public virtual void LevelUp()
    {
        if (currentLevel >= MaxLevel) return;

        currentLevel++;
        ApplyLevelUpStats(); // Call the child's specific logic
    }

    // (8) [ABSTRACT] The child MUST implement this specific upgrade logic
    protected abstract void ApplyLevelUpStats();
}