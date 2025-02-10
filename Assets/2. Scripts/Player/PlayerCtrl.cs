using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 객체로 Ctrl 객체 사용, 하위 모듈 객체로 이동, 공격, 카메라 등으로 생각 중
    // 테스트장에서는 gravity scale을 1 썼는대 너무 가벼움, 실제 게임 필드 설치는 gravity scale = 2를 바탕으로 세팅 및 수정할 것
    PlayerMove playerMove;

    // 퍼블릭 변수
    public float MaxHP = 10.0f;
    public float currentHP;
    public float attack = 1.0f;
    public int money = 0;
    public float invincibleTime = 2.0f;

    // private 변수
    private bool invincible;
    private float invincibleTimer;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        currentHP = MaxHP;
    }

    void Update() // Jump();는 FixedUpdate()에 배정시 즉각 반응하지 않아 Update()에 배치
    {
        playerMove.Jump();

        // 무적시간일 경우 무적 타이머 초마다 차감하여 통상상태로 되돌림
        if(invincible == true)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0)
            {
                invincible = false;
            }
        }
    }

    void FixedUpdate()
    {
        playerMove.HorizontalMove();
    }

    // 플레이어 사망, 현재는 임시로 Destroy만 사용 중, 이후 anim 추가 예정
    private void PlayerDie()
    {
        Destroy(gameObject);
    }

    // 플레이어 데미지 가해
    public void ChangeHP(int value)
    {
        // 데미지일 경우 무적시간으로 설정
        if(value < 0)
        {
            if(invincible == true)
            {
                return;
            }
            invincible = true;
            invincibleTimer = invincibleTime;
        }
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);

        // 데미지가 0이거나 그 이하일 경우 사망
        if(currentHP <= 0)
        {
            PlayerDie();
        }
    }
}
