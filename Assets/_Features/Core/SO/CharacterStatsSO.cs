using UnityEngine;

public class CharacterStatsSO : ScriptableObject
{
    [Header("Core Stats")]
    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 5f;
    public float baseDamageMultiplier = 1.0f;

    [Header("Critical Stats")]
    [Tooltip("Base chance to land a critical hit (0.0 to 1.0). e.g., 0.05 = 5%")]
    public float baseCritChance = 0.05f; // Default 5%

    [Tooltip("Damage multiplier on critical hit. e.g., 1.5 = 150% damage")]
    public float baseCritMultiplier = 1.5f; // Default 150% damage

    [Header("Death")]
    public float deathAnimationLength = 0.5f;
}