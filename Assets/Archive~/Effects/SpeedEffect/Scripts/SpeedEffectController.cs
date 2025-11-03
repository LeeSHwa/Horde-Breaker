// FileName: SpeedEffectController.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CharacterStats))]
public class SpeedEffectController : MonoBehaviour
{
    // Defines the type of effect. Can be selected in the Inspector.
    public enum EffectType { Timed, Zone }

    // An internal class to store data for each active effect.
    private class ActiveEffect
    {
        // The object that caused the effect (e.g., a SpeedEffectApplicator instance).
        public object Source;
        public float SpeedPercentage;
        public float ExpirationTime;
        public EffectType Type;
    }

    private readonly List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    private CharacterStats characterStats;
    // A flag to check if a speed recalculation is needed.
    private bool needsRecalculation = false;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        // Checks for expired 'Timed' effects and removes them from the list.
        // 'Zone' effects are removed directly by the Applicator's OnTriggerExit2D, so they are not handled here.
        int removedCount = activeEffects.RemoveAll(effect => effect.Type == EffectType.Timed && Time.time >= effect.ExpirationTime);

        // If any effects were removed, flag that a recalculation is needed.
        if (removedCount > 0)
        {
            needsRecalculation = true;
        }

        // Call the function only when recalculation is needed to reduce unnecessary operations.
        if (needsRecalculation)
        {
            RecalculateSpeed();
            needsRecalculation = false;
        }
    }

    // A function called from the outside (Applicator) to apply or update an effect.
    public void ApplyOrUpdateEffect(object source, float percentage, float duration, EffectType type)
    {
        var existingEffect = activeEffects.FirstOrDefault(e => e.Source == source);

        if (existingEffect != null)
        {
            // If the effect is already in the list, just update its expiration time (mainly for Zone effects).
            existingEffect.ExpirationTime = Time.time + duration;
        }
        else
        {
            // If it's a new effect not in the list, add it.
            activeEffects.Add(new ActiveEffect
            {
                Source = source,
                SpeedPercentage = percentage,
                ExpirationTime = Time.time + duration,
                Type = type
            });
            // A new effect was added, so a recalculation is needed.
            needsRecalculation = true;
        }

        // Since Zone effects are called every frame via Stay, force a recalculation immediately upon entry.
        if (type == EffectType.Zone)
        {
            needsRecalculation = true;
        }
    }

    // A function called from the outside (Applicator) to remove an effect (mainly used when exiting a Zone).
    public void RemoveEffect(object source)
    {
        int removedCount = activeEffects.RemoveAll(e => e.Source == source);

        // If an effect was successfully removed, a recalculation is needed.
        if (removedCount > 0)
        {
            needsRecalculation = true;
        }
    }

    // The core function that recalculates the player's movement speed based on all current effects.
    private void RecalculateSpeed()
    {
        // 1. Check if there are any active 'Zone' effects.
        var zoneEffects = activeEffects.Where(e => e.Type == EffectType.Zone).ToList();
        if (zoneEffects.Any())
        {
            // If overlapping in multiple zones, apply the one that slows the most (lowest percentage).
            float slowestZonePercentage = zoneEffects.Min(e => e.SpeedPercentage);
            characterStats.moveSpeed = characterStats.BaseMoveSpeed * (slowestZonePercentage / 100f);
            // Zone effects have top priority, so finish the calculation here.
            return;
        }

        // 2. If no 'Zone' effects, check for active 'Timed' effects.
        var timedEffects = activeEffects.Where(e => e.Type == EffectType.Timed).ToList();
        if (timedEffects.Any())
        {
            // If multiple Timed effects are active, apply the one that slows the most.
            float slowestTimedPercentage = timedEffects.Min(e => e.SpeedPercentage);
            characterStats.moveSpeed = characterStats.BaseMoveSpeed * (slowestTimedPercentage / 100f);
            return;
        }

        // 3. If no effects are active, restore the character's speed to the base speed.
        characterStats.moveSpeed = characterStats.BaseMoveSpeed;
    }
}