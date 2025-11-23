using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // [기존] 조작법 패널
    public GameObject controlPanel;

    // [추가] 무기 패널 (Inspector에서 연결해야 함!)
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

    // --- 조작법(Control) 패널 관련 함수 ---
    public void OpenControlPanel()
    {
        controlPanel.SetActive(true);
    }

    public void CloseControlPanel()
    {
        controlPanel.SetActive(false);
    }

    // --- 무기(Weapon) 패널 관련 함수 [추가됨] ---
    public void OpenWeaponPanel()
    {
        weaponPanel.SetActive(true);
    }

    public void CloseWeaponPanel()
    {
        weaponPanel.SetActive(false);
    }
}