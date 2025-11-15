using UnityEngine;

public class CharacterStatsSO : ScriptableObject
{
    [Header("Core Stats")]
    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 5f;
    public float baseDamageMultiplier = 1.0f;

    [Header("Death")]
    public float deathAnimationLength = 0.5f;
}