using UnityEngine;

public class CheatToggle : MonoBehaviour
{
    public GameObject cheatPanel; 

    void Start()
    {
        if (cheatPanel != null)
            cheatPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (cheatPanel != null)
            {
                bool isOpening = !cheatPanel.activeSelf;
                
                cheatPanel.SetActive(isOpening);

                if (isOpening)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
        }
    }
}