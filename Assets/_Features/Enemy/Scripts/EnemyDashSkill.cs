using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))] // 반드시 EnemyMovement와 함께 있어야 함
public class EnemyDashSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float triggerRange = 5f;    // 돌진을 시도할 거리
    public float chargeTime = 0.5f;    // 돌진 전 준비 시간 (웅크리기)
    public float dashSpeed = 20f;      // 돌진 속도
    public float dashDuration = 0.3f;  // 돌진 지속 시간
    public float skillCooldown = 5.0f; // 스킬 재사용 대기시간

    [Header("Visuals (Optional)")]
    public Color chargeColor = Color.red; // 차징 중 색상 변경 예시
    private Color originalColor;

    // 내부 변수
    private EnemyMovement movementScript;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform playerTarget;
    private bool isSkillAvailable = true;
    private bool isDashing = false;

    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        // 쿨타임 중이거나 이미 돌진 중이면 무시
        if (!isSkillAvailable || isDashing || playerTarget == null) return;

        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, playerTarget.position);

        // 사정거리 안에 들어오면 스킬 발동
        if (distance <= triggerRange)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        isSkillAvailable = false;

        // 1. [제어권 가져오기] 기존 이동 AI 끄기
        movementScript.enabled = false;

        // 정지 (관성 제거)
        rb.linearVelocity = Vector2.zero;

        // 2. [차징] 준비 동작 (예: 색상 변경, 애니메이션 트리거 등)
        if (sr != null) sr.color = chargeColor;
        // if (anim != null) anim.SetTrigger("Charge"); 

        yield return new WaitForSeconds(chargeTime);

        // 3. [돌진] 목표 방향 계산 및 발사
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        rb.AddForce(direction * dashSpeed, ForceMode2D.Impulse);

        // (선택) 돌진 중 색상 복구 또는 대시 이펙트
        // if (sr != null) sr.color = originalColor;

        yield return new WaitForSeconds(dashDuration);

        // 4. [복구] 미끄러짐 방지를 위해 속도 줄이기 (선택)
        rb.linearVelocity = Vector2.zero;
        if (sr != null) sr.color = originalColor;

        // 5. [제어권 반납] 기존 이동 AI 다시 켜기
        movementScript.enabled = true;
        isDashing = false;

        // 6. [쿨타임] 대기
        yield return new WaitForSeconds(skillCooldown);
        isSkillAvailable = true;
    }
}
