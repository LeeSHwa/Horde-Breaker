using UnityEngine;

// Creates an asset menu item for this specific skill data
// [MODIFIED] CreateAssetMenu path updated to reflect new name
[CreateAssetMenu(fileName = "NewAuraData", menuName = "Stats/Skill Data/Aura Data")]
public class AuraDataSO : SkillDataSO // [MODIFIED] Class name changed from GroundZoneDataSO
{
    [Header("Ground Zone Specific")] // [NOTE] Header kept for clarity, as this is still a ground zone type
    public GameObject zonePrefab;
    public float baseDuration = 3f;
    public float baseArea = 2f;

    [Tooltip("70 = 30% slow")]
    public float speedDebuffPercentage = 70f;

    [Header("Ground Zone Level Up Stats")] // [NOTE] Header kept for clarity
    public float level2_DamageIncrease = 5f;
    public float level3_AreaIncrease = 0.5f;
    public float level4_DurationIncrease = 1f;
    public float level5_DebuffIncrease = -10f;
}