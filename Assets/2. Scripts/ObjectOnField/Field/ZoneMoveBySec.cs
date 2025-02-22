using UnityEngine;

public class ZoneMoveBySec : MonoBehaviour
{
    // public 변수
    public static float CHANGE_TIME = 2.0f;
    public float moveSpeed = 2.0f;
    public bool vertical;

    // private 변수
    private float moveTimer;
    private int moveDir = 1;
    private Vector2 pos;
    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveTimer = CHANGE_TIME;
        pos = rb.position;
    }

    void Update()
    {
        // Timer가 0 미만이 될 경우 moveDir 역전
        moveTimer -= Time.deltaTime;
        if(moveTimer < 0)
        {
            moveDir = -moveDir;
            moveTimer = CHANGE_TIME;
        }

        // vertical == true면 상하 아니면 좌우
        if(vertical)
        {
            pos.y = pos.y + (moveSpeed * moveDir * Time.deltaTime);
        }
        else
        {
            pos.x = pos.x + (moveSpeed * moveDir * Time.deltaTime);
        }
        rb.MovePosition(pos);
    }
}
