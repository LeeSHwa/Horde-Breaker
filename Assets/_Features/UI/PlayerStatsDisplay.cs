using UnityEngine;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("Main Stats UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;        
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
        FindAndSetupPlayer();
        UpdateStatusUI();
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateStatusUI;
        }
    }

    private void FindAndSetupPlayer()
    {
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<StatsController>();
            }
        }

        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateStatusUI;
            playerStats.OnStatsChanged += UpdateStatusUI;
        }
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

        // --- 1. Survival (Defense) ---
        SetText(levelText, $"Lv. {playerStats.Level}");

        SetText(expText, $"EXP : {playerStats.CurrentExp} / {playerStats.MaxExp}");

        SetText(hpText, $"MaxHP : {playerStats.currentHP:F0} / {playerStats.MaxHP:F0}");

        SetText(hpRecoveryText, $"HP Recovery : {playerStats.hpRecoveryRate:F1} / sec");

        SetText(armorText, $"Armor : {playerStats.armor:F0}");
        SetText(revivalText, $"Revival : {playerStats.revivalCount}");

        // --- 2. Offense ---
        SetText(damageText, $"Damage : {playerStats.currentDamageMultiplier:F1}x");

        // Crit Chance
        SetText(critChanceText, $"Crit Chance : {playerStats.currentCritChance * 100f:F0}%");

        // Crit Damage
        SetText(critDamageText, $"Crit Damage : {playerStats.currentCritMultiplier:F1}x");

        // Cooldown
        SetText(cooldownText, $"Cooldown : {playerStats.bonusCooldownReduction * 100f:F0}%");

        // Amount (Projectiles)
        SetText(projectileText, $"Amount : +{playerStats.bonusProjectileCount}");


        // --- 3. Utility ---
        // Area
        float areaTotal = 1.0f + playerStats.bonusArea;
        SetText(areaText, $"Area : {areaTotal:F1}x");

        // Duration
        float durationTotal = 1.0f + playerStats.bonusDuration;
        SetText(durationText, $"Duration : {durationTotal:F1}x");

        // Speed
        SetText(speedText, $"Speed : {playerStats.currentMoveSpeed:F1}");

        // Magnet (Pickup Range)
        SetText(pickupText, $"Magnet : +{playerStats.bonusPickupRange:F1}");

        // Growth (Exp Gain)
        SetText(expGainText, $"Growth : {playerStats.expGainMultiplier:F1}x");


        // --- 4. Session Info ---
        if (GameManager.Instance != null)
        {
            float t = GameManager.Instance.gameTime;
            int min = (int)(t / 60);
            int sec = (int)(t % 60);

            SetText(timeText, $"Time : {min:D2}:{sec:D2}");
            SetText(killText, $"Kills : {GameManager.Instance.killCount}");
        }
    }

    private void SetText(TextMeshProUGUI ui, string content)
    {
        if (ui != null) ui.text = content;
    }
}