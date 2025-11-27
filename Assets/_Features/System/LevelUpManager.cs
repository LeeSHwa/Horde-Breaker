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

    // You MUST assign your 3 UI Card scripts here
    public UpgradeCardUI[] uiCards;

    [Header("System References")]
    public StatsController playerStats;
    public PlayerController playerController;
    public ActiveSkillManager skillManager;

    [Header("Available Upgrade Pools")]
    // All possible *new* skills to offer (Assign prefabs in Inspector)
    public List<GameObject> newSkillPrefabs;

    // All possible *passive* stat boosts to offer (Assign SOs in Inspector)
    public List<PassiveUpgradeSO> passiveUpgrades;

    [Header("Settings")]
    public int maxLevel = 99; // Max Level Cap

    // Used for shuffling
    private System.Random rng = new System.Random();

    // [NEW] Counter for pending level-ups to handle multiple level-ups at once
    private int pendingLevelUps = 0;

    // --- [Helper Class to manage upgrade types] ---
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
        // [CHANGED] Bind to 'RegisterLevelUp' instead of showing UI immediately
        if (playerStats != null) { playerStats.OnPlayerLevelUp += RegisterLevelUp; } 
        else { Debug.LogError("LevelUpManager: playerStats is not assigned!"); }

        if (playerController == null) Debug.LogError("LevelUpManager: playerController is not assigned!");
        if (skillManager == null) Debug.LogError("LevelUpManager: skillManager is not assigned!");

        if (levelUpPanel != null) { levelUpPanel.SetActive(false); }
    }

    void OnDestroy()
    {
        // [CHANGED] Unsubscribe correctly
        if (playerStats != null) { playerStats.OnPlayerLevelUp -= RegisterLevelUp; }
    }

    // [NEW] 1. Entry Point: Receives level-up event and queues it
    private void RegisterLevelUp()
    {
        // Increment queue
        pendingLevelUps++;
        
        // If the UI is NOT active, start the process.
        // If UI IS active, this level-up sits in 'pendingLevelUps' waiting for its turn.
        if (!levelUpPanel.activeSelf)
        {
            ProcessNextLevelUp();
        }
    }

    // [NEW] 2. Manager: Decides whether to show UI or resume game
    private void ProcessNextLevelUp()
    {
        if (pendingLevelUps > 0)
        {
            // Still have levels to process -> Show UI
            ShowUpgradeOptions(); 
        }
        else
        {
            // No more levels -> Close UI and Resume Game
            if (levelUpPanel != null) { levelUpPanel.SetActive(false); }

            SoundManager.Instance.StopLevelUpBGM();

            Time.timeScale = 1f;
        }
    }

    // [MOVED] 3. Worker: Prepares and displays the cards (Logic only)
    private void ShowUpgradeOptions()
    {
        Time.timeScale = 0f; // Pause Game
        if (levelUpPanel != null) { levelUpPanel.SetActive(true); }

        SoundManager.Instance.PlayLevelUpBGM();

        List<UpgradeChoice> choicePool = new List<UpgradeChoice>();

        // --- A. Gather Choices ---

        // 1. Weapon upgrade
        Weapon weapon = playerController.GetCurrentWeapon();
        if (weapon != null && weapon.CurrentLevel < weapon.MaxLevel)
        {
            choicePool.Add(new UpgradeChoice(weapon));
        }

        // 2. Equipped Skill upgrades
        List<Skills> skills = skillManager.GetEquippedSkills();
        foreach (Skills skill in skills)
        {
            if (skill != null && skill.CurrentLevel < skill.MaxLevel)
            {
                choicePool.Add(new UpgradeChoice(skill as AttackInterface));
            }
        }

        // 3. New Skills
        if (skills.Count < 6) // Max 6 skills
        {
            List<GameObject> availableNewSkills = new List<GameObject>();
            foreach (var skillPrefab in newSkillPrefabs)
            {
                string prefabName = skillPrefab.name;
                bool isEquipped = false;
                foreach (var equippedSkill in skills)
                {
                    // Check name (handling (Clone))
                    if (equippedSkill.gameObject.name.Contains(prefabName))
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

            foreach (var newSkill in availableNewSkills)
            {
                choicePool.Add(new UpgradeChoice(newSkill));
            }
        }

        // 4. Passive Stats
        foreach (var passive in passiveUpgrades)
        {
            // Add logic here if passives have max limits
            choicePool.Add(new UpgradeChoice(passive));
        }

        // --- B. Select Random 3 ---
        int countToTake = Mathf.Min(3, choicePool.Count);
        var randomChoices = choicePool.OrderBy(x => rng.Next()).Take(countToTake).ToList();

        // --- C. Update UI ---
        for (int i = 0; i < uiCards.Length; i++)
        {
            if (i < randomChoices.Count)
            {
                uiCards[i].Display(randomChoices[i]);
                uiCards[i].gameObject.SetActive(true);
            }
            else
            {
                uiCards[i].gameObject.SetActive(false);
            }
        }
    }

    // [NEW] 4. Exit Point: Called by UI Button
    public void OnUpgradeSelected(UpgradeChoice chosenUpgrade)
    {
        if (chosenUpgrade == null) return;

        // Apply the upgrade
        chosenUpgrade.Apply(playerStats, playerController, skillManager);

        // Decrease queue count
        pendingLevelUps--;

        // Check if there are more level-ups waiting
        ProcessNextLevelUp();
    }
}