using Mono.Cecil.Cil;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // public 변수
    Rigidbody2D rigidBody;
    public int maxJump = 1;
    public LayerMask ground;

    // private 변수
    private float moveSpeed = 5.0f;
    private float runSpeed = 10.0f;
    private float jumpSpeed = 10.0f;
    private float groundDistance = 0.1f;
    private bool isGroundLeft;
    private bool isGroundRight;
    private int jumpCount = 0;
    private bool onAir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void MoveCtrl()
    {
        // 수평 입력 값을 받아 방향 벡터 변환 후 Translate로 이동 입력
        float h = Input.GetAxis("Horizontal");

        if(Input.GetButton("Horizontal") && Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(h * runSpeed * Time.deltaTime * Vector2.right);
        }
        else if(Input.GetButton("Horizontal"))
        {
            transform.Translate(h * moveSpeed * Time.deltaTime * Vector2.right);
        }
        
        // 점프 가능 유무 방지를 발에서 시작한 ray가 Ground에 닿는지로 체크 후 점프
        GroundCheck();
        Jump();
    }
    private void GroundCheck()
    {
        Vector2 rayStartLeft = new Vector2(transform.position.x - 0.3f, transform.position.y - 1f);
        Vector2 rayStartRight = new Vector2(transform.position.x + 0.3f, transform.position.y - 1f);
        bool onAir = (isGroundLeft || isGroundRight); // 이전 상태 저장
        isGroundLeft = Physics2D.Raycast(rayStartLeft, Vector2.down, groundDistance, ground);
        isGroundRight = Physics2D.Raycast(rayStartRight, Vector2.down, groundDistance, ground);

        if ((isGroundLeft || isGroundRight) && !onAir) // 땅에 처음 닿았을 때만 점프 카운트 초기화
        {
            jumpCount = 0;
        }
    }

    private void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < maxJump) // ray가 Ground에 닿으면 점프
        {
            jumpCount += 1;
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpSpeed);
            onAir = true;
        }
    }
}