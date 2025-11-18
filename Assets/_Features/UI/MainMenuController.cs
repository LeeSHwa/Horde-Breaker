using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Play 버튼에 연결할 함수
    public void GameStart()
    {
        SceneManager.LoadScene("SampleScene");

        Debug.Log("게임 시작!"); 
    }

    // (선택) 게임 종료 버튼용 함수
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}