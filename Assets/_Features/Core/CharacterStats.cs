// FileName: CharacterStats.cs
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // A header for organization in the Inspector.
    // 인스펙터 창에서 구분을 위한 헤더입니다.
    [Header("Core Stats")]
    // The maximum health of the character.
    // 캐릭터의 최대 체력입니다.
    public float maxHealth = 100f;
    // The character's current movement speed. This can be changed in real-time by buffs/debuffs.
    // 캐릭터의 현재 이동 속도입니다. 버프/디버프에 의해 실시간으로 변경될 수 있습니다.
    public float moveSpeed = 5f;

    // This is the character's 'pure base' movement speed, unaffected by buffs/debuffs. (Read-only)
    // 버프/디버프의 영향을 받지 않는 이 캐릭터의 '순수한 기본' 이동 속도입니다. (읽기 전용)
    public float BaseMoveSpeed { get; private set; }

    // Hides this variable in the Inspector.
    // 인스펙터 창에서 이 변수를 숨깁니다.
    [HideInInspector]
    // The character's current health.
    // 캐릭터의 현재 체력입니다.
    public float currentHealth;

    // This function is called once when the script instance is being loaded. It's used for initialization.
    // 스크립트 인스턴스가 로드될 때 한 번 호출됩니다. 초기화에 사용됩니다.
    void Awake()
    {
        // Initialize current health to the maximum health at the start.
        // 현재 체력을 최대 체력으로 초기화합니다.
        currentHealth = maxHealth;

        // When the game starts, store the initial moveSpeed value from the Inspector into BaseMoveSpeed just once to serve as the initial baseline.
        // 게임이 시작될 때, 인스펙터에 설정된 moveSpeed 값을 BaseMoveSpeed에 한 번만 저장하여 초기 기준값으로 삼습니다.
        BaseMoveSpeed = moveSpeed;
    }

    // --- Function newly added ---
    // Updates the character's permanent base movement speed due to leveling up, equipping items, etc.
    // 레벨업이나 장비 장착 등으로 캐릭터의 영구적인 기본 이동 속도를 업데이트합니다.
    public void UpdateBaseMoveSpeed(float newBaseSpeed)
    {
        // Permanently changes the base speed to the new value.
        // 새로운 값으로 기본 속도를 영구적으로 변경합니다.
        BaseMoveSpeed = newBaseSpeed;

        // --- CORE MODIFICATION ---
        // The responsibility of checking for active effects and adjusting moveSpeed
        // has been moved to the SpeedEffectController.
        // This script no longer needs to check for TemporarySpeedEffect.
        // The controller will automatically recalculate the speed.
        // --- 핵심 수정 부분 ---
        // 현재 효과가 있는지 확인하고 moveSpeed를 조정하는 책임은
        // SpeedEffectController로 이전되었습니다.
        // 이 스크립트는 더 이상 TemporarySpeedEffect를 확인할 필요가 없습니다.
        // 컨트롤러가 자동으로 속도를 재계산할 것입니다.
        //
        // Note: We don't directly set `moveSpeed = BaseMoveSpeed;` here because a speed effect might be active.
        // The SpeedEffectController is responsible for applying the correct speed,
        // so we just update the base value and let the controller handle the rest.
        // 참고: 여기서 `moveSpeed = BaseMoveSpeed;`로 직접 설정하지 않습니다. 속도 효과가 활성화 상태일 수 있기 때문입니다.
        // 올바른 속도를 적용하는 것은 SpeedEffectController의 역할이므로,
        // 여기서는 기본값만 업데이트하고 나머지는 컨트롤러가 처리하도록 둡니다.
    }


    // Reduces the character's health by the given damage amount.
    // 주어진 데미지만큼 캐릭터의 체력을 감소시킵니다.
    public void TakeDamage(float damage)
    {
        // If the damage value is less than 0, do nothing.
        // 데미지 값이 0보다 작으면 아무 처리도 하지 않습니다.
        if (damage < 0) return;

        // Subtract the damage from the current health.
        // 현재 체력에서 데미지를 뺍니다.
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // If health drops to 0 or below
        // 만약 체력이 0 이하로 떨어졌다면
        if (currentHealth <= 0)
        {
            // Clamp the health to 0 so it doesn't go negative.
            // 체력이 음수가 되지 않도록 0으로 고정합니다.
            currentHealth = 0;

            // Call the death handling function.
            // 죽음 처리 함수를 호출합니다.
            Die();
        }
    }

    // Handles the death of the character.
    // 캐릭터의 죽음을 처리합니다.
    private void Die()
    {
        Debug.Log(transform.name + " has died.");

        // For now, just destroy the GameObject.
        // 지금은 단순히 게임 오브젝트를 파괴합니다.
        Destroy(gameObject);
    }
}