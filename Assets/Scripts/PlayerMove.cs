using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
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
        float h = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(h, 0);
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y - 1);
        isGround = Physics2D.Raycast(rayStart, Vector2.down, groundDistance, ground);
        if(Input.GetButtonDown("Jump") && isGround)
        {
            rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    public Transform followingCamera;
    void LateUpdate()
    {
        followingCamera.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
