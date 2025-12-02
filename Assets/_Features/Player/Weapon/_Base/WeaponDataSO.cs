using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Stats/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Common Stats")]
    public float baseDamage = 10f;
    public float baseAttackCooldown = 0.5f;
    public float knockback = 5f;

    [Header("UI & Leveling")]
    public string weaponName = "Weapon";
    public Sprite icon;
    public int maxLevel = 5;

    [TextArea(3, 5)]
    public string weaponDescription = "Weapon description.";

    // A list to hold descriptions for levels 2, 3, 4, 5+
    public List<string> levelDescriptions;

    // Audio Section
    [Header("Audio")]
    [Tooltip("Sound played when attacking (Firing/Swinging)")]
    public AudioClip attackSound;

    [Tooltip("Sound played when the projectile hits an enemy")]
    public AudioClip hitSound;

    // Random Damage Variance
    [Tooltip("Random damage variance percentage (+/-). e.g., 0.1 means +/- 10%")]
    public float damageVariance = 0.1f;
}