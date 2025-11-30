using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Stats/Weapon Data/Gun Data")]
public class GunDataSO : WeaponDataSO
{
    [Header("Gun Specific")]
    public GameObject bulletPrefab;
    public int baseProjectileCount = 1;
    public int basePenetrationCount = 0; // Base finite penetration count

    // Angle between projectiles when shooting multiple shots.
    [Tooltip("Angle spread between bullets (e.g. 15).")]
    public float multiShotSpread = 15f;

    [Header("Gun Level Up Stats")]

    [Header("Level 1")]
    public float level2_DamageBonus;
    public int level2_ProjectileCountBonus;
    [Tooltip("Reduces attack cooldown by Percentage (e.g. 10 = 10%).")]
    public float level2_CooldownReduction;
    public int level2_PenetrationBonus;

    [Header("Level 2")]
    public float level3_DamageBonus;
    public int level3_ProjectileCountBonus;
    public float level3_CooldownReduction;
    public int level3_PenetrationBonus;

    [Header("Level 3")]
    public float level4_DamageBonus;
    public int level4_ProjectileCountBonus;
    public float level4_CooldownReduction;
    public int level4_PenetrationBonus;

    [Header("Level 4")]
    public float level5_DamageBonus;
    public int level5_ProjectileCountBonus;
    public float level5_CooldownReduction;
    public int level5_PenetrationBonus;

    [Header("Level 5")]
    public float level6_DamageBonus;
    public int level6_ProjectileCountBonus;
    public float level6_CooldownReduction;
    public int level6_PenetrationBonus;

    [Header("Level 6")]
    public float level7_DamageBonus;
    public int level7_ProjectileCountBonus;
    public float level7_CooldownReduction;
    public int level7_PenetrationBonus;

    [Header("Level 7")]
    public float level8_DamageBonus;
    public int level8_ProjectileCountBonus;
    public float level8_CooldownReduction;
    public int level8_PenetrationBonus;

    [Header("Level 8")]
    public float level9_DamageBonus;
    public int level9_ProjectileCountBonus;
    public float level9_CooldownReduction;
    public int level9_PenetrationBonus;

    [Tooltip("Set true to unlock Penetration at Max Level")]
    public bool level9_UnlockPenetration = true;
}