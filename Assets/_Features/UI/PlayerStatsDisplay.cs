using UnityEngine;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;

    [Header("Session Info UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killText;

    private StatsController playerStats;

    // 이 패널(LevelUpPanel)이 켜질 때마다 자동으로 실행됩니다.
    private void OnEnable()
    {
        UpdateStatusUI();
    }

    public void UpdateStatusUI()
    {
        // 1. 플레이어 스탯 컨트롤러 가져오기
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerStats = player.GetComponent<StatsController>();
        }

        if (playerStats == null) return;

        // --- 기본 정보 표시 ---
        if (levelText != null)
            levelText.text = $"Lv. {playerStats.Level}";

        if (hpText != null)
            hpText.text = $"HP : {playerStats.currentHP:F0} / {playerStats.MaxHP:F0}";

        if (expText != null)
            expText.text = $"EXP : {playerStats.CurrentExp} / {playerStats.MaxExp}";

        // --- [핵심] 증가량 계산 (기본값 제외하고 보여주기) ---

        if (damageText != null)
        {
            // 기본 배율 1.0(100%)을 뺀 나머지만 표시
            // 예: 1.0 -> 0% / 1.1 -> 10% / 1.5 -> 50%
            float damageBonus = (playerStats.currentDamageMultiplier - 1.0f) * 100f;
            damageText.text = $"Damage : {damageBonus:F0}%";
        }

        if (speedText != null)
        {
            // 이동속도는 현재 속도를 그대로 표시
            // (만약 '+1.0' 처럼 증가분만 보고 싶다면 StatsController에 '기본속도' 변수가 따로 있어야 계산 가능합니다)
            speedText.text = $"Speed : {playerStats.currentMoveSpeed:F1}";
        }

        // --- 게임 진행 정보 (GameManager) ---
        if (GameManager.Instance != null)
        {
            float t = GameManager.Instance.gameTime;
            int min = (int)(t / 60);
            int sec = (int)(t % 60);

            if (timeText != null)
                timeText.text = $"Time : {min:D2}:{sec:D2}";

            if (killText != null)
                killText.text = $"Kills : {GameManager.Instance.killCount}";
        }
    }
}