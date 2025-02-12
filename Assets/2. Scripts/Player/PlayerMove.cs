using Mono.Cecil.Cil;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 움직임 관련 객체
    // 걷기, 달리기, 점프, 대쉬 정도 생각 중

    // public 변수
    // 아래들은 디버프 및 아이템에 의한 증감 있음 or 예정
    public float moveSpeed;
    public float runSpeed; // moveSpeed + 3.0f
    public int maxJump = 1;

    // private 변수
    [SerializeField] private LayerMask ground;
    private Rigidbody2D rb;
    private float jumpSpeed = 10.0f;
    private int jumpCount = 0;

    // 다른 객체에서 읽기 필요한 변수
    private float originSpeed = 7.0f;
    public float readOriginSpeed {get {return originSpeed;}}
    private float debuffedSpeed; // origin * 0.5f
    public float readDebuffedSpeed {get {return debuffedSpeed;}}


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = originSpeed;
        runSpeed = originSpeed + 3.0f;
        debuffedSpeed = moveSpeed * 0.5f;
    }

    // 좌우 이동 메서드
    public void HorizontalMove()
    {
        float h = Input.GetAxis("Horizontal");
        if(Input.GetButton("Horizontal") && Input.GetKey(KeyCode.LeftShift)) // 달리기
        {
            rb.linearVelocity = new Vector2(h * runSpeed, rb.linearVelocity.y);
        }
        else if(Input.GetButton("Horizontal")) // 걷기
        {
            rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
        }
    }

    // 점프에 필요한 메서드들
    // Ground 위면 JumpCount 초기화
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            jumpCount = 0;
        }   
    }
    public void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < maxJump) // W에 할당된 "Jump"를 눌러 maxJump까지 점프가능
        {
            jumpCount += 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }
}