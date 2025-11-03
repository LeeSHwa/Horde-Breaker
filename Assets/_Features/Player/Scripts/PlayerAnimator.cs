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

        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPoint = mainCam.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (worldPoint - (Vector2)transform.position).normalized;


        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        if (angle > -45 && angle <= 45)
        {
            animator.SetFloat("moveX", 1f);
            animator.SetFloat("moveY", 0f);
            spriteRenderer.flipX = false; 
        }
        else if (angle > 45 && angle <= 135)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 1f);
        }
        else if (angle > 135 || angle <= -135)
        {
            animator.SetFloat("moveX", -1f);
            animator.SetFloat("moveY", 0f);
            spriteRenderer.flipX = true;
        }
        else if (angle > -135 && angle <= -45)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", -1f);
        }

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