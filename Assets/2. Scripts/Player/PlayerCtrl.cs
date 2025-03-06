using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 객체로 Ctrl 객체 사용, 하위 모듈 객체로 이동, 공격, 카메라 등으로 생각 중
    // 테스트장에서는 gravity scale을 1 썼는대 너무 가벼움, 실제 게임 필드 설치는 gravity scale = 2를 바탕으로 세팅 및 수정할 것

    // public 변수
    public float MaxHP = 10.0f;
    public float currentHP;
    public int money = 0;
    public State state;
    public enum State
    {
        Idle,
        Move,
        Die,
    }
    public enum DebuffType
    {
        Stun,
        Slow,
    }

    // private 변수
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    public bool canMove;
    [SerializeField] private GameObject graveStone;
    
    // 무적 관련
    private bool invincible;
    public bool readInvincible {get {return invincible;}} // 적 관련 객체에서 가끔 참고
    private float invincibleTime = 2.0f;
    private float invincibleTimer;

    // 디버프 관련(스턴, 슬로우 생각 중)
    private float debuffTimer;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        currentHP = MaxHP;
        canMove = true;
        state = State.Idle;
    }

    void Update() // Jump();는 FixedUpdate()에 배정시 즉각 반응하지 않아 Update()에 배치
    {
        // 타이머가 위에 움직임 제어 권한 아래에, 안그러면 디버프나 무적 안풀릴떄 생김
        // 무적시간일 경우 무적 타이머 초마다 차감하여 통상상태로 되돌림
        if(invincible == true)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer <= 0)
            {
                invincible = false;
            }
        }

         // (debuffTimer > 0) == getDebuff를 당함, 이 경우도 타이머 차감하여 통상 상태로
        if(debuffTimer > 0)
        {
            debuffTimer -= Time.deltaTime;
            if(debuffTimer <= 0)
            {
                canMove = true;
                playerMove.moveSpeed = playerMove.readOriginSpeed;
            }
        }

        // canMove == true일때만 playerMove 객체 접근 가능
        if(canMove == false)
        {
            return;
        }

        // 모듈 클래스 함수 호출
        playerMove.Jump();
        playerAttack.MeleeAttack();
    }

    private void FixedUpdate()
    {
        // 이동 관련 모듈 함수는 여기서 처리
        if(canMove == false)
        {
            return;
        }
        // playerMove.SlopeCheck();
        playerMove.HorizontalMove();
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

    // 플레이어 사망, 현재는 임시로 Destroy만 사용 중, 이후 anim, audio 등 추가 예정
    private void PlayerDie()
    {
        Instantiate(graveStone, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    // 적 객체에서 디버프 타입, 시간 받아서 적용
    public void GetDebuff(DebuffType debuffType, float debuffTime)
    {
        debuffTimer = debuffTime;
        switch(debuffType)
        {
            case DebuffType.Stun:
                canMove = false;
                break;

            case DebuffType.Slow:
                playerMove.moveSpeed = playerMove.readDebuffedSpeed;
                break;
        }
    }

    public Vector2 MoveDirSet(Vector2 move)
    {
        Vector2 moveDir = new Vector2(0, 0);
        if(Mathf.Approximately(move.x, 0) == false)
        {
            moveDir.Set(move.x, 0);
            moveDir.Normalize();
        }
        return moveDir;
    }
}
