using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for .Any()

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
    public int maxActiveSlots = 4;  // Limit for active skills (Weapon + Skills)
    public int maxPassiveSlots = 4; // Limit for passive items

    // Random Number Generator
    private System.Random rng = new System.Random();

    // Counter for pending level-ups to handle multiple level-ups at once
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

        // Probability weight for RNG
        public int weight = 7;

        // Flag to identify if this is a new item or an upgrade
        public bool isNew = false;

        // --- Constructors ---

        // Constructor 1: For Upgrades (Weapon or Active Skill)
        public UpgradeChoice(AttackInterface item)
        {
            itemToUpgrade = item;
            isNew = false; // It's an upgrade, so not new

            // Logic: Weapons are powerful, so make their upgrades rare
            if (item is Weapon) weight = 2;
            else weight = 7;
        }

        // Constructor 2: For NEW Active Skills
        public UpgradeChoice(GameObject prefab)
        {
            newSkillPrefab = prefab;
            isNew = true; // It's a new skill
            weight = 7; // Standard weight for new skills
        }

        // Constructor 3: For Passive Upgrades
        public UpgradeChoice(PassiveUpgradeSO passive, bool _isNew)
        {
            passiveUpgrade = passive;
            isNew = _isNew;

            // Use the weight defined in the Scriptable Object (Data-Driven)
            // Ensure PassiveUpgradeSO has 'rarityWeight' field
            weight = passive.rarityWeight;
        }

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
        if (playerStats != null) { playerStats.OnPlayerLevelUp += RegisterLevelUp; }
        else { Debug.LogError("LevelUpManager: playerStats is not assigned!"); }

        if (playerController == null) Debug.LogError("LevelUpManager: playerController is not assigned!");
        if (skillManager == null) Debug.LogError("LevelUpManager: skillManager is not assigned!");

        if (levelUpPanel != null) { levelUpPanel.SetActive(false); }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (playerStats != null) { playerStats.OnPlayerLevelUp -= RegisterLevelUp; }
    }

    // 1. Entry Point: Receives level-up event and queues it
    private void RegisterLevelUp()
    {
        pendingLevelUps++;

        // If the UI is NOT active, start the process immediately.
        if (!levelUpPanel.activeSelf)
        {
            ProcessNextLevelUp();
        }
    }

    // 2. Manager: Decides whether to show UI or resume game
    private void ProcessNextLevelUp()
    {
        if (pendingLevelUps > 0)
        {
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

    // 3. Worker: Prepares and displays the cards (Main Logic)
    private void ShowUpgradeOptions()
    {
        Time.timeScale = 0f; // Pause Game
        if (levelUpPanel != null) { levelUpPanel.SetActive(true); }

        SoundManager.Instance.PlayLevelUpBGM();

        List<UpgradeChoice> choicePool = new List<UpgradeChoice>();

        // --------------------------------------------------------
        // A. Collect Candidates (With Slot Limits & Upgrade Checks)
        // --------------------------------------------------------

        // 1. Existing Weapon Upgrade (High Priority check, Low Weight)
        Weapon weapon = playerController.GetCurrentWeapon();
        if (weapon != null && weapon.CurrentLevel < weapon.MaxLevel)
        {
            choicePool.Add(new UpgradeChoice(weapon));
        }

        // 2. Existing Active Skill Upgrades
        List<Skills> activeSkills = skillManager.GetEquippedSkills();
        foreach (Skills skill in activeSkills)
        {
            if (skill.CurrentLevel < skill.MaxLevel)
            {
                choicePool.Add(new UpgradeChoice(skill as AttackInterface));
            }
        }

        // 3. Passive Upgrades (Existing Upgrades + New Passives)

        // First, count how many passives the player currently owns
        int myPassiveCount = 0;
        foreach (var p in passiveUpgrades)
        {
            if (playerStats.GetPassiveLevel(p.upgradeName) > 0) myPassiveCount++;
        }

        foreach (var passive in passiveUpgrades)
        {
            int currentLvl = playerStats.GetPassiveLevel(passive.upgradeName);
            bool hasPassive = currentLvl > 0;

            if (hasPassive)
            {
                // If owned: Add to pool if not max level (isNew = false)
                if (currentLvl < passive.maxLevel)
                    choicePool.Add(new UpgradeChoice(passive, false));
            }
            else
            {
                // If NOT owned: Add only if slot is available (isNew = true)
                if (myPassiveCount < maxPassiveSlots)
                    choicePool.Add(new UpgradeChoice(passive, true));
            }
        }

        // 4. New Active Skills (Check Active Slot Limit)
        if (activeSkills.Count < maxActiveSlots)
        {
            foreach (var skillPrefab in newSkillPrefabs)
            {
                // Check if already equipped by name (handling clones)
                bool isEquipped = activeSkills.Any(s => s.skillData.skillName == skillPrefab.GetComponent<Skills>().skillData.skillName);

                if (!isEquipped)
                {
                    choicePool.Add(new UpgradeChoice(skillPrefab));
                }
            }
        }

        // --------------------------------------------------------
        // B. Weighted Random Selection
        // --------------------------------------------------------
        List<UpgradeChoice> selectedChoices = new List<UpgradeChoice>();
        int countToSelect = Mathf.Min(3, choicePool.Count);

        for (int i = 0; i < countToSelect; i++)
        {
            if (choicePool.Count == 0) break;

            // 1. Calculate Total Weight
            int totalWeight = 0;
            foreach (var c in choicePool) totalWeight += c.weight;

            // 2. Pick a random value
            int randomValue = rng.Next(0, totalWeight);

            // 3. Find the winner based on weight
            int currentSum = 0;
            UpgradeChoice picked = null;
            foreach (var c in choicePool)
            {
                currentSum += c.weight;
                if (randomValue < currentSum)
                {
                    picked = c;
                    break;
                }
            }

            // 4. Add to result and remove from pool to prevent duplicates
            if (picked != null)
            {
                selectedChoices.Add(picked);
                choicePool.Remove(picked);
            }
        }

        // --------------------------------------------------------
        // C. Update UI
        // --------------------------------------------------------
        for (int i = 0; i < uiCards.Length; i++)
        {
            if (i < selectedChoices.Count)
            {
                uiCards[i].Display(selectedChoices[i]);
                uiCards[i].gameObject.SetActive(true);
            }
            else
            {
                uiCards[i].gameObject.SetActive(false);
            }
        }
    }

    // 4. Exit Point: Called by UI Button
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