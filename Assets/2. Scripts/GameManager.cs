using System;
using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    // 게임매니저: 게임 오버 및 초기화, 풀 관리, Player UI 관리 맞길 예정
    // 몬스터는 고정 위치 배치할 것임, 아이템을 풀 관리로 맞길 수 있을지 알아보기

    // public 변수
    // 게임 오버 이벤트
    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver;

    // private 변수
    [SerializeField] private GameObject PlayerSpawnPos;
    [SerializeField] private GameObject gameOverPanel;
    private PlayerCtrl playerCtrl;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    private Image gameOverImage;
    private float alphaChangeTime = 1.5f;
    private static float GAME_OVER_IMAGE_ALPHA = 0.8f;
    private bool isGameOver;
    public bool readIsGameOver {get {return isGameOver;}}

    // 플레이어 초기 상태
    public float playerMaxHP;
    public float playerCurrentHP;
    public int playerMoney;
    public float playerAttackDamage;
    public int playerMaxJump;

    // 세이브한 플레이어 상태
    public float savedPlayerMaxHP;
    public float savedPlayerCurrentHP;
    public int savedPlayerMoney;
    public float savedPlayerAttackDamage;
    public int savedPlayerMaxJump;

    // 싱글톤 선언
    public static GameManager instance = null;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        // 시작에 있어 플레이어 초기화
        InitPlayer();
    }

    // Enable되면 함수 구독 및 UI 초기화
    void OnEnable()
    {
        isGameOver = false;
        gameOverPanel.SetActive(false);
        OnGameOver += GameOver;
    }
    // Disable되면 함수 구독 해제
    void OnDisable()
    {
        OnGameOver -= GameOver;
    }

    
    void Start()
    {
        gameOverImage = gameOverPanel.GetComponent<Image>();
        playerCtrl = GetComponent<PlayerCtrl>();
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        // if isGameOver = true;일 경우 다시하기 진입 가능하도록
        if(Input.GetButtonDown("Restart") && isGameOver == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            gameOverPanel.SetActive(false);
            isGameOver = false;
        }
    }

    // Player 시작 상태 초기화
    private void InitPlayer()
    {
        playerMaxHP = 10.0f;
        playerCurrentHP = playerMaxHP;
        playerMoney = 0;
        playerAttackDamage = -1.0f;
        playerMaxJump = 1;
    }

    // 플레이어 상태 읽어와 세이브
    public void SaveState()
    {
        savedPlayerMaxHP = playerCtrl.MaxHP;
        savedPlayerCurrentHP = playerCtrl.currentHP;
        savedPlayerMoney = playerCtrl.money;
        savedPlayerAttackDamage = playerAttack.attackDamage;
        savedPlayerMaxJump = playerMove.maxJump;
    }

    // ✅ 복원: 저장 상태를 현재 상태로 덮어쓰기
    public void LoadSavedState()
    {
        playerCtrl.MaxHP = savedPlayerMaxHP;
        playerCtrl.currentHP = savedPlayerCurrentHP;
        playerCtrl.money = savedPlayerMoney;
        playerAttack.attackDamage = savedPlayerAttackDamage;
        playerMove.maxJump = savedPlayerMaxJump;
    }

    // GameOver 되면 UI 호출
    private void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        // 사망시 UI 점진적으로 짙어지게
        StartCoroutine(UtilityManager.utility.ChangeAlpha(gameOverImage, GAME_OVER_IMAGE_ALPHA, alphaChangeTime));
    }

    // 다른 객체에서 OnGameOver 호출 시에 사용
    public void GameOverTrigger()
    {
        OnGameOver?.Invoke();
    }
}
