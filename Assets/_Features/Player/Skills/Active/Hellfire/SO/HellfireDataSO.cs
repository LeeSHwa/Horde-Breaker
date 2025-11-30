using UnityEngine;

[CreateAssetMenu(fileName = "NewHellfireData", menuName = "Stats/Skill Data/Hellfire Data")]
public class HellfireDataSO : SkillDataSO
{
    [Header("Hellfire Specific Base Stats")]
    public GameObject missilePrefab;
    public float missileSpeed = 5f;
    public float searchRadius = 10f;

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_CooldownReduction;
    public int level2_ProjectileCountIncrease;
    public float level2_SpeedBonus;

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_CooldownReduction;
    public int level3_ProjectileCountIncrease;
    public float level3_SpeedBonus;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_CooldownReduction;
    public int level4_ProjectileCountIncrease;
    public float level4_SpeedBonus;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_CooldownReduction;
    public int level5_ProjectileCountIncrease;
    public float level5_SpeedBonus;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_CooldownReduction;
    public int level6_ProjectileCountIncrease;
    public float level6_SpeedBonus;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_CooldownReduction;
    public int level7_ProjectileCountIncrease;
    public float level7_SpeedBonus;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_CooldownReduction;
    public int level8_ProjectileCountIncrease;
    public float level8_SpeedBonus;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_CooldownReduction;
    public int level9_ProjectileCountIncrease;
    public float level9_SpeedBonus;
}