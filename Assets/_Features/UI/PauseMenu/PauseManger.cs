using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            UpdateStatusUI(); 
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

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
        pausePanel.SetActive(false);
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