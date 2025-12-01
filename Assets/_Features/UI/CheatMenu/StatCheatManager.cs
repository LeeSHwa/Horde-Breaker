using UnityEngine;

public class StatCheatManager : MonoBehaviour
{
    private StatsController playerStats;

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<StatsController>();
            }
        }
    }

    private bool CheckPlayer()
    {
        if (playerStats == null)
        {
            FindPlayer();
            if (playerStats == null) return true;
        }
        return false;
    }

    private void RefreshStats()
    {
        if (playerStats != null) playerStats.Heal(0);
    }

    // ==========================================
    // 1. Survival (Health, Armor, Recov)
    // ==========================================

    public void Cheat_LevelUp() { if (!CheckPlayer()) playerStats.AddExp(playerStats.MaxExp); }

    public void Cheat_HealFull() { if (!CheckPlayer()) playerStats.Heal(playerStats.MaxHP); }

    // Max HP
    public void Cheat_MaxHPUp() { if (!CheckPlayer()) playerStats.ApplyMaxHealth(10f); }
    public void Cheat_MaxHPDown() { if (!CheckPlayer()) playerStats.ApplyMaxHealth(-10f); }

    // Recovery
    public void Cheat_RecoveryUp() { ModifyRecovery(0.5f); }
    public void Cheat_RecoveryDown() { ModifyRecovery(-0.5f); }
    private void ModifyRecovery(float amount)
    {
        if (CheckPlayer()) return;
        playerStats.hpRecoveryRate += amount;
        RefreshStats();
    }

    // Armor
    public void Cheat_ArmorUp() { ModifyArmor(1f); }
    public void Cheat_ArmorDown() { ModifyArmor(-1f); }
    private void ModifyArmor(float amount)
    {
        if (CheckPlayer()) return;
        playerStats.armor += amount;
        RefreshStats();
    }

    // Revival
    public void Cheat_RevivalUp() { ModifyRevival(1); }
    public void Cheat_RevivalDown() { ModifyRevival(-1); }
    private void ModifyRevival(int amount)
    {
        if (CheckPlayer()) return;
        playerStats.revivalCount += amount;
        if (playerStats.revivalCount < 0) playerStats.revivalCount = 0;
        RefreshStats();
    }

    // ==========================================
    // 2. Offense (Dmg, Crit, Cool, Proj)
    // ==========================================

    // Damage
    public void Cheat_DamageUp() { if (!CheckPlayer()) playerStats.ApplyDamageMultiplier(0.1f); }
    public void Cheat_DamageDown() { if (!CheckPlayer()) playerStats.ApplyDamageMultiplier(-0.1f); }

    // Crit Chance
    public void Cheat_CritChanceUp() { if (!CheckPlayer()) playerStats.ApplyCritStats(0.05f, 0f); }
    public void Cheat_CritChanceDown() { if (!CheckPlayer()) playerStats.ApplyCritStats(-0.05f, 0f); }

    // Crit Damage
    public void Cheat_CritDamageUp() { if (!CheckPlayer()) playerStats.ApplyCritStats(0f, 0.2f); }
    public void Cheat_CritDamageDown() { if (!CheckPlayer()) playerStats.ApplyCritStats(0f, -0.2f); }

    // Cooldown
    public void Cheat_CooldownUp() // Reduces cooldown (Good)
    {
        if (CheckPlayer()) return;
        playerStats.bonusCooldownReduction += 0.05f;
        if (playerStats.bonusCooldownReduction > 0.9f) playerStats.bonusCooldownReduction = 0.9f;
        RefreshStats();
    }
    public void Cheat_CooldownDown() // Increases cooldown (Bad)
    {
        if (CheckPlayer()) return;
        playerStats.bonusCooldownReduction -= 0.05f;
        RefreshStats();
    }

    // Projectile
    public void Cheat_ProjectileUp() { ModifyProjectile(1); }
    public void Cheat_ProjectileDown() { ModifyProjectile(-1); }
    private void ModifyProjectile(int amount)
    {
        if (CheckPlayer()) return;
        playerStats.bonusProjectileCount += amount;
        RefreshStats();
    }

    // ==========================================
    // 3. Utility (Area, Speed, Pickup, Exp, Time)
    // ==========================================

    // Area
    public void Cheat_AreaUp() { ModifyArea(0.1f); }
    public void Cheat_AreaDown() { ModifyArea(-0.1f); }
    private void ModifyArea(float amount)
    {
        if (CheckPlayer()) return;
        playerStats.bonusArea += amount;
        RefreshStats();
    }

    // Duration
    public void Cheat_DurationUp() { ModifyDuration(0.1f); }
    public void Cheat_DurationDown() { ModifyDuration(-0.1f); }
    private void ModifyDuration(float amount)
    {
        if (CheckPlayer()) return;
        playerStats.bonusDuration += amount;
        RefreshStats();
    }

    // Speed
    public void Cheat_SpeedUp() { if (!CheckPlayer()) playerStats.ApplyMoveSpeed(1.0f); }
    public void Cheat_SpeedDown() { if (!CheckPlayer()) playerStats.ApplyMoveSpeed(-1.0f); }

    // Pickup (Renamed from Magnet)
    public void Cheat_PickupUp() { ModifyPickup(1.0f); }
    public void Cheat_PickupDown() { ModifyPickup(-1.0f); }
    private void ModifyPickup(float amount)
    {
        if (CheckPlayer()) return;
        playerStats.bonusPickupRange += amount;
        playerStats.InitializeStats(); // Apply to Collider
    }

    // Exp Gain
    public void Cheat_ExpGainUp() { ModifyExpGain(0.2f); }
    public void Cheat_ExpGainDown() { ModifyExpGain(-0.2f); }
    private void ModifyExpGain(float amount)
    {
        if (CheckPlayer()) return;
        playerStats.expGainMultiplier += amount;
        RefreshStats();
    }

    // Lifetime (Game Time Control)
    public void Cheat_LifetimeUp() { ModifyTime(30f); } // +1 Minute
    public void Cheat_LifetimeDown() { ModifyTime(-30f); } // -1 Minute
    private void ModifyTime(float seconds)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameTime += seconds;
            if (GameManager.Instance.gameTime < 0) GameManager.Instance.gameTime = 0;

            // Optional: Force UI update if GameManager doesn't update UI every frame
            // usually not needed if Update() handles it.
        }
    }
}