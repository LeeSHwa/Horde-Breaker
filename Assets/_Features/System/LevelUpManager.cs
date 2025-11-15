// It uses an internal 'UpgradeChoice' class to handle 3 different
// types of upgrades: Existing Items, New Skills, and Passive Stats.

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for .OrderBy() and .Take()

public class LevelUpManager : MonoBehaviour
{
    // Singleton instance
    public static LevelUpManager Instance;

    [Header("UI References")]
    // The main panel that shows the upgrade choices
    public GameObject levelUpPanel;

    // You MUST assign your 3 UI Card scripts here (e.g., a script on the card)
    public UpgradeCardUI[] uiCards;

    [Header("System References")]
    public StatsController playerStats;
    public PlayerController playerController;
    public ActiveSkillManager skillManager; // Renaming to ActiveSkillManager is recommended

    [Header("Available Upgrade Pools")]
    // All possible *new* skills to offer (Assign prefabs in Inspector)
    public List<GameObject> newSkillPrefabs;

    // All possible *passive* stat boosts to offer (Assign SOs in Inspector)
    public List<PassiveUpgradeSO> passiveUpgrades;

    // Used for shuffling
    private System.Random rng = new System.Random();

    // --- [NEW] Helper class to manage the 3 upgrade types ---
    public class UpgradeChoice
    {
        // 1. For existing items (Weapon or Skill)
        public AttackInterface itemToUpgrade;

        // 2. For new skills
        public GameObject newSkillPrefab;

        // 3. For passive stat boosts
        public PassiveUpgradeSO passiveUpgrade;

        // --- Constructors ---
        public UpgradeChoice(AttackInterface item) { itemToUpgrade = item; }
        public UpgradeChoice(GameObject prefab) { newSkillPrefab = prefab; }
        public UpgradeChoice(PassiveUpgradeSO passive) { passiveUpgrade = passive; }

        // --- UI Helper Functions (Called by UI Card) ---
        public string GetName()
        {
            if (itemToUpgrade != null) return itemToUpgrade.GetName();
            // Assumes prefab has Skills.cs to get data
            if (newSkillPrefab != null) return newSkillPrefab.GetComponent<Skills>().skillData.skillName;
            if (passiveUpgrade != null) return passiveUpgrade.upgradeName;
            return "Error";
        }

        public string GetDescription()
        {
            if (itemToUpgrade != null) return itemToUpgrade.GetNextLevelDescription();
            if (newSkillPrefab != null) return newSkillPrefab.GetComponent<Skills>().skillData.skillDescription;
            if (passiveUpgrade != null) return passiveUpgrade.description;
            return "Error";
        }

        public Sprite GetIcon()
        {
            if (itemToUpgrade != null) return itemToUpgrade.GetIcon();
            if (newSkillPrefab != null) return newSkillPrefab.GetComponent<Skills>().skillData.icon;
            if (passiveUpgrade != null) return passiveUpgrade.icon;
            return null;
        }

        // --- Apply Function (Called when selected) ---
        public void Apply(StatsController stats, PlayerController player, ActiveSkillManager skills)
        {
            if (itemToUpgrade != null)
            {
                itemToUpgrade.LevelUp();
            }
            else if (newSkillPrefab != null)
            {
                skills.EquipSkill(newSkillPrefab);
            }
            else if (passiveUpgrade != null)
            {
                passiveUpgrade.Apply(stats);
            }
        }
    }
    // --- [End of Helper Class] ---

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        if (playerStats != null) { playerStats.OnPlayerLevelUp += ShowLevelUpChoices; }
        else { Debug.LogError("LevelUpManager: playerStats is not assigned!"); }

        if (playerController == null) Debug.LogError("LevelUpManager: playerController is not assigned!");
        if (skillManager == null) Debug.LogError("LevelUpManager: skillManager is not assigned!");

        if (levelUpPanel != null) { levelUpPanel.SetActive(false); }
    }

    void OnDestroy()
    {
        if (playerStats != null) { playerStats.OnPlayerLevelUp -= ShowLevelUpChoices; }
    }

    private void ShowLevelUpChoices()
    {
        Time.timeScale = 0f;
        if (levelUpPanel != null) { levelUpPanel.SetActive(true); }

        List<UpgradeChoice> choicePool = new List<UpgradeChoice>();

        // 1. Get Weapon upgrade
        Weapon weapon = playerController.GetCurrentWeapon();
        if (weapon != null && weapon.CurrentLevel < weapon.MaxLevel)
        {
            choicePool.Add(new UpgradeChoice(weapon));
        }

        // 2. Get equipped Skill upgrades
        List<Skills> skills = skillManager.GetEquippedSkills();
        foreach (Skills skill in skills)
        {
            if (skill != null && skill.CurrentLevel < skill.MaxLevel)
            {
                choicePool.Add(new UpgradeChoice(skill as AttackInterface));
            }
        }

        // 3. [FIXED] Add "New Skill" options
        if (skills.Count < 6) // Max 6 skills
        {
            // Find skills that are NOT already equipped
            List<GameObject> availableNewSkills = new List<GameObject>();
            foreach (var skillPrefab in newSkillPrefabs)
            {
                // Check prefab name against instantiated game object names
                string prefabName = skillPrefab.name;
                bool isEquipped = false;
                foreach (var equippedSkill in skills)
                {
                    // Assumes instantiated object is named like "MySkill(Clone)"
                    if (equippedSkill.gameObject.name.StartsWith(prefabName))
                    {
                        isEquipped = true;
                        break;
                    }
                }
                if (!isEquipped)
                {
                    availableNewSkills.Add(skillPrefab);
                }
            }

            // Add all available new skills to the pool
            foreach (var newSkill in availableNewSkills)
            {
                choicePool.Add(new UpgradeChoice(newSkill));
            }
        }

        // 4. [FIXED] Add "Passive Stat" options
        foreach (var passive in passiveUpgrades)
        {
            // TODO: Add logic to check if passive is maxed out
            choicePool.Add(new UpgradeChoice(passive));
        }

        // 5. Randomly pick 3
        var randomChoices = choicePool.OrderBy(x => rng.Next()).Take(3).ToList();

        // 6. Assign to UI cards (Pseudo-code)
        // Your UI Card script must be able to handle 'UpgradeChoice'

        Debug.Log($"Offering {randomChoices.Count} choices:");
        for (int i = 0; i < uiCards.Length; i++)
        {
            if (i < randomChoices.Count)
            {
                uiCards[i].Display(randomChoices[i]); // Pass the whole choice
                uiCards[i].gameObject.SetActive(true);
            }
            else
            {
                uiCards[i].gameObject.SetActive(false);
            }
        }

    }

    // This function must be called by the UI button.
    // The button must pass back the 'UpgradeChoice' it was displaying.
    public void OnUpgradeSelected(UpgradeChoice chosenUpgrade)
    {
        if (chosenUpgrade == null) return;

        // 1. Apply the upgrade
        chosenUpgrade.Apply(playerStats, playerController, skillManager);

        // 2. Hide the panel and unpause
        if (levelUpPanel != null) { levelUpPanel.SetActive(false); }
        Time.timeScale = 1f;
    }
}