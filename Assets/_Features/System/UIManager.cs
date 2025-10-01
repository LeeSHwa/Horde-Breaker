using UnityEngine;
using UnityEngine.UI; // Required for using UI components like Text and Slider.

public class UIManager : MonoBehaviour
{
    // Singleton instance to allow easy access from anywhere.
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public Text timeText;      // Reference to the Text component for the timer.
    public Slider healthSlider; // Reference to the Slider component for the health bar.

    void Awake()
    {
        // Singleton Pattern implementation.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Updates the timer UI. Called by the GameManager.
    /// </summary>
    public void UpdateTimeUI(float currentTime)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    /// <summary>
    /// Updates the health bar UI. Called by CharacterStats when health changes.
    /// </summary>
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            // The slider's value is a ratio from 0 to 1, so we divide current by max.
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}