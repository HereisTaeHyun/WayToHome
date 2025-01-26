using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 7f;
    public LayerMask ground;
    public float groundDistance = 0.1f;
    private bool isGround;
    Rigidbody2D rigidBody;
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
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y - 1);
        isGround = Physics2D.Raycast(rayStart, Vector2.down, groundDistance, ground);
        if(Input.GetButtonDown("Jump") && isGround) // ray가 Ground에 닿으면 점프
        {
            rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    // MainCamera가 Player의 중심을 추적하도록
    public Transform followingCamera;
    void LateUpdate()
    {
        followingCamera.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
