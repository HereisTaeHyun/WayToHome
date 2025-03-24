using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerCtrl : MonoBehaviour
{
    // 총괄 객체로 Ctrl 객체 사용, 하위 모듈 객체로 이동, 공격, 카메라 등으로 생각 중
    // 테스트장에서는 gravity scale을 1 썼는대 너무 가벼움, 실제 게임 필드 설치는 gravity scale = 2를 바탕으로 세팅 및 수정할 것

    // public 변수
#region public
    public float MaxHP;
    public float currentHP;
    public int money;
    public int maxJump;
    public float damage;
    [NonSerialized] public bool canMove;
    [NonSerialized] public State state;
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
    private GameObject spawnPos;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    private Rigidbody2D rb2D;
    private Animator playerAnim;
    private bool isDie;
    private CapsuleCollider2D coll2D;
    private PhysicsMaterial2D physicsMaterial2D;
    private readonly int dieHash = Animator.StringToHash("Die");
    [SerializeField] private GameObject graveStone;

    // UI 관련
    private Image HPBar;
    private GameObject statUI;
    
    // 무적 관련
    private bool invincible;
    public bool readInvincible {get {return invincible;}} // 적 관련 객체에서 가끔 참고
    private float invincibleTime = 2.0f;
    private float invincibleTimer;
    
    // 디버프 관련(스턴, 슬로우 생각 중)
    private float debuffTimer;

    protected readonly int takeHitHash = Animator.StringToHash("TakeHit");
#endregion

    // 싱글톤 선언
    public static PlayerCtrl player = null;
    void Awake()
    {
        if(player == null)
        {
            player = this;
        }
        else if(player != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

            // spawnPos = GameObject.FindGameObjectWithTag("SpawnPos");
            // player = Instantiate(playerPrefab, spawnPos.transform.position, playerPrefab.transform.rotation);
    // 초기화
    public void Init()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        coll2D = GetComponent<CapsuleCollider2D>();
        physicsMaterial2D = new PhysicsMaterial2D();

        // UI 관련
        HPBar = GameObject.FindGameObjectWithTag("HPBar")?.GetComponent<Image>();
        statUI = GameObject.FindGameObjectWithTag("StatUI");
        statUI.SetActive(false);

        // 초기화
        StartCoroutine(ApplyState());
        MaxHP = GameManager.instance.baseMaxHP;
        currentHP = MaxHP;
        money = GameManager.instance.baseMoney;
        canMove = true;
        state = State.Idle;
        // 모듈 초기화
        playerMove.Init();
        playerAttack.Init();

        // HP바 초기화
        DisplayHP();
    }

    // 이벤트 등록 부분
    void OnEnable()
    {
        GameManager.OnGameOver += PlayerDie;
    }
    void OnDisable()
    {
        GameManager.OnGameOver -= PlayerDie;
    }

    // 각 상태에 따라 필요한 변화 적용하는 곳
    private IEnumerator ApplyState()
    {
        while(isDie != true)
        {
            yield return new WaitForSeconds(0.3f);

            // 정지 상태이면 마찰력 증가, 그래야 멈출때랑 경사에서 안미끄러짐
            if(state == State.Move)
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
        if(canMove == false || GameManager.instance.readIsGameOver == true)
        {
            return;
        }

        // 모듈 클래스 함수 호출
        // 이동 외 움직임 동작
        playerMove.Jump();
        playerAttack.Attack();
        
        // 플레이어 StatUI 관련 동작
        DisplayStat();
    }

    private void FixedUpdate()
    {
        // 이동 관련 모듈 함수는 여기서 처리
        if(canMove == false || GameManager.instance.readIsGameOver == true)
        {
            return;
        }
        playerMove.HorizontalMove();

        
        // 플롯폼은 내려가기 키를 누르면 내려갈 수 있도록하기
        playerMove.GoDownPlatfom();
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

        // 체력 계산 및 체력바 표기
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);
        DisplayHP();

        // 데미지가 0이거나 그 이하일 경우 사망
        if(currentHP <= 0)
        {
            GameOver();
        }
    }

    private void DisplayStat()
    {
        // StatUI == Q로 생각, UI가 있으면 끄고 없으면 키기
        if(Input.GetButtonDown("StatUI") && statUI.activeSelf == false)
        {
            TextMeshProUGUI HPText = statUI.transform.Find("HP").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI MoneyText = statUI.transform.Find("Money").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI PowerText = statUI.transform.Find("Damage").GetComponent<TextMeshProUGUI>();
            HPText.text = $"HP : {currentHP} / {MaxHP}";
            MoneyText.text = $"Money : {money}";
            PowerText.text = $"Damage : {-playerAttack.attackDamage}";
            statUI.SetActive(true);
        }
        else if(Input.GetButtonDown("StatUI") && statUI.activeSelf == true)
        {
            statUI.SetActive(false);
        }
    }

    private void GameOver()
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        canMove = false;
        isDie = true;
        state = State.Die;
        GameManager.instance.GameOverTrigger();
    }

    // 타 객체에서 최대 체력 증가시킬떄 접근
    public void ChangeMaxHP()
    {
        MaxHP += 1;
        DisplayHP();
    }

    // HP 패녈 표시
    private void DisplayHP()
    {
        HPBar.fillAmount = currentHP / MaxHP;
    }

    // 플레이어 사망
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
        StopAllCoroutines();
        gameObject.SetActive(false);
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
