using UnityEngine;

// Creates an asset menu item for this specific skill data
[CreateAssetMenu(fileName = "NewAuraData", menuName = "Stats/Skill Data/Aura Data")]
public class AuraDataSO : SkillDataSO //
{
    [Header("Ground Zone Specific")]
    public GameObject zonePrefab;
    public float baseDuration = 3f;
    public float baseArea = 2f;

    [Tooltip("70 = 30% slow")]
    public float speedDebuffPercentage = 70f;

    [Header("Ground Zone Level Up Stats")] // [NOTE] Header kept for clarity
    public float level2_DamageIncrease = 5f;
    public float level3_AreaIncrease = 0.5f;

    [Header("Level 4 (Slow)")] 
    [Tooltip("Level 4: Sets the enemy's speed percentage (e.g., 75 = 75% speed)")]
    public float level4_SlowValue = 75f;

    [Header("Level 5 (Mastery)")] 
    [Tooltip("Level 5: Additional damage increase")] 
    public float level5_DamageIncrease = 10f;
    [Tooltip("Level 5: Additional area increase")]
    public float level5_AreaIncrease = 1f;
    [Tooltip("Level 5: Additional speed percentage reduction (e.g., -15 makes 75% -> 60%)")] 
    public float level5_DebuffIncrease = -15f;
}