using UnityEngine;

// This is the abstract base class for ALL skill data.
// It ONLY contains data truly common to all skills (Aura, Satellite, etc.)
public abstract class SkillDataSO : ScriptableObject
{
    [Header("Common Stats")]
    // Stats that EVERY skill MUST have.
    public float baseDamage = 5f;
    public float baseAttackCooldown = 3f; // Skills often have different timings
    public float knockback = 1f;

    [Header("Description")]
    public string skillName = "Skill";
    [TextArea]
    public string skillDescription = "Skill description.";

    // Note: All Level Up stats (like 'area size' or 'satellite count')
    // are defined in the CHILD SO (e.g., AuraDataSO, SatelliteDataSO)
}