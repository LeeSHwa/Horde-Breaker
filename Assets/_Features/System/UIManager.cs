using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // [Required] For scene loading

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

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultTimeText;
    public TextMeshProUGUI resultKillText;

    [Header("Revival UI")]
    public GameObject revivalPanel;
    public TextMeshProUGUI revivalCountText;

    [Header("Scene Management")]
    [Tooltip("Enter the exact name of your Lobby Scene file")]
    public string lobbySceneName = "LobbyScene"; // [NEW] Set this in Inspector!

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

    public void ShowGameOver(float finalTime, int finalKills)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Show the panel
        }

        // Format time to MM:SS
        int minutes = Mathf.FloorToInt(finalTime / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);

        if (resultTimeText != null)
        {
            // [English String] Display Survival Time
            resultTimeText.text = $"Survival Time : {minutes:00}:{seconds:00}";
        }

        if (resultKillText != null)
        {
            // [English String] Display Kill Count
            resultKillText.text = $"Enemies Killed : {finalKills}";
        }
    }

    public void ShowRevivalPopup(int remainingCount)
    {
        if (revivalPanel != null)
        {
            revivalPanel.SetActive(true);
            if (revivalCountText != null)
            {
                revivalCountText.text = $"Revival Left: {remainingCount}";
            }
        }
    }

    public void OnClickReviveButton()
    {
        if (revivalPanel != null) revivalPanel.SetActive(false);

        StatsController player = FindAnyObjectByType<StatsController>();

        if (player != null)
        {
            player.RevivePlayer();
        }
        else
        {
            Debug.LogError("Player StatsController Not Found!");
        }
    }

    public void OnClickGiveUpButton()
    {
        if (revivalPanel != null) revivalPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    // Function for the 'Retry' (Restart) button
    public void RetryGame()
    {
        Time.timeScale = 1f; // Resume game time
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // [NEW] Function for the 'Lobby' button
    public void GoToLobby()
    {
        Time.timeScale = 1f; // Resume time is crucial before leaving!
        SceneManager.LoadScene(lobbySceneName);
    }
}