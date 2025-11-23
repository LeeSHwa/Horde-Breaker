using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject controlPanel;

    public GameObject weaponPanel;

    public void GameStart()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("게임 시작!");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
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
}