using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton instance to allow easy access from anywhere.
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI stageText;
    public Slider hpSlider;

    
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
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }


    public void UpdateStageUI(int currentStage)
    {
        stageText.text = "Stage " + currentStage;  
    }

    public void UpdateHP(int currentHP, int maxHP)
    {
            hpSlider.value = currentHP / maxHP;
    }
}