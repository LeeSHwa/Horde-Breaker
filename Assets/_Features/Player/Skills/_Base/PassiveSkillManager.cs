using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PassiveSkillManager : MonoBehaviour
{
    // [Modified] Variable to link the 'SKill' container object, not 'Hero'
    [Tooltip("The 'SKill' container object that holds passive skills (Moon, Aura, etc.) as children")]
    public Transform skillContainerTransform;

    // List of passive skills to manage
    private List<Skills> passiveSkills;

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
        passiveSkills = skillContainerTransform.GetComponentsInChildren<Skills>().ToList();
    }

    void Update()
    {
        // Call TryAttack() for every managed passive skill
        foreach (Skills skill in passiveSkills)
        {
            // Call the TryAttack() with no parameters from Skills.cs
            skill.TryAttack();
        }
    }
}