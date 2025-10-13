using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Camera mainCam;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1. 마우스 방향 계산 ---
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPoint = mainCam.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (worldPoint - (Vector2)transform.position).normalized;

        // 2. 방향에 따라 Animator의 moveX, moveY 파라미터 설정
        // 키보드 입력이 아닌 마우스 방향을 기준으로 애니메이션을 결정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 각도 범위를 4등분하여 방향을 결정
        if (angle > -45 && angle <= 45)
        {
            // 오른쪽
            animator.SetFloat("moveX", 1f);
            animator.SetFloat("moveY", 0f);
            spriteRenderer.flipX = false; // 오른쪽 볼 때 스프라이트 반전 없음
        }
        else if (angle > 45 && angle <= 135)
        {
            // 위쪽 (뒷모습)
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 1f);
        }
        else if (angle > 135 || angle <= -135)
        {
            // 왼쪽
            animator.SetFloat("moveX", -1f);
            animator.SetFloat("moveY", 0f);
            spriteRenderer.flipX = true; // 왼쪽 볼 때 스프라이트 X축 반전
        }
        else if (angle > -135 && angle <= -45)
        {
            // 아래쪽 (앞모습)
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", -1f);
        }

        // --- 3. 움직임 상태를 isMoving 파라미터에 전달 ---
        // Rigidbody의 속력을 확인하여 움직이고 있는지 판단
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}