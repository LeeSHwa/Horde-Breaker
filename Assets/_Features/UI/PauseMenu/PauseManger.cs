using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;       // The main pause menu background
    public GameObject settingPanel;     // The settings panel (Drag your Setting_Panel here)

    [Header("Status Text UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;

    [Header("Session Info UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killText;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private StatsController playerStats;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<StatsController>();
        }

        // Ensure panels are closed on start
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If Settings is open, close only Settings (Go back to Pause Menu)
            if (settingPanel != null && settingPanel.activeSelf)
            {
                OnClickCloseSetting();
            }
            // If Settings is closed, toggle the Pause Menu
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause Game
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);

            // Ensure settings is closed when first opening pause menu
            if (settingPanel != null) settingPanel.SetActive(false);

            UpdateStatusUI();
        }
        else
        {
            // Resume Game
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
            if (settingPanel != null) settingPanel.SetActive(false);
        }
    }

    // --- Settings Panel Control ---

    public void OnClickOpenSetting()
    {
        if (settingPanel != null) settingPanel.SetActive(true);
    }

    public void OnClickCloseSetting()
    {
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    // ------------------------------

    private void UpdateStatusUI()
    {
        if (playerStats == null) return;

        if (levelText != null)
            levelText.text = $"Lv. {playerStats.Level}";

        if (hpText != null)
            hpText.text = $"HP : {playerStats.currentHP:F0} / {playerStats.MaxHP:F0}";

        if (expText != null)
            expText.text = $"EXP : {playerStats.CurrentExp} / {playerStats.MaxExp}";

        if (damageText != null)
        {
            float dmgPer = playerStats.currentDamageMultiplier * 100f;
            damageText.text = $"Damage : {dmgPer:F0}%";
        }

        if (speedText != null)
            speedText.text = $"Speed : {playerStats.currentMoveSpeed:F1}";

        if (GameManager.Instance != null)
        {
            float t = GameManager.Instance.gameTime;
            int min = (int)(t / 60);
            int sec = (int)(t % 60);

            if (timeText != null)
                timeText.text = $"LifeTime : {min:D2}:{sec:D2}";

            if (killText != null)
                killText.text = $"EnemyKills : {GameManager.Instance.killCount}";
        }
    }

    public void OnClickResume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}