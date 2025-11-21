using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton instance to allow easy access from anywhere.
    public static UIManager Instance { get; private set; }

    [Header("Text UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI levelText;

    [Header("Bars & Fill UI")]
    public Image playerHPBarFill;
    public Image hudHPBarFill;

    public TextMeshProUGUI playerHPText;
    public Image expBarFill;

    [Header("Dash UI")]
    public Transform dashContainer;
    public GameObject dashSlotPrefab;
    private List<Image> dashFillImages = new List<Image>();

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
        float ratio = Mathf.Clamp01(currentHP / maxHP);

        if (playerHPBarFill != null)
        {
            playerHPBarFill.fillAmount = ratio;
        }

        if (hudHPBarFill != null)
        {
            hudHPBarFill.fillAmount = ratio;
        }

        if (playerHPText != null)
        {
            int displayHP = Mathf.Max(0, (int)currentHP);
            playerHPText.text = $"{displayHP} / {(int)maxHP}";
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
            levelText.text = "" + newLevel;
        }
    }

    public void InitDashSlots(int maxDashCount)
    {
        if (dashContainer == null || dashSlotPrefab == null) return;

        foreach (Transform child in dashContainer) Destroy(child.gameObject);
        dashFillImages.Clear();

        for (int i = 0; i < maxDashCount; i++)
        {
            GameObject slot = Instantiate(dashSlotPrefab, dashContainer);
            Image fill = slot.transform.GetChild(0).GetComponent<Image>();
            if (fill != null) dashFillImages.Add(fill);
        }
    }

    public void UpdateDashUI(float currentCharge)
    {
        for (int i = 0; i < dashFillImages.Count; i++)
        {
            float fill = Mathf.Clamp01(currentCharge - i);
            dashFillImages[i].fillAmount = fill;
        }
    }

}