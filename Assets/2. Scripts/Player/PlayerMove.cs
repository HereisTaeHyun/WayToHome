using Mono.Cecil.Cil;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 움직임 관련 객체

    // public 변수
    // 아래 3개는 아이템에 의한 증감 가능하게 할 생각
    public float moveSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public int maxJump = 1;

    // private 변수
    [SerializeField] private LayerMask ground;
    Rigidbody2D rigidBody;
    private float jumpSpeed = 10.0f;
    private int jumpCount = 0;
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
        Jump();
    }

    // 좌우 이동 메서드
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

    // 점프에 필요한 메서드들
    // Ground 위면 JumpCount 초기화
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            jumpCount = 0;
        }   
    }
    private void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < maxJump) // W에 할당된 "Jump"를 눌러 maxJump까지 점프가능
        {
            jumpCount += 1;
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpSpeed);
        }
    }
}