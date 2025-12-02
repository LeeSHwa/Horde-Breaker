using UnityEngine;

[CreateAssetMenu(fileName = "EnergyBall_Data", menuName = "Stats/Weapon Data/EnergyBallData")]
public class EnergyBallDataSO : WeaponDataSO
{
    [Header("EnergyBall Base Stats (Lv.1)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public int baseBounces = 1;
    public float bounceRange = 10f; // Increased default range
    public float lifetime = 5f;     // Initial lifetime (long)

    [Header("Behavior")]
    [Tooltip("Time to find next target after a bounce (Short)")]
    public float bounceSearchDuration = 1.5f; // Short duration after hit

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_CooldownReduction;
    public int level2_BounceIncrease;
    public int level2_ProjectileCountIncrease;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_CooldownReduction;
    public int level3_BounceIncrease;
    public int level3_ProjectileCountIncrease;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_CooldownReduction;
    public int level4_BounceIncrease;
    public int level4_ProjectileCountIncrease;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_CooldownReduction;
    public int level5_BounceIncrease;
    public int level5_ProjectileCountIncrease;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_CooldownReduction;
    public int level6_BounceIncrease;
    public int level6_ProjectileCountIncrease;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_CooldownReduction;
    public int level7_BounceIncrease;
    public int level7_ProjectileCountIncrease;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_CooldownReduction;
    public int level8_BounceIncrease;
    public int level8_ProjectileCountIncrease;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_CooldownReduction;
    public int level9_BounceIncrease;
    public int level9_ProjectileCountIncrease;
}