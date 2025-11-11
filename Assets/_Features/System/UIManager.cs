using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton instance to allow easy access from anywhere.
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI stageText;
    public Image playerHPBarFill;
    public Image expBarFill;

    public TextMeshProUGUI levelText;

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


    public void UpdateTimeUI(float currentTime)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }


    //public void UpdateStageUI(int currentStage)
    //{
    //    stageText.text = "Stage " + currentStage;
    //}

    public void UpdateHP(float currentHP, float maxHP)
    {
        if (playerHPBarFill != null)
        {
            playerHPBarFill.fillAmount = Mathf.Clamp01(currentHP / maxHP);
        }
    }

    // Called by StatsController.AddExp()
    public void UpdateExp(int current, int max)
    {
        if (expBarFill != null)
        {
            // Use fillAmount for Image-based bars
            // We need to cast to float for division
            expBarFill.fillAmount = Mathf.Clamp01((float)current / max);
        }
    }

    public void UpdateLevel(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = "Lv. " + newLevel;
        }
    }

}