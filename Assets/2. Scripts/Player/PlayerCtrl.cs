using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class PlayerCtrl : MonoBehaviour
{
    // public 변수
    #region public
    [NonSerialized] public PlayerInput inputActions;
    public Vector2 moveInput { get; private set; }
    public Vector2 lastMoveDir { get; private set; }
    public Vector2 aimPos { get; private set; }
    public bool isMagic { get; private set; }
    public bool isSubmit { get; private set; }

    public event Action<int> SelectMagic;

    public float maxHP;
    public float currentHP;
    public float maxMana;
    public float currentMana;
    public int money;
    [NonSerialized] public SpriteRenderer spriteRenderer;
    [NonSerialized] public bool canMove;
    [NonSerialized] public State state;
    [NonSerialized] public bool canAttack;
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
    private Rigidbody2D rb2D;
    private bool isDie;
    private CapsuleCollider2D coll2D;
    private PhysicsMaterial2D physicsMaterial2D;
    private Camera mainCam;
    private Vector2 mouseScreenPos;
    private readonly int dieHash = Animator.StringToHash("Die");
    [SerializeField] private GameObject graveStone;
    [SerializeField] private StatGemBar HPGemBar;
    [SerializeField] private StatGemBar moneyGemBar;
    [SerializeField] private StatGemBar damageGemBar;

    // UI 관련
    private Image HPBar;
    private Image manaBar;
    private GameObject fadeUI;
    private Image fadeImage;
    private GameObject statUI;
    private GameObject menuUI;
    private TextMeshProUGUI text;
    private static float DISPLAY_ITEM_EFFECT_TIME = 1.0f;
    private float fadeOutTime = 1.5f;
    private const float FADE_OUT_ALPHA = 1.0f;

    // 무적 관련
    private bool invincible;
    public bool readInvincible { get { return invincible; } } // 적 관련 객체에서 가끔 참고
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
    [SerializeField] private AudioClip toggleAttackModeSFX;

    // 디버프 관련(스턴, 슬로우 생각 중)
    private float debuffTimer;

    protected readonly int takeHitHash = Animator.StringToHash("TakeHit");
    #endregion

    #region 초기화
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
        HPBar = transform.Find("PlayerUI/GamePlayUI/HP/HPBar").GetComponent<Image>();
        manaBar = transform.Find("PlayerUI/GamePlayUI/Mana/ManaBar").GetComponent<Image>();
        text = transform.Find("TextCanvas/Text").GetComponent<TextMeshProUGUI>();
        fadeUI = transform.Find("PlayerUI/FadeUI").gameObject;
        fadeImage = fadeUI.GetComponent<Image>();
        statUI = transform.Find("PlayerUI/GamePlayUI/StatUI").gameObject;
        menuUI = transform.Find("PlayerUI/MenuUI").gameObject;

        Color newColor = fadeImage.color;
        newColor.a = 0.0f;
        fadeImage.color = newColor;
        text.text = "";
        statUI.SetActive(false);
        menuUI.SetActive(false);

        // 상태 체커 시작 및 상태 변수 초기화
        StartCoroutine(ApplyState());
        isDie = false;
        canMove = true;
        canAttack = true;
        state = State.Idle;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 모듈 초기화
        playerMove.Init();
        playerAttack.Init();

        // Bar, 다시 되살아날 때 필요
        DisplayHP();
        DisplayMana();
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

        inputActions.Player.Submit.performed += ctx => isSubmit = true;
        inputActions.Player.Submit.canceled += ctx => isSubmit = false;

        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.EnableMagic.performed += ToggleAttackMode;
        inputActions.Player.AimPos.performed += OnAim;
        inputActions.Player.SelectMagic1.performed += ctx => SelectMagic(0);
        inputActions.Player.SelectMagic2.performed += ctx => SelectMagic(1);

        inputActions.Player.DisplayStat.performed += OnDisPlayStat;
        inputActions.Player.DisplayMenu.performed += OnDisPlayMenu;
    }
    void OnDisable()
    {
        GameManager.OnGameOver -= PlayerDie;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        inputActions.Disable();

        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= CancelMove;

        inputActions.Player.Jump.performed -= OnJump;

        inputActions.Player.Submit.performed += ctx => isSubmit = true;
        inputActions.Player.Submit.canceled += ctx => isSubmit = false;

        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.EnableMagic.performed -= ToggleAttackMode;
        inputActions.Player.AimPos.performed -= OnAim;
        inputActions.Player.SelectMagic1.performed -= ctx => SelectMagic(0);
        inputActions.Player.SelectMagic2.performed -= ctx => SelectMagic(1);

        inputActions.Player.DisplayStat.performed -= OnDisPlayStat;
        inputActions.Player.DisplayMenu.performed -= OnDisPlayMenu;
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

    #region 입력 감지
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

    private void ToggleAttackMode(InputAction.CallbackContext ctx)
    {
        isMagic = !isMagic;
        UtilityManager.utility.PlaySFX(toggleAttackModeSFX);
    }

    private void OnDisPlayStat(InputAction.CallbackContext context)
    {
        if (canMove == false)
        {
            return;
        }
        DisplayStat();
    }

    private void OnDisPlayMenu(InputAction.CallbackContext context)
    {
        DisplayMenu();
    }
    #endregion

    #region Unity 제공 메서드
    void Start()
    {
        // 바 초기화, 스탯 변수들 다 임포트 되고 초기화해야 해서 얘는 Start에서 한 번 호츌 필요
        DisplayHP();
        DisplayMana();
    }

    // 즉각 반응해야 하는 모듈들은 Update()에 배치
    void Update()
    {
        // 타이머는 움직임 제어 권한 위에, 안그러면 디버프나 무적 안풀릴떄 생김
        // 무적시간일 경우 무적 타이머 초마다 차감하여 통상상태로 되돌림
        if (invincible == true)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                invincible = false;
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

    #region 체력, 게임 오버 관련
    // 플레이어 체력 수정
    public void ChangeHP(float value)
    {
        // 데미지일 경우 체크 사항
        if (value < 0)
        {
            // 이미 무적 시간이면 다음 단계 진입하지 않음
            if (invincible == true)
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

    // 무적 시간 동안 깜빡거리기 코루틴
    IEnumerator BlinkUntilInvincible()
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;
        // 무적이고 == 데미지를 입었고, 사망이 아니라면 깜빡임 시작
        while (invincible == true && isDie == false)
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
    #endregion

    #region UI 관련
    // HP 패널 표시
    private void DisplayHP()
    {
        HPBar.fillAmount = currentHP / maxHP;
    }

    public void DisplayMana()
    {
        manaBar.fillAmount = currentMana / maxMana;
    }

    // StatUI == Q, UI가 있으면 끄고 없으면 키기
    private void DisplayStat()
    {
        if (statUI.activeSelf == false)
        {
            WriteCurrentStat();
            DrawStatGem();
            statUI.SetActive(true);
        }
        else if (statUI.activeSelf == true)
        {
            statUI.SetActive(false);
        }
    }

    private void WriteCurrentStat()
    {
        TextMeshProUGUI HPText = statUI.transform.Find("HP/HPText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI MoneyText = statUI.transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI PowerText = statUI.transform.Find("Damage/DamageText").GetComponent<TextMeshProUGUI>();

        HPText.text = $" :  {currentHP} / {maxHP}";
        MoneyText.text = $" :  {money}";
        PowerText.text = $" :  {-playerAttack.attackDamage}";
    }
    private void DrawStatGem()
    {
        HPGemBar.RefreshGem(currentHP);
        moneyGemBar.RefreshGem(money);
        damageGemBar.RefreshGem(Mathf.Abs(playerAttack.attackDamage));
    }

    // esc로 메뉴 열고 닫기 가능 첫 화면으로, 리트라이 정도 기능 필요
    private void DisplayMenu()
    {
        if (menuUI.activeSelf == false)
        {
            menuUI.SetActive(true);
            canMove = false;
            canAttack = false;
            Time.timeScale = 0f;
        }
        else if (menuUI.activeSelf == true)
        {
            menuUI.SetActive(false);
            canMove = true;
            canAttack = true;
            Time.timeScale = 1f;
        }
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

    #region 스탯
    // Stat 수정 관련

    public void MaxHpPlus()
    {
        maxHP += 1;
        StartCoroutine(DisplayPlayerText("MaxHP+ !"));
        UtilityManager.utility.PlaySFX(maxHPPlusSFX);
        DisplayHP();
    }
    public void GetMoney(int plusMoney)
    {
        money += plusMoney;
        UtilityManager.utility.PlaySFX(moneySFX);
    }
    public void Attacklus()
    {
        playerAttack.attackDamage -= 1;
        StartCoroutine(DisplayPlayerText("Attack+ !"));
        UtilityManager.utility.PlaySFX(attackPlusSFX);
    }

    public void MaxJumpPlus()
    {
        playerMove.maxJump += 1;
        StartCoroutine(DisplayPlayerText("Jump+ !"));
        UtilityManager.utility.PlaySFX(jumpPlusSFX);
    }

    IEnumerator DisplayPlayerText(string displayText)
    {
        text.text = displayText;
        yield return new WaitForSeconds(DISPLAY_ITEM_EFFECT_TIME);
        text.text = "";
    }
#endregion

# region 메뉴 버튼 관련
    public void ReturnToMenuButton()
    {
        Time.timeScale = 1f;
        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        StartCoroutine(UtilityManager.utility.ChangeAlpha(fadeImage, FADE_OUT_ALPHA, fadeOutTime));
        yield return new WaitForSeconds(fadeOutTime);

        // 싱글톤들 리셋
        Destroy(GameManager.instance.gameObject);
        Destroy(UtilityManager.utility.gameObject);
        Destroy(ItemManager.itemManager.gameObject);
        Destroy(DataManager.dataManager.gameObject);
        Destroy(PlayerCtrl.player.gameObject);

        SceneManager.LoadScene(0);
    }

    public void ReloadSavedSceneButton()
    {
        Time.timeScale = 1f;
        StartCoroutine(ReloadSavedScene());
    }
    private IEnumerator ReloadSavedScene()
    {
        StartCoroutine(UtilityManager.utility.ChangeAlpha(fadeImage, FADE_OUT_ALPHA, fadeOutTime));
        yield return new WaitForSeconds(fadeOutTime);

        menuUI.SetActive(false);
        SceneManager.LoadScene(DataManager.dataManager.playerData.savedStage);
    }
    #endregion
}
