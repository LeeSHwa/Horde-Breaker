using UnityEngine;

public class CheatToggle : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject cheatPanel;
    public GameObject activeSkillPanel;

    void Start()
    {
        if (cheatPanel != null)
            cheatPanel.SetActive(false);

        if (activeSkillPanel != null)
            activeSkillPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {

            if (cheatPanel != null)
            {
                bool isOpening = !cheatPanel.activeSelf;

                cheatPanel.SetActive(isOpening);

                if (activeSkillPanel != null)
                {
                    activeSkillPanel.SetActive(isOpening);

                    
                    if (isOpening) 
                    {
                         var manager = activeSkillPanel.GetComponent<SkillCheatManager>();
                         if (manager) manager.RefreshSkillPanel();
                    }
                    
                }

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