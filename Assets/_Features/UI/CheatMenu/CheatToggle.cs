using UnityEngine;

public class CheatToggle : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject cheatPanel;       // 기존 스탯 치트 패널
    public GameObject activeSkillPanel; // [추가] 스킬 치트 패널 (ActiveHolder)

    void Start()
    {
        if (cheatPanel != null)
            cheatPanel.SetActive(false);

        // [추가] 시작할 때 스킬 패널도 꺼둠
        if (activeSkillPanel != null)
            activeSkillPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            // cheatPanel이 null이 아닐 때 로직 실행
            if (cheatPanel != null)
            {
                // 현재 상태의 반대로 설정 (켜져있으면 끄고, 꺼져있으면 킴)
                bool isOpening = !cheatPanel.activeSelf;

                // 1. 메인 패널 켜기/끄기
                cheatPanel.SetActive(isOpening);

                // 2. [추가] 스킬 패널도 같이 켜기/끄기
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