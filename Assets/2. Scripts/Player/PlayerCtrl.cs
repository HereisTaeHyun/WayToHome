using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class PlayerCtrl : MonoBehaviour
{
    // public 변수
    #region public
    public Vector2 moveInput { get; private set; }
    public Vector2 lastMoveDir { get; private set; }
    public Vector2 aimPos { get; private set; }
    public bool isMagic { get; private set; }
    public bool isMenu { get; private set; }
    [NonSerialized] public bool isSubmit;

    public event Action<int> SelectMagic;
    public event Action<bool> ToggleAttackModeEvent;

    public float maxHP;
    public float currentHP;
    public float maxMana;
    public float currentMana;
    public int money;
    [NonSerialized] public SpriteRenderer spriteRenderer;
    [NonSerialized] public bool canMove;
    [NonSerialized] public State state;
    [NonSerialized] public bool canAttack;
    [NonSerialized] public bool canDash;
    [NonSerialized] public bool onDash;
    [NonSerialized] public Animator playerAnim;
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
    public PlayerMove playerMove { get; private set; }
    public PlayerAttack playerAttack { get; private set; }
    private PlayerInput inputActions;
    private Rigidbody2D rb2D;
    private bool isDie;
    private CapsuleCollider2D coll2D;
    private PhysicsMaterial2D physicsMaterial2D;
    private Camera mainCam;
    private Vector2 mouseScreenPos;
    private readonly int dieHash = Animator.StringToHash("Die");
    [SerializeField] private GameObject graveStone;

    // UI 관련
    private Image hpBar;
    private Image manaBar;
    private TextMeshProUGUI hpText;
    private TextMeshProUGUI manaText;
    private TextMeshProUGUI moneyText;
    private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI statText;

    // 무적 관련
    private bool damageInvincible;
    private bool dashInvincible;
    private bool isInvincible { get { return damageInvincible || dashInvincible; } }
    public bool readIsInvincible { get { return isInvincible; } } // 적 관련 객체에서 가끔 참고
    private static float DAMAGE_INVINCIBLE_TIMER = 2.0f;
    private float damageInvincibleTimer;
    private static float BLINK_TIME = 0.1f;
    private float dashInvincibleTime = 0.25f;

    // 오디오 클립
    [SerializeField] private AudioClip takeHitSFX;
    [SerializeField] private AudioClip dieSFX;
    [SerializeField] private AudioClip maxHPPlusSFX;
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private AudioClip attackPlusSFX;
    [SerializeField] private AudioClip jumpPlusSFX;
    [SerializeField] private AudioClip moneySFX;
    [SerializeField] private AudioClip toggleAttackModeSFX;

    // 디버프 관련(스턴, 슬로우 생각 중)
    private float debuffTimer;

    protected readonly int takeHitHash = Animator.StringToHash("TakeHit");
    #endregion

    #region Init
    // 싱글톤 선언
    public static PlayerCtrl player = null;
    void Awake()
    {
        if (player == null)
        {
            player = this;
            inputActions = new PlayerInput();
        }
        else if (player != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 초기화
    public void Init()
    {
        StopAllCoroutines();

        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        rb2D = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        coll2D = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        physicsMaterial2D = new PhysicsMaterial2D();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // UI 관련
        hpBar = UIManager.uIManager.HpBar;
        manaBar = UIManager.uIManager.ManaBar;

        hpText = UIManager.uIManager.HpText;
        manaText = UIManager.uIManager.ManaText;
        moneyText = UIManager.uIManager.MoneyText;
        damageText = UIManager.uIManager.DamageText;

        // 상태 체커 시작 및 상태 변수 초기화
        StartCoroutine(ApplyState());
        isDie = false;
        isMenu = false;
        canMove = true;
        canAttack = true;
        canDash = true;
        state = State.Idle;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        lastMoveDir = Vector2.right;

        // 모듈 초기화
        playerMove.Init();
        playerAttack.Init();

        UIManager.uIManager.RefreshPlayerUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
    }

    // 이벤트 등록 부분
    void OnEnable()
    {
        GameManager.OnGameOver += PlayerDie;
        SceneManager.sceneLoaded += OnSceneLoaded;

        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += CancelMove;

        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Dash.performed += OnDash;

        inputActions.Player.Submit.performed += ctx => isSubmit = true;
        inputActions.Player.Submit.canceled += ctx => isSubmit = false;

        inputActions.Player.Menu.performed += ToggleMenuUI;

        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.EnableMagic.performed += ToggleAttackMode;
        inputActions.Player.AimPos.performed += OnAim;
        inputActions.Player.SelectMagic1.performed += ctx => SelectMagic(0);
        inputActions.Player.SelectMagic2.performed += ctx => SelectMagic(1);
    }
    void OnDisable()
    {
        GameManager.OnGameOver -= PlayerDie;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        inputActions.Disable();

        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= CancelMove;

        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Dash.performed -= OnDash;

        inputActions.Player.Submit.performed -= ctx => isSubmit = true;
        inputActions.Player.Submit.canceled -= ctx => isSubmit = false;

        inputActions.Player.Menu.performed -= ToggleMenuUI;

        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.EnableMagic.performed -= ToggleAttackMode;
        inputActions.Player.AimPos.performed -= OnAim;
        inputActions.Player.SelectMagic1.performed -= ctx => SelectMagic(0);
        inputActions.Player.SelectMagic2.performed -= ctx => SelectMagic(1);
    }

    // 각 상태에 따라 필요한 변화 적용하는 곳
    private IEnumerator ApplyState()
    {
        while (isDie != true)
        {
            yield return new WaitForSeconds(0.3f);

            // 정지 상태이면 마찰력 증가, 그래야 멈출때랑 경사에서 안미끄러짐
            if (state == State.Move)
            {
                physicsMaterial2D.friction = 1.8f;
                coll2D.sharedMaterial = physicsMaterial2D;
            }
            else if (state == State.Idle)
            {
                physicsMaterial2D.friction = 20.0f;
                coll2D.sharedMaterial = physicsMaterial2D;
            }
        }
    }

    public IEnumerator InvokeSelectMagic(int idx)
    {
        yield return null;
        SelectMagic?.Invoke(idx);
    }
    #endregion

    #region InputSyetem
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.x != 0)
        {
            lastMoveDir = UtilityManager.utility.HorizontalDirSet(moveInput);
        }
    }

    private void CancelMove(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (playerMove.jumpCount < playerMove.maxJump)
        {
            playerMove.Jump();
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (canMove == true)
        {
            StartCoroutine(playerMove.Dash());
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack == true)
        {
            playerAttack.Attack();
        }
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        mouseScreenPos = context.ReadValue<Vector2>();
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        aimPos = ((Vector2)worldPos - (Vector2)transform.position);
    }

    private void ToggleAttackMode(InputAction.CallbackContext context)
    {
        isMagic = !isMagic;
        UtilityManager.utility.PlaySFX(toggleAttackModeSFX);
        ToggleAttackModeEvent?.Invoke(isMagic);
        SelectMagic?.Invoke(playerAttack.selectedMagicIdx);
    }

    private void ToggleMenuUI(InputAction.CallbackContext context)
    {
        isMenu = !isMenu;
        if (isMenu)
        {
            UIManager.uIManager.OpenMenuUI();
        }
        else if (!isMenu)
        {
            UIManager.uIManager.CloseMenuUI();
        }
    }
    #endregion

    #region Unity Method
    void Start()
    {
        DisplayHP();
        DisplayMana();
        DisplayMoney();
        DisplayDamage();
    }

    // 즉각 반응해야 하는 모듈들은 Update()에 배치
    void Update()
    {
        // 타이머는 움직임 제어 권한 위에, 안그러면 디버프나 무적 안풀릴떄 생김
        // 무적시간일 경우 무적 타이머 초마다 차감하여 통상상태로 되돌림
        if (damageInvincible == true)
        {
            damageInvincibleTimer -= Time.deltaTime;
            if (damageInvincibleTimer <= 0)
            {
                damageInvincible = false;;
            }
        }
        // (debuffTimer > 0) == getDebuff를 당함, 이 경우도 타이머 차감하여 통상 상태로
        if (debuffTimer > 0)
        {
            debuffTimer -= Time.deltaTime;
            if (debuffTimer <= 0)
            {
                canMove = true;
                playerMove.moveSpeed = playerMove.readOriginSpeed;
            }
        }

        // canMove == true거나 isGameOver == false playerMove 객체 접근
        if (canMove == false || GameManager.instance.readIsGameOver == true)
        {
            return;
        }

        // 이동 관련 모듈 함수
        playerMove.HorizontalMove();
        playerMove.GoDownPlatfom();
    }
    #endregion

    #region HP & GameOver
    // 플레이어 체력 수정
    public void ChangeHP(float value)
    {
        // 데미지일 경우 체크 사항
        if (value < 0)
        {
            // 이미 무적 시간이면 다음 단계 진입하지 않음
            if (isInvincible == true)
            {
                return;
            }
            // 무적 시간이 아니었으면 사운드 재생 및 무적으로 만든 후 Timer 설정
            UtilityManager.utility.PlaySFX(takeHitSFX);
            damageInvincible = true;
            damageInvincibleTimer = DAMAGE_INVINCIBLE_TIMER;
            // 데미지 입으면 무적 시간 동안 깜빡임
            StartCoroutine(BlinkUntilInvincible());
        }
        else if (value > 0)  // value가 plus면 힐이니까 힐 사운드 재생
        {
            UtilityManager.utility.PlaySFX(healSFX);
        }
        // 체력 계산 및 체력바 표기
        currentHP = Mathf.Clamp(currentHP + value, 0, maxHP);
        DisplayHP();

        // 데미지가 0이거나 그 이하일 경우 사망
        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    // 데미지 입고 무적 시간 동안 깜빡거리기 코루틴
    IEnumerator BlinkUntilInvincible()
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;
        // 무적이고 == 데미지를 입었고, 사망이 아니라면 깜빡임 시작
        while (damageInvincible == true && isDie == false)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if (isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if (isBlink == false)
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

    public IEnumerator DashInvincible()
    {
        dashInvincible = true;
        yield return new WaitForSeconds(dashInvincibleTime);
        dashInvincible = false;
    }
    #endregion

    #region UI
    public void DisplayHP()
    {
        hpBar.fillAmount = currentHP / maxHP;
        hpText.text = $"{currentHP} / {maxHP}";
    }

    public void DisplayMana()
    {
        manaBar.fillAmount = currentMana / maxMana;
        manaText.text = $"{currentMana} / {maxMana}";
    }
    public void DisplayMoney()
    {
        moneyText.text = $": {money}";
    }
    public void DisplayDamage()
    {
        damageText.text = $": {-playerAttack.attackDamage}";
    }

    #endregion

    // 게임오버, GameManager 이벤트 호출 역할
    private void GameOver()
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        canMove = false;
        isDie = true;
        state = State.Die;
        GameManager.instance.GameOverTrigger();
    }

    // 플레이어 사망, GamaManager OnGameOver 이벤트에 등록하여 사용
    private void PlayerDie()
    {
        StartCoroutine(DieStart());
    }
    private IEnumerator DieStart()
    {
        // 사망 애니메이션 재생 후 묘비 배치 및 set false
        state = State.Die;
        playerAnim.SetTrigger(dieHash);

        yield return new WaitForSeconds(1.5f);

        UtilityManager.utility.PlaySFX(dieSFX);
        Instantiate(graveStone, transform.position, transform.rotation);
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    // 적 객체에서 디버프 타입, 시간 받아서 적용
    public void GetDebuff(DebuffType debuffType, float debuffTime)
    {
        debuffTimer = debuffTime;
        switch (debuffType)
        {
            case DebuffType.Stun:
                canMove = false;
                break;

            case DebuffType.Slow:
                playerMove.moveSpeed = playerMove.readDebuffedSpeed;
                break;
        }
    }

    #region StatChange
    // Stat 수정 관련

    private void ShowStatChange(string message)
    {
        statText.text = message;
        StopCoroutine(nameof(ClearStatText));
        StartCoroutine(ClearStatText());
    }

    private IEnumerator ClearStatText()
    {
        yield return new WaitForSeconds(2.0f);
        statText.text = "";
    }
    public void MaxHpPlus()
    {
        maxHP += 10;
        currentHP += 10;

        ShowStatChange("MaxHP +");
        DisplayHP();
        UtilityManager.utility.PlaySFX(maxHPPlusSFX);
    }
    public void MaxManaPlus()
    {
        maxMana += 10;
        currentMana += 10;

        ShowStatChange("MaxMana +");
        DisplayMana();
        UtilityManager.utility.PlaySFX(maxHPPlusSFX);
    }
    public void GetMoney(int plusMoney)
    {
        money += plusMoney;
        DisplayMoney();
        UtilityManager.utility.PlaySFX(moneySFX);
    }
    public void DamagePlus()
    {
        playerAttack.attackDamage -= 5;

        ShowStatChange("Damage +");
        DisplayDamage();
        UtilityManager.utility.PlaySFX(attackPlusSFX);
    }

    public void MaxJumpPlus()
    {
        playerMove.maxJump += 1;

        ShowStatChange("MaxJump +");
        UtilityManager.utility.PlaySFX(jumpPlusSFX);
    }
    #endregion
}
