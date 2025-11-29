using UnityEngine;

[CreateAssetMenu(fileName = "NewPassiveUpgrade", menuName = "Stats/Passive Upgrade")]
public class PassiveUpgradeSO : ScriptableObject
{
    public enum UpgradeType
    {
        DamageMultiplier, MaxHealth, MoveSpeed,
        Cooldown, ProjectileCount, Area, Duration, PickupRange,
        HealthRecovery, Armor, Revival, ExpGain, CritChance, CritDamage
    }

    [Header("Upgrade Details")]
    public UpgradeType type;
    public float value;

    [Header("Level Settings")]
    public int maxLevel = 8;

    [Header("Spawn Settings")]
    [Tooltip("Higher value = More frequent appearance. (Standard: 20, Rare: 5)")]
    public int rarityWeight = 20;

    [Header("UI Info")]
    public string upgradeName;
    public Sprite icon;
    [TextArea] public string description;

    public void Apply(StatsController stats)
    {
        stats.ApplyPassive(this);
    }
}