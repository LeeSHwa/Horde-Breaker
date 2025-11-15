// PassiveUpgradeSO.cs (NEW FILE)
// This ScriptableObject defines a single passive stat boost.

using UnityEngine;

[CreateAssetMenu(fileName = "NewPassiveUpgrade", menuName = "Stats/Passive Upgrade")]
public class PassiveUpgradeSO : ScriptableObject
{
    // Enum to define what stat this upgrade affects
    public enum UpgradeType
    {
        DamageMultiplier,
        MaxHealth,
        MoveSpeed
        // (Add more as needed)
    }

    [Header("Upgrade Details")]
    public UpgradeType type;

    // The flat value to add (e.g., 0.1 for +10% Dmg)
    public float value;

    [Header("UI Info")]
    public string upgradeName;
    public Sprite icon;
    [TextArea]
    public string description;

    // A helper function to apply this upgrade
    public void Apply(StatsController stats)
    {
        switch (type)
        {
            case UpgradeType.DamageMultiplier:
                stats.ApplyDamageMultiplier(value);
                break;
            case UpgradeType.MaxHealth:
                stats.ApplyMaxHealth(value);
                break;
                // case UpgradeType.MoveSpeed:
                //    stats.ApplyMoveSpeed(value); 
                //    break;
        }
    }
}