using UnityEngine;

public class BossUnit : MonoBehaviour
{
    private StatsController stats;

    [Header("Boss Settings")]
    public bool isFinalBoss = false;

    void Awake()
    {
        stats = GetComponent<StatsController>();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            if (stats != null && stats.currentHP <= 0)
            {
                if (isFinalBoss)
                {
                    Debug.Log("Final Boss Defeated! Game Clear.");
                    GameManager.Instance.ProcessGameClear();
                }
                else
                {
                    Debug.Log("Boss Defeated! Resuming Timer.");
                    GameManager.Instance.ResumeGameTimer();
                }
            }
        }
    }
}