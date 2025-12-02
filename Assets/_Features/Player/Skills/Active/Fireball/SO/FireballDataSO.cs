using UnityEngine;

[CreateAssetMenu(fileName = "NewFireballData", menuName = "Stats/Skill Data/Fireball Data")]
public class FireballDataSO : SkillDataSO
{
    [Header("Fireball Specific Base Stats")]
    public GameObject projectilePrefab;
    public float baseProjectileSpeed = 10f;
    public float baseProjectileLifetime = 3f;
    public float damageFalloffPercentage = 50f;
    public float spreadAngle = 15f; // Angle between projectiles

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_AreaBonus; // Scale increase
    public float level2_CooldownReduction; // Value (seconds)
    public int level2_ProjectileCountIncrease;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_AreaBonus;
    public float level3_CooldownReduction;
    public int level3_ProjectileCountIncrease;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_AreaBonus;
    public float level4_CooldownReduction;
    public int level4_ProjectileCountIncrease;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_AreaBonus;
    public float level5_CooldownReduction;
    public int level5_ProjectileCountIncrease;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_AreaBonus;
    public float level6_CooldownReduction;
    public int level6_ProjectileCountIncrease;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_AreaBonus;
    public float level7_CooldownReduction;
    public int level7_ProjectileCountIncrease;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_AreaBonus;
    public float level8_CooldownReduction;
    public int level8_ProjectileCountIncrease;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_AreaBonus;
    public float level9_CooldownReduction;
    public int level9_ProjectileCountIncrease;
}