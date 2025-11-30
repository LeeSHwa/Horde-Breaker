using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossPatternManager : MonoBehaviour
{
    [Header("Skill References")]
    // Assign your existing skill scripts here in the Inspector
    public EnemyDashSkill dashSkill;
    public EnemySummonSkill summonSkill;
    public EnemyRangeAttackSkill rangeSkill;
    public EnemyTeleportSkill teleportSkill;

    [Header("Global Settings")]
    public float minGlobalCooldown = 3.0f; // Minimum wait time after a skill
    public float maxGlobalCooldown = 5.0f; // Maximum wait time after a skill

    private Transform playerTarget;
    private bool isPatternRunning = false; // Flag to check if a skill is currently executing

    void Start()
    {
        // Find Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        // Start the AI loop
        StartCoroutine(PatternLoop());
    }

    private IEnumerator PatternLoop()
    {
        // Initial delay before the boss starts acting
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            // Safety check: if player is dead or missing, stop AI
            if (playerTarget == null) yield break;

            // 1. Calculate distance to player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            // 2. Collect available skills (Queue/List Logic)
            List<MonoBehaviour> readySkills = new List<MonoBehaviour>();

            // Check each skill if it is ready (cooldown finished & within range)
            if (dashSkill != null && dashSkill.IsReady(distanceToPlayer))
                readySkills.Add(dashSkill);

            if (summonSkill != null && summonSkill.IsReady(distanceToPlayer))
                readySkills.Add(summonSkill);

            if (rangeSkill != null && rangeSkill.IsReady(distanceToPlayer))
                readySkills.Add(rangeSkill);

            if (teleportSkill != null && teleportSkill.IsReady(distanceToPlayer))
                readySkills.Add(teleportSkill);

            // 3. Select and Execute a skill
            if (readySkills.Count > 0)
            {
                // [Random Selection] Pick one random skill from the ready list
                MonoBehaviour selectedSkill = readySkills[Random.Range(0, readySkills.Count)];

                isPatternRunning = true;

                // Execute the selected skill and pass the callback (OnSkillFinished)
                if (selectedSkill == dashSkill)
                    dashSkill.Execute(OnSkillFinished);
                else if (selectedSkill == summonSkill)
                    summonSkill.Execute(OnSkillFinished);
                else if (selectedSkill == rangeSkill)
                    rangeSkill.Execute(OnSkillFinished);
                else if (selectedSkill == teleportSkill)
                    teleportSkill.Execute(OnSkillFinished);

                // Wait until the skill reports it is finished
                yield return new WaitUntil(() => isPatternRunning == false);

                // 4. Global Cooldown (Wait 3~5 seconds)
                float waitTime = Random.Range(minGlobalCooldown, maxGlobalCooldown);
                // Debug.Log($"Boss Resting for {waitTime} seconds...");
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                // If no skills are ready (all on cooldown or out of range), wait a frame
                yield return null;
            }
        }
    }

    // Callback function called by skills when they finish their action
    private void OnSkillFinished()
    {
        isPatternRunning = false;
    }
}