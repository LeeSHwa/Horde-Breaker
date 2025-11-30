using UnityEngine;
using System.Collections.Generic;

public class SkillStatusManager : MonoBehaviour
{
    public static SkillStatusManager Instance;

    [Header("UI References")]
    public SkillSlotUI[] activeSlots;
    public SkillSlotUI[] passiveSlots;

    [Header("System References")]
    public ActiveSkillManager skillManager;
    public StatsController playerStats;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var slot in activeSlots) if (slot != null) slot.ClearSlot();
        foreach (var slot in passiveSlots) if (slot != null) slot.ClearSlot();

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (skillManager == null || playerStats == null) return;

        List<Skills> myActives = skillManager.GetEquippedSkills();
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] == null) continue;

            if (i < myActives.Count)
            {
                activeSlots[i].SetSlot(myActives[i].skillData.icon, myActives[i].CurrentLevel);
            }
            else
            {
                activeSlots[i].ClearSlot();
            }
        }

        List<PassiveUpgradeSO> myPassives = playerStats.learnedPassives;

        for (int i = 0; i < passiveSlots.Length; i++)
        {
            if (passiveSlots[i] == null) continue;

            if (i < myPassives.Count)
            {
                PassiveUpgradeSO data = myPassives[i];
                int level = playerStats.GetPassiveLevel(data.upgradeName);

                passiveSlots[i].SetSlot(data.icon, level);
            }
            else
            {
                passiveSlots[i].ClearSlot();
            }
        }
    }
}