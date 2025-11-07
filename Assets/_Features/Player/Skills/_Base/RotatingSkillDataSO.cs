using UnityEngine;

[CreateAssetMenu(fileName = "NewRotatingSkillData", menuName = "Stats/Skill Data/Rotating Skill Data")]
public class RotatingSkillDataSO : SkillDataSO // Inherits from the base SkillDataSO
{
    [Header("Rotating Skill Specific")]
    public GameObject projectilePrefab;
    public float baseDuration = 5f;
    public float baseRotationSpeed = 100f;
    public float baseRotationRadius = 1.5f;

    [Header("Rotating Skill Level Up Stats")]
    public float level2_DamageIncrease = 10f;
    public int level3_ProjectileCount = 2;
    public float level4_RotationSpeedIncrease = 20f;
    public float level5_RadiusIncrease = 0.5f;
}
