// FileName: SpeedEffectController.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CharacterStats))]
public class SpeedEffectController : MonoBehaviour
{
    // Defines the type of effect. Can be selected in the Inspector.
    // 효과의 종류를 정의합니다. Inspector 창에서 선택할 수 있습니다.
    public enum EffectType { Timed, Zone }

    // An internal class to store data for each active effect.
    // 현재 활성화된 개별 효과의 데이터를 저장하는 내부 클래스입니다.
    private class ActiveEffect
    {
        // The object that caused the effect (e.g., a SpeedEffectApplicator instance).
        // 효과를 발생시킨 오브젝트 (예: SpeedEffectApplicator).
        public object Source;
        public float SpeedPercentage;
        public float ExpirationTime;
        public EffectType Type;
    }

    private readonly List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    private CharacterStats characterStats;
    // A flag to check if a speed recalculation is needed.
    // 속도 재계산이 필요한지 확인하는 플래그입니다.
    private bool needsRecalculation = false;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        // Checks for expired 'Timed' effects and removes them from the list.
        // 만료된 'Timed' 효과가 있는지 확인하고 리스트에서 제거합니다.
        // 'Zone' effects are removed directly by the Applicator's OnTriggerExit2D, so they are not handled here.
        // 'Zone' 효과는 Applicator의 OnTriggerExit2D에서 직접 제거되므로 여기서는 처리하지 않습니다.
        int removedCount = activeEffects.RemoveAll(effect => effect.Type == EffectType.Timed && Time.time >= effect.ExpirationTime);

        // If any effects were removed, flag that a recalculation is needed.
        // 제거된 효과가 있다면 속도 재계산이 필요하다고 표시합니다.
        if (removedCount > 0)
        {
            needsRecalculation = true;
        }

        // Call the function only when recalculation is needed to reduce unnecessary operations.
        // 재계산이 필요할 때만 함수를 호출하여 불필요한 연산을 줄입니다.
        if (needsRecalculation)
        {
            RecalculateSpeed();
            needsRecalculation = false;
        }
    }

    // A function called from the outside (Applicator) to apply or update an effect.
    // 외부(Applicator)에서 호출하여 효과를 적용하거나 갱신하는 함수입니다.
    public void ApplyOrUpdateEffect(object source, float percentage, float duration, EffectType type)
    {
        var existingEffect = activeEffects.FirstOrDefault(e => e.Source == source);

        if (existingEffect != null)
        {
            // If the effect is already in the list, just update its expiration time (mainly for Zone effects).
            // 이미 리스트에 있는 효과라면 만료 시간만 갱신합니다. (주로 Zone 효과).
            existingEffect.ExpirationTime = Time.time + duration;
        }
        else
        {
            // If it's a new effect not in the list, add it.
            // 리스트에 없는 새로운 효과라면 추가합니다.
            activeEffects.Add(new ActiveEffect
            {
                Source = source,
                SpeedPercentage = percentage,
                ExpirationTime = Time.time + duration,
                Type = type
            });
            // A new effect was added, so a recalculation is needed.
            // 새로운 효과가 추가되었으므로 속도 재계산이 필요합니다.
            needsRecalculation = true;
        }

        // Since Zone effects are called every frame via Stay, force a recalculation immediately upon entry.
        // Zone 효과는 매 프레임 Stay에서 호출되므로, 들어오는 즉시 속도를 재계산하도록 합니다.
        if (type == EffectType.Zone)
        {
            needsRecalculation = true;
        }
    }

    // A function called from the outside (Applicator) to remove an effect (mainly used when exiting a Zone).
    // 외부(Applicator)에서 호출하여 효과를 제거하는 함수입니다. (주로 Zone에서 나갈 때 사용).
    public void RemoveEffect(object source)
    {
        int removedCount = activeEffects.RemoveAll(e => e.Source == source);

        // If an effect was successfully removed, a recalculation is needed.
        // 제거된 효과가 있다면 속도 재계산이 필요합니다.
        if (removedCount > 0)
        {
            needsRecalculation = true;
        }
    }

    // The core function that recalculates the player's movement speed based on all current effects.
    // 현재 적용된 모든 효과를 바탕으로 플레이어의 이동 속도를 다시 계산하는 핵심 함수입니다.
    private void RecalculateSpeed()
    {
        // 1. Check if there are any active 'Zone' effects.
        // 1. 활성화된 'Zone' 효과가 있는지 확인합니다.
        var zoneEffects = activeEffects.Where(e => e.Type == EffectType.Zone).ToList();
        if (zoneEffects.Any())
        {
            // If overlapping in multiple zones, apply the one that slows the most (lowest percentage).
            // 여러 장판 위에 겹쳐있을 경우, 가장 느리게 만드는 효과(가장 낮은 percentage)를 적용합니다.
            float slowestZonePercentage = zoneEffects.Min(e => e.SpeedPercentage);
            characterStats.moveSpeed = characterStats.BaseMoveSpeed * (slowestZonePercentage / 100f);
            // Zone effects have top priority, so finish the calculation here.
            // Zone 효과가 최우선이므로 여기서 계산을 마칩니다.
            return;
        }

        // 2. If no 'Zone' effects, check for active 'Timed' effects.
        // 2. 'Zone' 효과가 없다면, 활성화된 'Timed' 효과가 있는지 확인합니다.
        var timedEffects = activeEffects.Where(e => e.Type == EffectType.Timed).ToList();
        if (timedEffects.Any())
        {
            // If multiple Timed effects are active, apply the one that slows the most.
            // 여러 Timed 효과에 중첩된 경우, 가장 느리게 만드는 효과를 적용합니다.
            float slowestTimedPercentage = timedEffects.Min(e => e.SpeedPercentage);
            characterStats.moveSpeed = characterStats.BaseMoveSpeed * (slowestTimedPercentage / 100f);
            return;
        }

        // 3. If no effects are active, restore the character's speed to the base speed.
        // 3. 적용할 효과가 아무것도 없다면, 캐릭터의 기본 속도로 복구합니다.
        characterStats.moveSpeed = characterStats.BaseMoveSpeed;
    }
}