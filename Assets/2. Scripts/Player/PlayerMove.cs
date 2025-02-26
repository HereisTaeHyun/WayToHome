using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    // 움직임 관련 객체
    // 걷기, 달리기, 점프, 대쉬 정도 생각 중

    // public 변수
    // 아래들은 디버프 및 아이템에 의한 증감 있음 or 예정
    public float moveSpeed = 7.0f;
    public int maxJump = 1;
    public bool isGround;
    public bool isJump;

    // private 변수
    private Rigidbody2D rb;
    private PlayerCtrl playerCtrl;

    private Vector2 newVelocity;
    private float jumpSpeed = 5.0f;
    private int jumpCount = 0;

    private Animator playerAnim;

    private PhysicsMaterial2D physicsMaterial2D;
    private CapsuleCollider2D coll2D;
    private Vector2 collSize;
    [SerializeField] private float slopeCheckDistance = 0.5f;
    private Vector2 slopeNormalPerp;
    private float slopeAngle;
    [SerializeField] private LayerMask groundLayer;

    // 애니메이션 읽기 해시
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int dirHash = Animator.StringToHash("MoveDir");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    // 다른 객체에서 읽기 필요한 변수
    private float originSpeed = 7.0f;
    public float readOriginSpeed {get {return originSpeed;}}
    private float debuffedSpeed; // origin * 0.5f
    public float readDebuffedSpeed {get {return debuffedSpeed;}}


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCtrl = GetComponent<PlayerCtrl>();
        playerAnim = GetComponent<Animator>();
        moveSpeed = originSpeed;
        debuffedSpeed = moveSpeed * 0.5f;
        physicsMaterial2D = new PhysicsMaterial2D();

        coll2D = GetComponent<CapsuleCollider2D>();
        collSize = coll2D.size;
    }

    // 좌우 이동 메서드
    public void HorizontalMove()
    {
        // 이동 방향 지정
        float h = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(h, 0);
        Vector2 moveDir = playerCtrl.MoveDirSet(move);

        // 이동 방향이 left 쪽이면 Player가 왼쪽으로 보기
        playerAnim.SetFloat(speedHash, move.magnitude);

        // player state가 idle인지 move인지 h에 따라 변화
        if(h != 0)
        {
            playerCtrl.state = PlayerCtrl.State.Move;
        }
        else if(h == 0)
        {
            playerCtrl.state = PlayerCtrl.State.Idle;
        }

        // 정지 상태이면 마찰력 증가, 그래야 멈출때랑 경사에서 안미끄러짐
        if(playerCtrl.state == PlayerCtrl.State.Idle)
        {
            physicsMaterial2D.friction = 5.0f;
            coll2D.sharedMaterial = physicsMaterial2D;
        }
        else if(playerCtrl.state == PlayerCtrl.State.Move)
        {
            physicsMaterial2D.friction = 1.8f;
            coll2D.sharedMaterial = physicsMaterial2D;
        }

        // 경사 이동인지 알기 위해 이동 각도 구함
        Vector2 checkPos = transform.position - new Vector3(0.0f, collSize.y / 2);

        // x 이동 방향에 따라 조금 더 앞에서 스캐닝, 그래야 걸림 적어짐
        if(move.x > 0)
        {
            checkPos.x += 0.14f;
            VerticalSlopeCheck(checkPos);
        }
        else if(move.x < 0)
        {
            checkPos.x -= 0.14f;
            VerticalSlopeCheck(checkPos);
        }
        // 실제 이동 함수
        if(Input.GetButton("Horizontal"))
        {
            playerAnim.SetFloat(dirHash, moveDir.x);

            // Ground 위면 얻어진 각도에 따라 이동, 아니면 그냥 이전 velocity에 따라 이동
            if(isGround == true)
            {
                newVelocity.Set(-move.x * moveSpeed * slopeNormalPerp.x, -move.x * moveSpeed * slopeNormalPerp.y);
                rb.linearVelocity = newVelocity;
            }
            else if(isGround == false)
            {
                newVelocity.Set(move.x * moveSpeed, rb.linearVelocity.y);
                rb.linearVelocity = newVelocity;
            }
        }
    }

    // 경사 체크 메서드
    private void VerticalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);
        if(hit2D)
        {
            // 수직 2D 벡터 반환 받기
            slopeNormalPerp = Vector2.Perpendicular(hit2D.normal).normalized;
        }
    }

    // 점프에 필요한 메서드들
    // Ground 위면 JumpCount 초기화
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            isGround = true;
            jumpCount = 0;
            isJump = false;
        }   
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGround = false; // 땅에서 떨어지면 isGround = false 설정
        }
    }
    public void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < maxJump) // W에 할당된 "Jump"를 눌러 maxJump까지 점프가능
        {
            // jumpCount 추가 후 jump
            isJump = true;
            jumpCount += 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            isGround = false;

            playerAnim.SetTrigger(jumpHash);
        }
    }
}