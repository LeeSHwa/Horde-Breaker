using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ActiveSkillManager : MonoBehaviour
{
    // [Modified] Variable to link the 'SKill' container object, not 'Hero'
    [Tooltip("The 'SKill' container object that holds passive skills (Moon, Aura, etc.) as children")]
    public Transform skillContainerTransform;

    // List of passive skills to manage
    private List<Skills> activeSkills;
    private StatsController ownerStats;

    void Awake()
    {
        ownerStats = GetComponentInParent<StatsController>();
        if (ownerStats == null)
        {
            Debug.LogError("PassiveSkillManager: Cannot find StatsController in parent!");
        }
    }
    void Start()
    {
        if (skillContainerTransform == null)
        {
            Debug.LogError("PassiveSkillManager is missing the 'SKill' container object reference!");
            return;
        }

        // [Modified] Find all 'Skills' components in the children of
        // 'skillContainerTransform' (not 'Hero') and add them to the list.
        // (This will find Aura, Moon, etc.)
        activeSkills = skillContainerTransform.GetComponentsInChildren<Skills>().ToList();
    }

    void Update()
    {
        // Call TryAttack() for every managed passive skill
        foreach (Skills skill in activeSkills)
        {
            // Call the TryAttack() with no parameters from Skills.cs
            skill.TryAttack();
        }
    }

    // Returns the list of currently equipped skills
    public List<Skills> GetEquippedSkills()
    {
        return activeSkills;
    }

    // Called by LevelUpManager to add a new skill
    public void EquipSkill(GameObject skillPrefab)
    {
        // Example: Check for 6 active skill slots
        if (activeSkills.Count >= 6)
        {
            Debug.Log("Skill slots are full.");
            return;
        }

        // Instantiate the skill as a child of the container
        GameObject skillObj = Instantiate(skillPrefab, skillContainerTransform);
        Skills newSkill = skillObj.GetComponent<Skills>();

        if (newSkill != null)
        {
            // The 'Skills' abstract class should find its own StatsController in Awake/Start
            // newSkill.Initialize(stats); // (If manual initialization is needed)
            activeSkills.Add(newSkill);
        }
    }
}