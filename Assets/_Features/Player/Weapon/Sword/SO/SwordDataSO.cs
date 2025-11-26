using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordData", menuName = "Stats/Weapon Data/Sword Data")]
public class SwordDataSO : WeaponDataSO
{
    [Header("Sword Specific")]
    public float baseSwingDuration = 0.5f;
    public float baseAngle = 90f;
    public float baseAreaRadius = 1.5f;
    public float swingStartOffset = 45f;

    [Header("Level 1")]
    public float level2_DamageBonus = 10f;
    public float level2_SpeedIncrease = 0.1f;

    [Tooltip("Reduces attack cooldown by Percentage (e.g. 10 = 10%).")]
    public float level2_CooldownReduction = 10f; // Example default changed to 10
    public float level2_AreaIncrease = 0f;
    public float level2_AngleIncrease = 0f;

    [Header("Level 2")]
    public float level3_DamageBonus = 0f;
    public float level3_AreaIncrease = 0.5f;
    public float level3_AngleIncrease = 0f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level3_CooldownReduction = 0f;

    [Header("Level 3")]
    public float level4_DamageBonus = 0f;
    public float level4_AreaIncrease = 0f;
    public float level4_AngleIncrease = 30f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level4_CooldownReduction = 0f;

    [Header("Level 4")]
    public float level5_DamageBonus = 0f;
    public float level5_AreaIncrease = 0f;
    public float level5_AngleIncrease = 0f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level5_CooldownReduction = 10f;

    [Header("Level 5")]
    public float level6_DamageBonus = 0f;
    public float level6_AreaIncrease = 0f;
    public float level6_AngleIncrease = 0f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level6_CooldownReduction = 0f;

    [Header("Level 6")]
    public float level7_DamageBonus = 0f;
    public float level7_AreaIncrease = 0f;
    public float level7_AngleIncrease = 0f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level7_CooldownReduction = 0f;

    [Header("Level 7")]
    public float level8_DamageBonus = 0f;
    public float level8_AreaIncrease = 0f;
    public float level8_AngleIncrease = 0f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level8_CooldownReduction = 0f;

    [Header("Level 8 (Projectile Unlock)")]
    public float level9_DamageBonus = 0f;
    public float level9_AreaIncrease = 0f;
    public float level9_AngleIncrease = 0f;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level9_CooldownReduction = 0f;

    public int level9_AttacksPerProjectile_Reduce = 0;

    [Header("Projectile Settings")]
    public int baseAttacksPerProjectile = 5;
    public GameObject projectilePrefab;
    public float projectileDamagePercent = 0.5f;
    public float projectileKnockbackPercent = 0.5f;
    public float projectileLifetime = 3f;
    public float projectileArcScaleMultiplier = 1.0f;
}