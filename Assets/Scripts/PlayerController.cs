using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동 속도
    public float moveSpeed = 5f;

    // Rigidbody : 물리엔진을 통해 이동하는 방식의 객체 변수
    private Rigidbody2D rb;

    // 입력받은 방향을 저장할 변수
    private Vector2 moveInput;

    void Start()
    {
        // 게임 오브젝트에 붙어있는 Rigidbody2D 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 키보드 입력받기
        // GetAxisRaw : -1, 0, 1 값만 반환 (즉각적인 움직임)
        float moveX = Input.GetAxisRaw("Horizontal"); // 좌우 입력
        float moveY = Input.GetAxisRaw("Vertical");   // 상하 입력
        
        // 2. 입력받은 값을 Vector2 형태로 저장하고 정규화 (대각선 이동시에도 동일한 속도)
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        // 3. Rigidbody2D를 이용해 물리적으로 이동
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
