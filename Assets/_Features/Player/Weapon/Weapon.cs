using UnityEngine;

// This is the base blueprint for all weapons. // 모든 무기를 위한 기본 설계도.
public abstract class Weapon : MonoBehaviour
{
    [Header("Common Stats")] // 공통 능력치
    [Tooltip("Damage dealt per hit.")] // 공격 당 데미지.
    public float damage = 10f;

    [Tooltip("Cooldown time between attacks in seconds.")] // 공격 사이의 쿨다운 시간(초).
    public float attackCooldown = 0.5f;

    [Tooltip("Force applied to enemies on hit.")] // 피격된 적에게 가해지는 힘.
    public float knockback = 5f;

    protected float lastAttackTime; // The time of the last attack. // 마지막 공격 시간.

    // A virtual method that can be overridden by child classes. // 자식 클래스에서 오버라이드 할 수 있는 가상 메소드.
    // This method checks the cooldown and initiates the attack. // 이 메소드는 쿨다운을 확인하고 공격을 시작함.
    public virtual void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    // An abstract method that MUST be implemented by child classes. // 자식 클래스에서 반드시 구현해야 하는 추상 메소드.
    // This defines the actual attack behavior (e.g., swinging, shooting). // 실제 공격 행동(휘두르기, 쏘기 등)을 정의함.
    protected abstract void PerformAttack();
}

