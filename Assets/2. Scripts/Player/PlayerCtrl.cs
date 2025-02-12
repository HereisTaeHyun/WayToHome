using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 객체로 Ctrl 객체 사용, 하위 모듈 객체로 이동, 공격, 카메라 등으로 생각 중
    // 테스트장에서는 gravity scale을 1 썼는대 너무 가벼움, 실제 게임 필드 설치는 gravity scale = 2를 바탕으로 세팅 및 수정할 것

    // canMove로 조작 가능 불가능 체크 필요 ex: 폭파에 당하면 canMove == false 후 되돌리기 방식
    // 폭파에 당한 후 invincible 끝날때 canMove 다시 풀기 고민 중
    // 데미지 타입을 구분해야 할까? ChangeHP에서 어떤 공격인지에 따라 canMove true or false? 이 부분은 고민
    // public 변수
    public float MaxHP = 10.0f;
    public float currentHP;
    public float attack = 1.0f;
    public int money = 0;
    public enum DebuffType
    {
        Stun,
        Slow,
    }

    // private 변수
    PlayerMove playerMove;
    public bool canMove; // 디버깅 후 버그 없으면 private로
    // 무적 관련
    private bool invincible;
    private float invincibleTime = 2.0f;
    private float invincibleTimer;

    // 디버프 관련(스턴, 슬로우 생각 중)
    public float debuffTimer;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        currentHP = MaxHP;
        canMove = true;
    }

    void Update() // Jump();는 FixedUpdate()에 배정시 즉각 반응하지 않아 Update()에 배치
    {
        playerMove.Jump();

        // 무적시간일 경우 무적 타이머 초마다 차감하여 통상상태로 되돌림
        if(invincible == true)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer <= 0)
            {
                invincible = false;
            }
        }

        if(debuffTimer > 0) // (debuffTimer > 0) == getDebuff를 당함
        {
            debuffTimer -= Time.deltaTime;
            if(debuffTimer <= 0)
            {
                canMove = true;
                playerMove.moveSpeed = playerMove.originSpeed;
            }
        }
    }

    void FixedUpdate()
    {
        if(!canMove) return;
        playerMove.HorizontalMove();
    }

    // 플레이어 사망, 현재는 임시로 Destroy만 사용 중, 이후 anim, audio 추가 예정
    private void PlayerDie()
    {
        Destroy(gameObject);
    }

    // 플레이어 데미지 가해
    public void ChangeHP(float value)
    {
        // 데미지일 경우 체크 사항
        if(value < 0)
        {
            // 이미 무적 시간이면 다음 단계 진입하지 않음
            if(invincible == true)
            {
                return;
            }
            // 무적 시간이 아니었으면 무적으로 만든 후 Timer 설정
            invincible = true;
            invincibleTimer = invincibleTime;
        }
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);
        Debug.Log($"체력변화 : {value}");

        // 데미지가 0이거나 그 이하일 경우 사망
        if(currentHP <= 0)
        {
            PlayerDie();
        }
    }

    public void GetDebuff(DebuffType debuffType, float debuffTime)
    {
        debuffTimer = debuffTime;
        switch(debuffType)
        {
            case DebuffType.Stun:
                canMove = false;
                break;

            case DebuffType.Slow:
                playerMove.moveSpeed = playerMove.debuffedSpeed;
                break;
        }
    }
}
