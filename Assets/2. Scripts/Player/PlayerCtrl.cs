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
    [NonSerialized] public bool canMove;
    [NonSerialized] public State state;
    [NonSerialized] public bool canAttack;
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
    private SpriteRenderer spriteRenderer;
    private bool isDie;
    private CapsuleCollider2D coll2D;
    private PhysicsMaterial2D physicsMaterial2D;
    private readonly int dieHash = Animator.StringToHash("Die");
    [SerializeField] private GameObject graveStone;

    // UI 관련
    private Image HPBar;
    private GameObject statUI;
    private TextMeshProUGUI itemText;
    private static float DISPLAY_ITEM_EFFECT_TIME = 1.0f;
    
    // 무적 관련
    private bool invincible;
    public bool readInvincible {get {return invincible;}} // 적 관련 객체에서 가끔 참고
    private static float INVINCIBLE_TIME = 2.0f;
    private float invincibleTimer;
    private static float BLINK_TIME = 0.1f;

    // 오디오 클립
    [SerializeField] private AudioClip takeHitSFX;
    [SerializeField] private AudioClip dieSFX;
    [SerializeField] private AudioClip maxHPPlusSFX;
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private AudioClip attackPlusSFX;
    [SerializeField] private AudioClip jumpPlusSFX;
    [SerializeField] private AudioClip moneySFX;
    
    // 디버프 관련(스턴, 슬로우 생각 중)
    private float debuffTimer;

    protected readonly int takeHitHash = Animator.StringToHash("TakeHit");
#endregion

#region 초기화
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

    // 초기화
    public void Init()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        coll2D = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        physicsMaterial2D = new PhysicsMaterial2D();

        // UI 관련
        HPBar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Image>();
        itemText = transform.Find("TextCanvas/ItemText").GetComponent<TextMeshProUGUI>();
        statUI = transform.Find("PlayerUI/GamePlayUI/StatUI").gameObject;

        itemText.text = "";
        statUI.SetActive(false);

        // 상태 체커 시작 및 상태 변수 초기화
        StartCoroutine(ApplyState());
        canMove = true;
        canAttack = true;
        state = State.Idle;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 모듈 초기화
        playerMove.Init();
        playerAttack.Init();

        // HPBar 초기화, 다시 되살아날 때 필요
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
                physicsMaterial2D.friction = 20.0f;
                coll2D.sharedMaterial = physicsMaterial2D;
            }
        }
    }
#endregion

#region Unity 제공 메서드
    void Start()
    {
        // HP바 초기화, 스탯 변수들 다 임포트 되고 초기화해야 해서 얘는 Start에서 한 번 호츌 필요
        DisplayHP();
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
        // 상점에서는 공격 막기위해 canAttack 플래그 존재
        playerMove.Jump();
        if(canAttack == true)
        {
            playerAttack.Attack();
        }
        
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
#endregion
 
#region 체력, 게임 오버 관련
    // 플레이어 체력 수정
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
            // 무적 시간이 아니었으면 사운드 재생 및 무적으로 만든 후 Timer 설정
            UtilityManager.utility.PlaySFX(takeHitSFX);
            invincible = true;
            invincibleTimer = INVINCIBLE_TIME;
            // 데미지 입으면 무적 시간 동안 깜빡임
            StartCoroutine(BlinkUntilInvincible());
        }
        else if(value > 0)  // value가 plus면 힐이니까 힐 사운드 재생
        {
            UtilityManager.utility.PlaySFX(healSFX);
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

    // 무적 시간 동안 깜빡거리기 코루틴
    IEnumerator BlinkUntilInvincible()
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;
        // 무적이고 == 데미지를 입었고, 사망이 아니라면 깜빡임 시작
        while(invincible == true && isDie == false)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if(isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if(isBlink == false)
            {
                color.a = 1.0f;
                spriteRenderer.color = color;
                isBlink = true;
            }
            yield return new WaitForSeconds(BLINK_TIME);
        }
        // 기본 상태로 초기화
        color.a = 1.0f;
        spriteRenderer.color = color;
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

    // 게임오버, GameManager 이벤트 호출 역할
    private void GameOver()
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        canMove = false;
        isDie = true;
        state = State.Die;
        GameManager.instance.GameOverTrigger();
    }

    // HP 패녈 표시
    private void DisplayHP()
    {
        HPBar.fillAmount = currentHP / MaxHP;
    }

    // 플레이어 사망, GamaManager OnGameOver 이벤트에 등록하여 사용
    private void PlayerDie()
    {
        StartCoroutine(DieStart());
    }
    private IEnumerator DieStart()
    {
        // 사망 과정은 점진적, 사망 애니메이션 재생 후 묘비 배치 및 끄기
        state = State.Die;
        playerAnim.SetTrigger(dieHash);
        yield return new WaitForSeconds(1.5f);
        UtilityManager.utility.PlaySFX(dieSFX);
        Instantiate(graveStone, transform.position, transform.rotation);
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
#endregion

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

#region Stat
    // Stat 수정 관련
    
    public void MaxHpPlus()
    {
        MaxHP += 1;
        StartCoroutine(DisplayItemEffect("MaxHP+ !"));
        UtilityManager.utility.PlaySFX(maxHPPlusSFX);
        DisplayHP();
        Debug.Log("최대 체력 증가");
    }
    public void GetMoney(int plusMoney)
    {
        money += plusMoney;
        UtilityManager.utility.PlaySFX(moneySFX);
        Debug.Log($"{plusMoney} 골드 획득");
    }
    public void Attacklus()
    {
        playerAttack.attackDamage -= 1;
        StartCoroutine(DisplayItemEffect("Attack+ !"));
        UtilityManager.utility.PlaySFX(attackPlusSFX);
        Debug.Log("공격력 증가");
    }

    public void MaxJumpPlus()
    {
        playerMove.maxJump += 1;
        StartCoroutine(DisplayItemEffect("Jump+ !"));
        UtilityManager.utility.PlaySFX(jumpPlusSFX);
        Debug.Log("최대 점프 증가");
    }

    IEnumerator DisplayItemEffect(string ItemEffectText)
    {
        itemText.text = ItemEffectText;
        yield return new WaitForSeconds(DISPLAY_ITEM_EFFECT_TIME);
        itemText.text = "";
    }
#endregion
}
