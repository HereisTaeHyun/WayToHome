using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 7f;
    public LayerMask ground;
    public float groundDistance = 0.3f;
    public bool isGround;
    Rigidbody2D rigidBody;
    public int maxJump = 1;
    public int jumpCount = 0;
    public bool onAir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 수평 입력 값을 받아 방향 벡터 변환 후 Translate로 이동 입력
        float h = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * h * moveSpeed * Time.deltaTime);
        
        // 점프 가능 유무 방지를 발에서 시작한 ray가 Ground에 닿는지로 체크
        GroundCheck();

        if(Input.GetButtonDown("Jump") && jumpCount < maxJump) // ray가 Ground에 닿으면 점프
        {
            jumpCount += 1;
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpSpeed);
            onAir = true;
        }
    }
    void GroundCheck()
    {
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y - 1f);
        bool wasGrounded = isGround; // 이전 상태 저장
        isGround = Physics2D.Raycast(rayStart, Vector2.down, groundDistance, ground);

        if (isGround && !wasGrounded) // 땅에 처음 닿았을 때만 점프 카운트 초기화
        {
            jumpCount = 0;
            onAir = false; // 점프 상태 해제
        }

        Debug.DrawRay(rayStart, Vector2.down * groundDistance, Color.red);
    }
}