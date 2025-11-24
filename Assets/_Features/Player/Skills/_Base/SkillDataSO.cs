using System.Collections.Generic;
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

    [Header("UI & Leveling")]
    public string skillName = "Skill";
    public Sprite icon;
    public int maxLevel = 5;

    [TextArea(3, 5)]
    public string skillDescription = "Skill description.";

    public List<string> levelDescriptions;

    // [NEW] Audio Section
    [Header("Audio")]
    [Tooltip("Sound played when the skill activates (on cooldown)")]
    public AudioClip castSound;

    [Tooltip("Sound played when the skill hits an enemy")]
    public AudioClip hitSound;
}