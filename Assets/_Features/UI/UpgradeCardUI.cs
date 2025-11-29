using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro requires this

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Button selectButton; // The button component on this object

    [Header("New & Level UI")]
    // [NEW] Assign the "NEW!" badge object (Image or Panel) here
    public GameObject newBadge;
    // [NEW] Assign the TextMeshPro object for "Lv. 1" here
    public TextMeshProUGUI levelText;
    // -----------------------------

    // Stores the choice this card represents
    private LevelUpManager.UpgradeChoice currentChoice;

    void Awake()
    {
        // Ensure the button is found or assigned
        if (selectButton == null)
        {
            selectButton = GetComponent<Button>();
        }
    }

    // LevelUpManager calls this function to set the card's info
    public void Display(LevelUpManager.UpgradeChoice choice)
    {
        currentChoice = choice;

        // 1. Set Basic UI (Title, Desc, Icon)
        if (titleText != null) titleText.text = choice.GetName();
        if (descriptionText != null) descriptionText.text = choice.GetDescription();
        if (iconImage != null) iconImage.sprite = choice.GetIcon();

        // 2. Set "NEW!" Badge
        if (newBadge != null)
        {
            // Show badge only if it's a new skill
            newBadge.SetActive(choice.isNew);
        }

        // 3. Set Level Text
        if (levelText != null)
        {
            if (choice.isNew)
            {
                // New skills always start at Level 1
                levelText.text = "Lv.1";
                // Optional: You can change color for emphasis (e.g., Yellow)
                levelText.color = Color.yellow;
            }
            else
            {
                // Existing skills show the NEXT level
                int nextLevel = GetNextLevel(choice);
                levelText.text = $"Lv.{nextLevel}";
                levelText.color = Color.white;
            }
        }

        // Enable the button
        selectButton.interactable = true;

        // Make sure the button is set up to call OnCardSelected
        selectButton.onClick.RemoveAllListeners(); // Clear old listeners
        selectButton.onClick.AddListener(OnCardSelected);
    }

    // Helper function to calculate the next level
    private int GetNextLevel(LevelUpManager.UpgradeChoice choice)
    {
        // A. If it's a Weapon or Active Skill
        if (choice.itemToUpgrade != null)
        {
            return choice.itemToUpgrade.CurrentLevel + 1;
        }

        // B. If it's a Passive Skill
        if (choice.passiveUpgrade != null)
        {
            // Check current level from StatsController
            if (LevelUpManager.Instance != null && LevelUpManager.Instance.playerStats != null)
            {
                int currentLvl = LevelUpManager.Instance.playerStats.GetPassiveLevel(choice.passiveUpgrade.upgradeName);
                return currentLvl + 1;
            }
        }

        return 1; // Default fallback
    }

    // This function is called when the player clicks this card
    private void OnCardSelected()
    {
        // Disable all buttons to prevent double-clicking
        selectButton.interactable = false;

        // Tell the LevelUpManager what was chosen
        if (LevelUpManager.Instance != null)
        {
            LevelUpManager.Instance.OnUpgradeSelected(currentChoice);
        }
    }
}