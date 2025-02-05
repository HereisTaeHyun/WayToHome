using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    PlayerMove playerMove;

    // 퍼블릭 변수
    public int currentHP = 10;
    public int MaxHP = 10;
    public int money = 0;
    public float attack = 1.0f;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        playerMove.MoveCtrl();
    }
}
