using UnityEngine;

public class BossUnit : MonoBehaviour
{
    private StatsController stats;

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
                Debug.Log("Boss Defeated! Resuming Timer.");

                GameManager.Instance.ResumeGameTimer();
            }
        }
    }
}