using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 클래스로 Ctrl 클래스, 모듈 클래스로 이동, 공격, 카메라 등으로 생각 중
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
