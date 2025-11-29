using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject controlPanel;
    public GameObject weaponPanel;
    public GameObject settingPanel;   
    public GameObject upgradePanel;

    [Header("Etc")]
    public GameObject gunImage;
    public Weapon startingWeaponPrefab;

    public void GameStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenControlPanel()
    {
        controlPanel.SetActive(true);
    }

    public void CloseControlPanel()
    {
        controlPanel.SetActive(false);
    }

    public void OpenWeaponPanel()
    {
        weaponPanel.SetActive(true);
    }

    public void CloseWeaponPanel()
    {
        weaponPanel.SetActive(false);
    }

    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        upgradePanel.SetActive(true);
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }
}