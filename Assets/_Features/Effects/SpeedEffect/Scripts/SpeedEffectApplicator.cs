// FileName: SpeedEffectApplicator.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SpeedEffectApplicator : MonoBehaviour
{
    [Header("효과 설정")]
    [Tooltip("오브젝트의 투명도입니다. 0(투명) ~ 1(불투명)")]
    [Range(0f, 1f)]
    public float alpha = 0.5f;

    [Tooltip("체크하면 통과 가능한 지역(Trigger)이 되고, 해제하면 충돌하는 오브젝트가 됩니다.")]
    public bool isTrigger = true;

    [Header("속도 변경 설정")]
    [Tooltip("이 효과의 종류를 선택하세요. Zone은 범위 내에 있을 때만, Timed는 지정된 시간 동안 지속됩니다.")]
    public SpeedEffectController.EffectType effectType = SpeedEffectController.EffectType.Timed;

    [Tooltip("이동 속도 변경 비율입니다. 100을 기준으로, 70은 30% 둔화, 120은 20% 가속입니다.")]
    public float speedModifierPercentage = 70f;

    [Tooltip("효과 지속 시간입니다. Zone 타입은 0.2초 같이 짧은 값으로, Timed 타입은 원하는 지속시간으로 설정하세요.")]
    public float effectDuration = 3f;

    private void Awake()
    {
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }
        if (TryGetComponent<Collider2D>(out var col))
        {
            col.isTrigger = isTrigger;
        }
    }

    // --- Collision and Trigger Logic ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        HandleContact(other.gameObject);

    }

    // For Zone types, continuously update the effect while the target stays within the area.
    // Zone 타입의 경우, 범위 안에 머무르는 동안 계속 효과를 갱신해줍니다.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (effectType == SpeedEffectController.EffectType.Zone)
        {
            HandleContact(other.gameObject);
        }
    }

    // For Zone types, immediately remove the effect when the target exits the area.
    // Zone 타입의 경우, 범위를 벗어나면 즉시 효과를 제거합니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (effectType == SpeedEffectController.EffectType.Zone)
        {
            if (other.TryGetComponent<SpeedEffectController>(out var controller))
            {
                // Request the controller to remove the effect caused by 'this' object.
                // 컨트롤러에게 '나(this)'로 인해 발생한 효과를 제거해달라고 요청합니다.
                controller.RemoveEffect(this);
            }
        }
    }

    // A common function to request the application of a speed effect to a target.
    // 대상에게 속도 효과 적용을 요청하는 공통 함수입니다.
    private void HandleContact(GameObject target)
    {
        // Do not affect other effect-applying objects.
        // 다른 효과 적용 오브젝트에게는 영향을 주지 않습니다.
        if (target.GetComponent<SpeedEffectApplicator>() != null) return;

        // Check if the target has a SpeedEffectController and, if so, request it to apply the effect.
        // 대상에게 SpeedEffectController가 있는지 확인하고, 있다면 효과 적용을 요청합니다.
        if (target.TryGetComponent<SpeedEffectController>(out var controller))
        {
            // Request the controller to apply or update the effect, using 'this' instance as the source.
            // 컨트롤러에게 '나 자신(this)'을 소스로 하여 효과를 적용하거나 갱신해달라고 요청합니다.
            controller.ApplyOrUpdateEffect(this, speedModifierPercentage, effectDuration, effectType);
        }
    }
}