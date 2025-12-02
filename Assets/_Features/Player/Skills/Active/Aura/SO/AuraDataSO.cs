using UnityEngine;

[CreateAssetMenu(fileName = "NewAuraData", menuName = "Stats/Skill Data/Aura Data")]
public class AuraDataSO : SkillDataSO
{
    [Header("Aura Specific Base Stats")]
    public GameObject zonePrefab;
    public float baseDuration = 3f;
    public float baseArea = 2f;
    [Tooltip("70 = 30% slow")]
    public float baseSpeedDebuffPercentage = 70f;

    [Header("Audio")]
    public AudioClip slowHitSound;

    // --- Flexible Level Up Data Structure ---

    [Header("Level 1 (Upgrade to Lv.2)")]
    public float level2_DamageBonus;
    public float level2_AreaBonus;
    public float level2_DurationBonus;
    public float level2_DebuffBonus; // e.g. -5 to make 70 -> 65 (stronger slow)

    [Header("Level 2 (Upgrade to Lv.3)")]
    public float level3_DamageBonus;
    public float level3_AreaBonus;
    public float level3_DurationBonus;
    public float level3_DebuffBonus;

    [Header("Level 3 (Upgrade to Lv.4)")]
    public float level4_DamageBonus;
    public float level4_AreaBonus;
    public float level4_DurationBonus;
    public float level4_DebuffBonus;

    [Header("Level 4 (Upgrade to Lv.5)")]
    public float level5_DamageBonus;
    public float level5_AreaBonus;
    public float level5_DurationBonus;
    public float level5_DebuffBonus;

    [Header("Level 5 (Upgrade to Lv.6)")]
    public float level6_DamageBonus;
    public float level6_AreaBonus;
    public float level6_DurationBonus;
    public float level6_DebuffBonus;

    [Header("Level 6 (Upgrade to Lv.7)")]
    public float level7_DamageBonus;
    public float level7_AreaBonus;
    public float level7_DurationBonus;
    public float level7_DebuffBonus;

    [Header("Level 7 (Upgrade to Lv.8)")]
    public float level8_DamageBonus;
    public float level8_AreaBonus;
    public float level8_DurationBonus;
    public float level8_DebuffBonus;

    [Header("Level 8 (Upgrade to Lv.9 - Max)")]
    public float level9_DamageBonus;
    public float level9_AreaBonus;
    public float level9_DurationBonus;
    public float level9_DebuffBonus;
}