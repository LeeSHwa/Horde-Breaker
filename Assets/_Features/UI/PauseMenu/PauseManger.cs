using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public GameObject settingPanel;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel != null && settingPanel.activeSelf)
            {
                OnClickCloseSetting();
            }
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
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
            if (settingPanel != null) settingPanel.SetActive(false);
        }
        else
        {
            ResumeGameLogic();
        }
    }

    public void OnClickOpenSetting()
    {
        if (settingPanel != null) settingPanel.SetActive(true);
    }

    public void OnClickCloseSetting()
    {
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    public void OnClickResume()
    {
        isPaused = false;
        ResumeGameLogic();
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

    private void ResumeGameLogic()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }
}