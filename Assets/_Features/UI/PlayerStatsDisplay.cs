using UnityEngine;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("Main Stats UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI hpRecoveryText; 
    public TextMeshProUGUI armorText;     

    [Header("Offensive Stats UI")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI critChanceText;
    public TextMeshProUGUI critDamageText; 
    public TextMeshProUGUI cooldownText;   
    public TextMeshProUGUI projectileText; 

    [Header("Utility Stats UI")]
    public TextMeshProUGUI areaText;      
    public TextMeshProUGUI durationText;   
    public TextMeshProUGUI speedText;      
    public TextMeshProUGUI pickupText;    
    public TextMeshProUGUI expGainText;    
    public TextMeshProUGUI revivalText;    

    [Header("Session Info UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killText;

    private StatsController playerStats;

    private void OnEnable()
    {
        UpdateStatusUI();
    }

    public void UpdateStatusUI()
    {
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerStats = player.GetComponent<StatsController>();
        }

        if (playerStats == null) return;

        SetText(levelText, $"Lv. {playerStats.Level}");
        SetText(hpText, $"{playerStats.currentHP:F0} / {playerStats.MaxHP:F0}");
        SetText(hpRecoveryText, $"{playerStats.hpRecoveryRate:F1} / sec");
        SetText(armorText, $"{playerStats.armor:F0}");
        SetText(revivalText, $"{playerStats.revivalCount}");

        
        SetText(damageText, $"{playerStats.currentDamageMultiplier:F1}x");

        SetText(critChanceText, $"{playerStats.currentCritChance * 100f:F0}%");

        SetText(critDamageText, $"{playerStats.currentCritMultiplier:F1}x");

        SetText(cooldownText, $"-{playerStats.bonusCooldownReduction * 100f:F0}%");

        SetText(projectileText, $"+{playerStats.bonusProjectileCount}");


        float areaTotal = 1.0f + playerStats.bonusArea;
        SetText(areaText, $"{areaTotal:F1}x");

        float durationTotal = 1.0f + playerStats.bonusDuration;
        SetText(durationText, $"{durationTotal:F1}x");

        SetText(speedText, $"{playerStats.currentMoveSpeed:F1}");

        
        SetText(pickupText, $"+{playerStats.bonusPickupRange:F1}");

        SetText(expGainText, $"{playerStats.expGainMultiplier:F1}x");


        if (GameManager.Instance != null)
        {
            float t = GameManager.Instance.gameTime;
            int min = (int)(t / 60);
            int sec = (int)(t % 60);

            SetText(timeText, $"{min:D2}:{sec:D2}");
            SetText(killText, $"{GameManager.Instance.killCount}");
        }
    }

    private void SetText(TextMeshProUGUI ui, string content)
    {
        if (ui != null) ui.text = content;
    }
}