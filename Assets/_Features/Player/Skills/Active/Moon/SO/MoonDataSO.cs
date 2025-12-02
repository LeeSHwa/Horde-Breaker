using UnityEngine;

[CreateAssetMenu(fileName = "NewMoonData", menuName = "Stats/Skill Data/Moon Data")]
public class MoonDataSO : SkillDataSO
{
    [Header("Moon Specific Base Stats")]
    public GameObject projectilePrefab;
    public float baseDuration = 5f;
    public float baseRotationSpeed = 100f;
    public float baseRotationRadius = 1.5f;
    public int baseProjectileCount = 3; // Starts with 3 satellites

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_RotationSpeedBonus;
    public float level2_DurationBonus;
    public float level2_RadiusBonus;
    public int level2_ProjectileCountBonus;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_RotationSpeedBonus;
    public float level3_DurationBonus;
    public float level3_RadiusBonus;
    public int level3_ProjectileCountBonus;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_RotationSpeedBonus;
    public float level4_DurationBonus;
    public float level4_RadiusBonus;
    public int level4_ProjectileCountBonus;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_RotationSpeedBonus;
    public float level5_DurationBonus;
    public float level5_RadiusBonus;
    public int level5_ProjectileCountBonus;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_RotationSpeedBonus;
    public float level6_DurationBonus;
    public float level6_RadiusBonus;
    public int level6_ProjectileCountBonus;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_RotationSpeedBonus;
    public float level7_DurationBonus;
    public float level7_RadiusBonus;
    public int level7_ProjectileCountBonus;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_RotationSpeedBonus;
    public float level8_DurationBonus;
    public float level8_RadiusBonus;
    public int level8_ProjectileCountBonus;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_RotationSpeedBonus;
    public float level9_DurationBonus;
    public float level9_RadiusBonus;
    public int level9_ProjectileCountBonus;
}