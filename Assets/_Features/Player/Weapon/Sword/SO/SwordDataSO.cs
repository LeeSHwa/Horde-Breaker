using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordData", menuName = "Stats/Weapon Data/Sword Data")]
public class SwordDataSO : WeaponDataSO
{
    [Header("Sword Specific Base Stats")]
    public float baseSwingDuration = 0.5f;
    public float baseAngle = 90f;
    public float baseAreaRadius = 1.5f;
    public float swingStartOffset = 45f;

    [Header("Projectile Settings (Unlocked via Level Up)")]
    public int baseAttacksPerProjectile = 5;
    public GameObject projectilePrefab;
    public float projectileDamagePercent = 0.5f;
    public float projectileKnockbackPercent = 0.5f;
    public float projectileLifetime = 3f;
    public float projectileArcScaleMultiplier = 1.0f;

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_CooldownReduction; // Percent
    public float level2_AreaBonus;
    public float level2_AngleBonus;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_CooldownReduction;
    public float level3_AreaBonus;
    public float level3_AngleBonus;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_CooldownReduction;
    public float level4_AreaBonus;
    public float level4_AngleBonus;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_CooldownReduction;
    public float level5_AreaBonus;
    public float level5_AngleBonus;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_CooldownReduction;
    public float level6_AreaBonus;
    public float level6_AngleBonus;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_CooldownReduction;
    public float level7_AreaBonus;
    public float level7_AngleBonus;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_CooldownReduction;
    public float level8_AreaBonus;
    public float level8_AngleBonus;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_CooldownReduction;
    public float level9_AreaBonus;
    public float level9_AngleBonus;
    public bool level9_UnlockProjectile;
    public int level9_AttacksPerProjectile_Reduce;
}