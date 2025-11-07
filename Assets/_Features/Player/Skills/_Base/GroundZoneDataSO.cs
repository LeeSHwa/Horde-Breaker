using UnityEngine;

// Creates an asset menu item for this specific skill data
[CreateAssetMenu(fileName = "NewGroundZoneData", menuName = "Stats/Skill Data/Ground Zone Data")]
public class GroundZoneDataSO : SkillDataSO // Inherits from the base SkillDataSO
{
    [Header("Ground Zone Specific")]
    public GameObject zonePrefab;
    public float baseDuration = 3f;
    public float baseArea = 2f;

    [Tooltip("70 = 30% slow")]
    public float speedDebuffPercentage = 70f;

    [Header("Ground Zone Level Up Stats")]
    public float level2_DamageIncrease = 5f;
    public float level3_AreaIncrease = 0.5f;
    public float level4_DurationIncrease = 1f;
    public float level5_DebuffIncrease = -10f;
}