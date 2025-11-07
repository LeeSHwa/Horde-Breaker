using UnityEngine;

// This path must match the existing file
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Stats/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [Header("Common Stats")]
    // These two are required by Skills.cs
    public float baseDamage = 10f;
    public float baseAttackCooldown = 1f;

    // Added for RotatingSkill
    public float knockback = 3f;

    [Header("Description")]
    public string skillName = "New Skill";
    [TextArea]
    public string skillDescription = "Skill description.";
}