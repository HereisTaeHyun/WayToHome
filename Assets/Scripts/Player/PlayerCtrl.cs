using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 컴포넌트트
    Rigidbody2D rigidBody;
    PlayerMove playerMove;

    // 퍼블릭 변수
    public int currentHP = 10;
    public int MaxHP = 10;
    public int money = 0;
    public float attack = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMove.MoveCtrl();
    }
}
