using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 객체로 Ctrl 객체 사용, 하위 모듈 객체로 이동, 공격, 카메라 등으로 생각 중
    // 테스트장에서는 gravity scale을 1 썼는대 너무 가벼움, 실제 게임 필드 설치는 gravity scale = 2를 바탕으로 세팅 및 수정할 것

    // public 변수
#region public
    public float MaxHP = 10.0f;
    public float currentHP;
    public int money = 0;
    public bool canMove;
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
#endregion

#region private
    // private 변수
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    private Rigidbody2D rb2D;
    private Animator playerAnim;
    private bool isDie;
    private CapsuleCollider2D coll2D;
    private PhysicsMaterial2D physicsMaterial2D;
    private readonly int dieHash = Animator.StringToHash("Die");
    [SerializeField] private GameObject graveStone;
    
    // 무적 관련
    private bool invincible;
    public bool readInvincible {get {return invincible;}} // 적 관련 객체에서 가끔 참고
    private float invincibleTime = 2.0f;
    private float invincibleTimer;
    
    // 디버프 관련(스턴, 슬로우 생각 중)
    private float debuffTimer;

    protected readonly int takeHitHash = Animator.StringToHash("TakeHit");
#endregion

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        coll2D = GetComponent<CapsuleCollider2D>();
        physicsMaterial2D = new PhysicsMaterial2D();

        StartCoroutine(ApplyState());
        currentHP = MaxHP;
        canMove = true;
        state = State.Idle;
    }

    // 각 상태에 따라 필요한 변화 적용하는 곳
    private IEnumerator ApplyState()
    {
        while(isDie != true)
        {
            yield return new WaitForSeconds(0.3f);

            if(state == State.Die)
            {
                canMove = false;
                rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                yield break;
            }
             // 정지 상태이면 마찰력 증가, 그래야 멈출때랑 경사에서 안미끄러짐
            else if(state == State.Move)
            {
                physicsMaterial2D.friction = 1.8f;
                coll2D.sharedMaterial = physicsMaterial2D;
            }
            else if(state == State.Idle)
            {
                physicsMaterial2D.friction = 10.0f;
                coll2D.sharedMaterial = physicsMaterial2D;
            }
        }
    }

    // 즉각 반응해야 하는 모듈들은 Update()에 배치
    void Update()
    {
        // 타이머는 움직임 제어 권한 위에, 안그러면 디버프나 무적 안풀릴떄 생김
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
        playerAttack.Attack();
    }

    private void FixedUpdate()
    {
        // 이동 관련 모듈 함수는 여기서 처리
        if(canMove == false)
        {
            return;
        }
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
            Debug.Log($"체력변화 : {value}");
        }
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);

        // 데미지가 0이거나 그 이하일 경우 사망
        if(currentHP <= 0)
        {
            state = State.Die;
            PlayerDie();
        }
    }

    // 플레이어 사망, 현재는 임시로 Destroy만 사용 중, 이후 anim, audio 등 추가 예정
    private void PlayerDie()
    {
        StartCoroutine(DieStart());
    }
    private IEnumerator DieStart()
    {
        state = State.Die;
        playerAnim.SetTrigger(dieHash);
        yield return new WaitForSeconds(1.5f);
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
}
