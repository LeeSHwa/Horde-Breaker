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
            // 0.0 ~ 1.0 사이 값으로 변환하여 이미지 채움 정도 조절
            playerHPBarFill.fillAmount = Mathf.Clamp01(currentHP / maxHP);
        }
    }
}