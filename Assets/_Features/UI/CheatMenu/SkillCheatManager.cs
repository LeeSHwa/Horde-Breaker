using UnityEngine;
using System.Collections.Generic;

public class SkillCheatManager : MonoBehaviour
{
    [Header("All Skills Database")]
    [Tooltip("게임에 존재하는 모든 스킬 프리팹을 여기에 다 넣어주세요.")]
    public List<GameObject> allSkillPrefabs;

    [Header("UI Slots")]
    public CheatSkillSlot[] skillSlots;

    private ActiveSkillManager playerSkillManager;

    private void OnEnable()
    {
        RefreshSkillPanel();
    }

    public void RefreshSkillPanel()
    {
        if (!FindPlayerSkillManager()) return;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (i < allSkillPrefabs.Count)
            {
                GameObject prefab = allSkillPrefabs[i];
                Skills prefabSkillScript = prefab.GetComponent<Skills>();

                if (prefabSkillScript != null)
                {
                    string skillName = prefabSkillScript.GetName();

                    Skills existingInstance = playerSkillManager.GetSkill(skillName);

                    skillSlots[i].Setup(this, prefab, existingInstance);
                }
            }
            else
            {
                skillSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void ForceEquipSkill(GameObject skillPrefab)
    {
        if (playerSkillManager != null)
        {
            playerSkillManager.EquipSkill(skillPrefab);
            RefreshSkillPanel(); 
        }
    }

    private bool FindPlayerSkillManager()
    {
        if (playerSkillManager != null) return true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerSkillManager = player.GetComponentInChildren<ActiveSkillManager>();
        }
        return playerSkillManager != null;
    }
    public void ForceRemoveSkill(Skills skill)
    {
        if (playerSkillManager != null)
        {
            playerSkillManager.UnEquipSkill(skill);

            RefreshSkillPanel();
        }
    }
}