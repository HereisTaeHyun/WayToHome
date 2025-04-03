using System;
using System.Collections;
using System.Collections.Generic;
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
    [NonSerialized] public float moveSpeed = 7.0f;
    [NonSerialized] public int maxJump;

#region private
    // private 변수
    private Rigidbody2D rb2D;
    private PlayerCtrl playerCtrl;

    private Vector2 newVelocity;
    private float jumpSpeed = 5.0f;
    private int jumpCount = 0;

    // 땅인지 체크하는 Ray 시작 위치
    private Vector2 groundCheckStartPos;
    [SerializeField] private float groundCheckDistance = 0.5f;

    private bool isPlatform;
    private static float DISABLE_COLLIDER_TIME = 0.5f;

    private Animator playerAnim;

    private CapsuleCollider2D coll2D;
    private Vector2 collSize;
    private float slopeCheckDistance = 0.5f;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private AudioClip jumpSFX;

    // 애니메이션 읽기 해시
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int dirHash = Animator.StringToHash("MoveDir");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    // 다른 객체에서 읽기 필요한 변수
    [SerializeField] private bool isGround;
    public bool readIsGround {get {return isGround;}}
    [SerializeField] private bool isJump;
    public bool readIsJump {get {return isJump;}}
    [SerializeField] private bool isSlope;
    private float originSpeed = 7.0f;
    public float readOriginSpeed {get {return originSpeed;}}
    private float debuffedSpeed; // origin * 0.5f
    public float readDebuffedSpeed {get {return debuffedSpeed;}}
#endregion


    public void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerCtrl = GetComponent<PlayerCtrl>();
        playerAnim = GetComponent<Animator>();
        moveSpeed = originSpeed;
        debuffedSpeed = moveSpeed * 0.5f;

        isPlatform = false;

        coll2D = GetComponent<CapsuleCollider2D>();
        collSize = coll2D.size;
    }

#region HorizontalMove
    // 좌우 이동 메서드
    public void HorizontalMove()
    {
        // 이동 방향 지정
        float h = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(h, 0);
        Vector2 moveDir = UtilityManager.utility.DirSet(move);

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

        // 플레이어가 있는 Ground 상태를 알기 위해 스캐닝하는 위치
        Vector2 checkPos = transform.position - new Vector3(0.0f, collSize.y / 2);

        // 이동에 필요한 정보 스캐닝
        HorizontalSlopeCheck(checkPos);
        VerticalSlopeCheck(checkPos);

        Debug.DrawRay(new Vector2(rb2D.position.x, rb2D.position.y - (collSize.y / 2)), Vector2.down * groundCheckDistance, Color.blue);

        // 실제 이동 적용 부분
        if(Input.GetButton("Horizontal"))
        {
            ApplyMove(move, moveDir);
        }
    }

    private void ApplyMove(Vector2 move, Vector2 moveDir)
    {
        playerAnim.SetFloat(dirHash, moveDir.x);

        // Ground, Slope 위면 얻어진 수직 벡터 각도에 따라 이동, 아니면 그냥 이전 velocity에 따라 이동
        if(isGround == true && isSlope != true)
        {
            newVelocity.Set(move.x * moveSpeed, 0.0f);
            rb2D.linearVelocity = newVelocity;
        }
        else if(isGround == true && isSlope == true)
        {
            newVelocity.Set(-move.x * moveSpeed * slopeNormalPerp.x, -move.x * moveSpeed * slopeNormalPerp.y);
            rb2D.linearVelocity = newVelocity;
        }
        else if(isGround == false)
        {
            newVelocity.Set(move.x * moveSpeed, rb2D.linearVelocity.y);
            rb2D.linearVelocity = newVelocity;
        }
    }
#endregion

#region slopeChecker
    // 수평 언덕 체크 메서드
    private void HorizontalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

        // 잎 or 뒤에 Ground check가 돠면 == 언덕이 앞에 있음
        if (slopeHitFront)
        {
            isSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        }
        else if (slopeHitBack)
        {
            isSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isSlope = false;
        }

    }
    // 직선 언덕 체크 메서드
    private void VerticalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);
        if(hit2D)
        {
            // 수직 2D 벡터 반환 받기
            slopeNormalPerp = Vector2.Perpendicular(hit2D.normal).normalized;            
            slopeDownAngle = Vector2.Angle(hit2D.normal, Vector2.up);

            // 이전 경사 정보와 달라지면 == 경사에 올라옴
            if(slopeDownAngle != lastSlopeAngle)
            {
                isSlope = true;
            }                       
            lastSlopeAngle = slopeDownAngle;
        }
    }
# endregion

#region JumpGround
    // 점프 및 Ground 체크에 필요한 메서드들
    // Ground에 접촉하면 JumpCount 초기화
    private void OnCollisionStay2D(Collision2D other)
    {
        // 땅이나 플랫폼에 닿았음
        if(other.collider.CompareTag("Ground") || other.collider.CompareTag("Platform"))
        {
            // jumpCount가 초기화되지 않았고 하강 중임
            // rb2D.linearVelocity.y < 0.01f 없으면 점프 키를 누른 프레임때도 초기화해서 2중 점프됨 삭제하지 말 것
            if(footOnGround())
            {
                isJump = false;
                isGround = true;
                jumpCount = 0;
            }
        }
        // Platform 위지만 isPlatform이 아니면 isPlatform
        if(other.gameObject.CompareTag("Platform") && isPlatform == false)
        {
            isPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Platform"))
        {
            if (gameObject.activeInHierarchy)
            {
                Collider2D col = other.collider; // 미리 복사
                StartCoroutine(GroundCheck(col));
            }
        }
    }

    // 삭제 금지 필요 없어 보이는데 얘 지우면 허공답보 버그남
    IEnumerator GroundCheck(Collider2D col)
    {
        yield return new WaitForSeconds(0.05f);

        if (col != null && (col.CompareTag("Ground") || col.CompareTag("Platform")))
        {
            isGround = false;
            isJump = true;
        }
    }

    // 땅이 발에 닿는지 체크
    private bool footOnGround()
    {
        // Ground, Platform 체크
        groundCheckStartPos = new Vector2(rb2D.position.x, rb2D.position.y - (collSize.y / 2));
        RaycastHit2D hit = Physics2D.Raycast(groundCheckStartPos, Vector2.down, groundCheckDistance, groundLayer | platformLayer);

        // 둘 중 하나라도 체크가 되면
        if(hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // jumpCount가 있으며 Jump 입력 받으면
    public void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < maxJump) // W에 할당된 "Jump"를 눌러 maxJump까지 점프가능
        {
            // jumpCount 추가 후 jump
            isJump = true;
            jumpCount += 1;

            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpSpeed);
            UtilityManager.utility.PlaySFX(jumpSFX);
            playerAnim.SetTrigger(jumpHash);
        }
    }
#endregion

#region Platform
    public void GoDownPlatfom()
    {
        // Vertical negative 키를 눌렀다면, platform 위라면
        if(Input.GetAxis("Vertical") < 0 && isPlatform == true)
        {
            StartCoroutine(DisablePlatformCollider());
        }
    }

    // 아래 키가 눌린다면 PlatformCollider를 일시적으로 무시 후 되살리기
    IEnumerator DisablePlatformCollider()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), true);
        yield return new WaitForSeconds(DISABLE_COLLIDER_TIME);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), false);
    }
    #endregion
}