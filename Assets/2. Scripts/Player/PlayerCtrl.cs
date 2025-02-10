using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 객체로 Ctrl 객체 사용, 하위 모듈 객체로 이동, 공격, 카메라 등으로 생각 중
    PlayerMove playerMove;

    // 퍼블릭 변수
    public float currentHP = 10.0f;
    public float MaxHP = 10.0f;
    public float attack = 1.0f;
    public int money = 0;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    void Update() // Jump();는 FixedUpdate()에 배정시 즉각 반응하지 않아 Update()에 배치
    {
        playerMove.Jump();
    }

    void FixedUpdate()
    {
        playerMove.HorizontalMove();
    }
}
