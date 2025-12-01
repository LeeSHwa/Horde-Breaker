using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStatusManager : MonoBehaviour
{
    public static SkillStatusManager Instance;

    [Header("UI References")]
    public SkillSlotUI weaponSlot;
    public SkillSlotUI[] activeSlots;
    public SkillSlotUI[] passiveSlots;

    [Header("System References")]
    public PlayerController playerController;
    public ActiveSkillManager skillManager;
    public StatsController playerStats;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();


        yield return null;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (skillManager == null || playerStats == null || playerController == null) return;

        Weapon currentWeapon = playerController.GetCurrentWeapon();

        if (weaponSlot != null)
        {
            if (currentWeapon != null)
            {
                weaponSlot.SetSlot(currentWeapon.GetIcon(), currentWeapon.CurrentLevel);
            }

        }

        List<Skills> myActives = skillManager.GetEquippedSkills();
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] == null) continue;

            if (i < myActives.Count)
            {
                activeSlots[i].SetSlot(myActives[i].skillData.icon, myActives[i].CurrentLevel);
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
        }
    }
}