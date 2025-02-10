using UnityEngine;

public class EntityMoveBySec : MonoBehaviour
{
    // public 변수
    public float changeTime = 2.0f;
    public float moveSpeed = 2.0f;

    // private 변수
    private float moveTimer;
    private int moveDir = 1;
    Rigidbody2D rigidBody;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        moveTimer = changeTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Timer가 0 미만이 될 경우 moveDir 역전
        moveTimer -= Time.deltaTime;
        if(moveTimer < 0)
        {
            moveDir = -moveDir;
            moveTimer = changeTime;
        }
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, moveSpeed * moveDir);
    }
}
