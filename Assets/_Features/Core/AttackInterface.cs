using UnityEngine;

public interface AttackInterface
{
    // Gets the current level of the item.
    int CurrentLevel { get; }

    // Gets the maximum level of the item.
    int MaxLevel { get; }

    // The function to call when the item is upgraded.
    void LevelUp();

    // --- Info for the UI Card ---

    // Gets the display name for the UI.
    string GetName();

    // Gets the description for the *next* level.
    string GetNextLevelDescription();

    // Gets the icon for the UI.
    Sprite GetIcon();
}
