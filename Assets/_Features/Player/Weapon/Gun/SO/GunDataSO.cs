using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Stats/Weapon Data/Gun Data")]
public class GunDataSO : WeaponDataSO
{
    [Header("Gun Specific")]
    public GameObject bulletPrefab;
    public int baseProjectileCount = 1; // Default count

    // Angle between projectiles when shooting multiple shots.
    [Tooltip("Angle spread between bullets (e.g. 15).")]
    public float multiShotSpread = 15f;

    [Header("Gun Level Up Stats")]

    // [Header Logic] "Level X" header contains stats for reaching Level (X+1)

    [Header("Level 1")] // Stats for Level 2
    public float level2_DamageBonus = 5f;
    public int level2_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage (e.g. 10 = 10%).")]
    public float level2_CooldownReduction = 0f;

    [Header("Level 2")] // Stats for Level 3
    public float level3_DamageBonus = 0f;
    public int level3_ProjectileCountBonus = 2;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level3_CooldownReduction = 0f;

    [Header("Level 3")] // Stats for Level 4
    public float level4_DamageBonus = 0f;
    public int level4_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level4_CooldownReduction = 10f; // Example

    [Header("Level 4")] // Stats for Level 5
    public float level5_DamageBonus = 0f;
    public int level5_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level5_CooldownReduction = 0f;

    [Header("Level 5")] // Stats for Level 6
    public float level6_DamageBonus = 0f;
    public int level6_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level6_CooldownReduction = 0f;

    [Header("Level 6")] // Stats for Level 7
    public float level7_DamageBonus = 0f;
    public int level7_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level7_CooldownReduction = 0f;

    [Header("Level 7")] // Stats for Level 8
    public float level8_DamageBonus = 0f;
    public int level8_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level8_CooldownReduction = 0f;

    [Header("Level 8")] // Stats for Level 9 (Max)
    public float level9_DamageBonus = 0f;
    public int level9_ProjectileCountBonus = 0;
    [Tooltip("Reduces attack cooldown by Percentage.")]
    public float level9_CooldownReduction = 0f;

    [Tooltip("Set true to unlock Penetration at Max Level")]
    public bool level9_UnlockPenetration = true;
}