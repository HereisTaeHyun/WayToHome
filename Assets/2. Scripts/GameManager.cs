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

    public float baseMaxHP = 10.0f;
    public float baseCurrentHP;
    public int baseMoney = 0;
    public int baseMaxJump = 1;
    public float baseDamage = -1.0f;

    // private 변수
    [SerializeField] private GameObject PlayerSpawnPos;
    [SerializeField] private GameObject gameOverPanel;
<<<<<<< HEAD
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    private GameObject spawnPos;
=======
    private PlayerCtrl playerCtrl;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
    private Image gameOverImage;
    private float alphaChangeTime = 1.5f;
    private static float GAME_OVER_IMAGE_ALPHA = 0.8f;
    private bool isGameOver;
    public bool readIsGameOver {get {return isGameOver;}}

    // 플레이어 초기 상태
    public GameObject playerPrefab;
    private GameObject player;
    public GameObject spawnPos;
    public float playerMaxHP;
    public float playerCurrentHP;
    public int playerMoney;
    public float playerAttackDamage;
    public int playerMaxJump;

    // 세이브한 플레이어 상태
    public GameObject savedSpawnPos;
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
<<<<<<< HEAD
            spawnPos = GameObject.FindGameObjectWithTag("SpawnPos");
            baseCurrentHP = baseMaxHP;
            player = Instantiate(playerPrefab, spawnPos.transform.position, playerPrefab.transform.rotation);
            PlayerCtrl.player.Init();
=======
            // 시작에 있어 플레이어 초기화
            InitPlayer();
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Enable되면 함수 구독 및 UI 초기화
    void OnEnable()
    {
        isGameOver = false;
        gameOverPanel.SetActive(false);
        OnGameOver += GameOver;
<<<<<<< HEAD
        SceneManager.sceneLoaded += LoadPlayerStat;
        SceneManager.sceneLoaded += OnSceneLoaded;
=======
        SceneManager.sceneLoaded += Restart;
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
    }
    // Disable되면 함수 구독 해제
    void OnDisable()
    {
        OnGameOver -= GameOver;
<<<<<<< HEAD
        SceneManager.sceneLoaded -= LoadPlayerStat;
        SceneManager.sceneLoaded -= OnSceneLoaded;
=======
        SceneManager.sceneLoaded -= Restart;
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
    }

    // 씬 로드시 Player를 받아와 위치를 spawnPos에 설정
    // 씬 로드기에 플레이어 off면 on
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(PlayerCtrl.player.gameObject.activeSelf == false)
        {
            PlayerCtrl.player.gameObject.SetActive(true);
            PlayerCtrl.player.Init();
        }
        PlayerSet();
    }
    
    void Start()
    {
        gameOverImage = gameOverPanel.GetComponent<Image>();

        player = GameObject.FindWithTag("Player");
        playerCtrl = player.GetComponent<PlayerCtrl>();
        playerMove = player.GetComponent<PlayerMove>();
        playerAttack = player.GetComponent<PlayerAttack>();
    }

    void Update()
    {
        // if isGameOver = true;일 경우 다시하기 진입 가능하도록
<<<<<<< HEAD
        if(Input.GetButtonDown("Restart") && isGameOver == true && player.activeSelf == false)
=======
        if (Input.GetButtonDown("Restart") && isGameOver && player == null)
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
        {
            LoadSavedState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

<<<<<<< HEAD
    // 맵 이동 시에 상태 저장
    public void SavePlayerStat()
    {
        baseMaxHP = PlayerCtrl.player.MaxHP;
        baseCurrentHP = PlayerCtrl.player.currentHP;
        baseMoney = PlayerCtrl.player.money;
        baseMaxJump = PlayerCtrl.player.maxJump;
        baseDamage = PlayerCtrl.player.damage;
    }

    // 저장된 스테이트 로드
    public void LoadPlayerStat(Scene scene, LoadSceneMode mode)
    {
        PlayerCtrl.player.MaxHP = baseMaxHP;
        PlayerCtrl.player.currentHP = baseCurrentHP;
        PlayerCtrl.player.money = baseMoney;
        PlayerCtrl.player.maxJump = baseMaxJump;
        PlayerCtrl.player.damage = baseDamage;
    }

    // 게임 시작, 다시하기 등 씬 세팅에서 PlayerSet에 사용
    private void PlayerSet()
    {
        spawnPos = GameObject.FindGameObjectWithTag("SpawnPos");
        if(PlayerCtrl.player != null && spawnPos != null)
        {
            PlayerCtrl.player.transform.position = spawnPos.transform.position;
        }
    }

=======
    private void Restart(Scene scene, LoadSceneMode mode)
    {
        if (isGameOver) // 사망 후 재시작된 경우
        {
            spawnPos = GameObject.FindWithTag("SpawnPos");
            Instantiate(playerPrefab, spawnPos.transform.position, playerPrefab.transform.rotation);
            isGameOver = false;
            gameOverPanel.SetActive(false);
        }
    }

    // Player 시작 상태 초기화
    private void InitPlayer()
    {
        spawnPos = GameObject.FindWithTag("SpawnPos");
        Instantiate(playerPrefab, spawnPos.transform.position, playerPrefab.transform.rotation);
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

>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
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
