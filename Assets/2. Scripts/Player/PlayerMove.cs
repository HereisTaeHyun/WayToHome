using Mono.Cecil.Cil;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigidBody;

    // public 변수
    public LayerMask ground;
    // 아래 3개는 아이템에 의한 증감 가능하게 할 생각
    private float moveSpeed = 5.0f;
    private float runSpeed = 10.0f;
    public int maxJump = 1;

    // private 변수
    private float jumpSpeed = 10.0f;
    private float groundDistance = 0.1f;
    private bool isGroundLeft;
    private bool isGroundRight;
    private int jumpCount = 0;
    private bool onAir;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void MoveCtrl()
    {
        // 좌우 이동 메서드
        HorizontalMove();
        
        // 점프 메서드
        GroundCheck();
        Jump();
    }

    private void HorizontalMove()
    {
        float h = Input.GetAxis("Horizontal");

        if(Input.GetButton("Horizontal") && Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(h * runSpeed * Time.deltaTime * Vector2.right);
        }
        else if(Input.GetButton("Horizontal"))
        {
            transform.Translate(h * moveSpeed * Time.deltaTime * Vector2.right);
        }
    }

    private void GroundCheck()
    {
        // Ray가 Ground에 닿으면 땅 위
        Vector2 rayStartLeft = new Vector2(transform.position.x - 0.3f, transform.position.y - 1f);
        Vector2 rayStartRight = new Vector2(transform.position.x + 0.3f, transform.position.y - 1f);
        bool onAir = (isGroundLeft || isGroundRight);
        isGroundLeft = Physics2D.Raycast(rayStartLeft, Vector2.down, groundDistance, ground);
        isGroundRight = Physics2D.Raycast(rayStartRight, Vector2.down, groundDistance, ground);

        if ((isGroundLeft || isGroundRight) && !onAir) // 땅에 닿으면 점프 카운트 초기화
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