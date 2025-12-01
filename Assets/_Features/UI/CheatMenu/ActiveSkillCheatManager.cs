using UnityEngine;
using System.Collections.Generic;

public class ActiveSkillCheatManager : MonoBehaviour
{
    public ActiveSkillManager activeSkillManager;

    // Hierarchy에 있는 6개 카드 슬롯
    public List<CheatSkillSlot> uiSlots;

    // [중요] 여기에 프리팹 말고, 'UpgradeChoice' (데이터)를 넣어야 합니다.
    // LevelUpManager에서 사용하는 스킬 데이터 리스트를 여기에 할당해주세요.
    public List<LevelUpManager.UpgradeChoice> skillDataList;

    private void OnEnable()
    {
        if (activeSkillManager == null) return;

        // 슬롯 개수만큼 돌면서 데이터 연결
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < skillDataList.Count)
            {
                uiSlots[i].gameObject.SetActive(true);
                // 슬롯에게 데이터(Choice)와 매니저를 넘겨줌
                uiSlots[i].Init(skillDataList[i], activeSkillManager);
            }
            else
            {
                uiSlots[i].gameObject.SetActive(false);
            }
        }
    }
}