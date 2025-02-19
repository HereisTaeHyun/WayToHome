using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    // 움직임 관련 객체
    // 걷기, 달리기, 점프, 대쉬 정도 생각 중

    // public 변수
    // 아래들은 디버프 및 아이템에 의한 증감 있음 or 예정
    public float moveSpeed = 7.0f;
    public int maxJump = 1;

    // private 변수
    [SerializeField] private LayerMask ground;
    private Rigidbody2D rb;
    private float jumpSpeed = 5.0f;
    private int jumpCount = 0;
    private Animator playerAnim;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDir = new Vector2(1, 0);

    // 애니메이션 읽기 해시
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int dirHash = Animator.StringToHash("MoveDir");

    // 다른 객체에서 읽기 필요한 변수
    private float originSpeed = 7.0f;
    public float readOriginSpeed {get {return originSpeed;}}
    private float debuffedSpeed; // origin * 0.5f
    public float readDebuffedSpeed {get {return debuffedSpeed;}}


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveSpeed = originSpeed;
        debuffedSpeed = moveSpeed * 0.5f;
    }

    // 좌우 이동 메서드
    public void HorizontalMove()
    {
        float h = Input.GetAxis("Horizontal");

        // 이동 방향 지정
        Vector2 move = new Vector2(h, 0);
        if(Mathf.Approximately(move.x, 0) == false)
        {
            moveDir.Set(move.x, 0);
            moveDir.Normalize();
        }

        // 이동 방향이 left 쪽이면 Player 왼쪽으로 보게 해두기

        playerAnim.SetFloat(speedHash, move.magnitude);
        if(Input.GetButton("Horizontal"))
        {
            playerAnim.SetFloat(dirHash, moveDir.x);
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