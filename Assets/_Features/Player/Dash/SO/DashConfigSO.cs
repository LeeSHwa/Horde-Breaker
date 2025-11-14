using UnityEngine;

[CreateAssetMenu(fileName = "DashConfig", menuName = "Player/Dash Config", order = 1)]
public class DashConfigSO : ScriptableObject
{
    [Header("Dash Stats")]
    public float dashSpeed = 20f;
    public float dashMoveDuration = 0.2f; 
    public float invincibilityDuration = 0.3f; 

    [Header("Stack and Cooldown")]
    public int maxDashStacks = 3;
    public float stackRechargeCooldown = 5.0f;
    public int startingDashStacks = 3;
}