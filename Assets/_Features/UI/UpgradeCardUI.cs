// UpgradeCardUI.cs
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

        // Use the helper functions from the choice to set UI
        titleText.text = choice.GetName();
        descriptionText.text = choice.GetDescription();
        iconImage.sprite = choice.GetIcon();

        // Enable the button
        selectButton.interactable = true;

        // Make sure the button is set up to call OnCardSelected
        selectButton.onClick.RemoveAllListeners(); // Clear old listeners
        selectButton.onClick.AddListener(OnCardSelected);
    }

    // This function is called when the player clicks this card
    private void OnCardSelected()
    {
        // Disable all buttons to prevent double-clicking
        // This is a simple way; a better way is LevelUpManager controlling this.
        selectButton.interactable = false;

        // Tell the LevelUpManager what was chosen
        LevelUpManager.Instance.OnUpgradeSelected(currentChoice);
    }
}