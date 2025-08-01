using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Pool;
using Image = UnityEngine.UI.Image;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // public 변수

    // 게임 오버 이벤트
    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver;
    public Dictionary<GameObject, ObjectPool<GameObject>> magicPools = new Dictionary<GameObject, ObjectPool<GameObject>>();

    public bool usePortal = false;

    // private 변수
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private GameObject shurikenPrefab;
    [SerializeField] private GameObject smallShockwavePrefab;
    [SerializeField] private GameObject waterSplashPrefab;
    private BGMCtrl bGMCtrl;

    private PlayerInput playerInputActions;

    private GameObject player;
    private Dictionary<PlayerMagicType, GameObject> magicPrefabs;

    private GameObject cam;
    private CinemachineCamera camComp;
    private CameraCtrl cameraCtrl;

    private Transform firstSpawnPos;
    private Vector2 currentSpawnPos;
    public Vector2 readCurrentSpawnPos {get {return currentSpawnPos;}}

    private bool isGameOver;
    public bool readIsGameOver {get {return isGameOver;}}
    private bool isEnd;

    // 싱글톤 선언
    public static GameManager instance = null;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            isEnd = false;
            player = Instantiate(playerPrefab, currentSpawnPos, playerPrefab.transform.rotation);
            playerInputActions = new PlayerInput();

            // 플레이어 초기화
            PlayerCtrl.player.Init();
            magicPrefabs = new Dictionary<PlayerMagicType, GameObject>
            {
                {PlayerMagicType.Kunai, kunaiPrefab},
                {PlayerMagicType.Shuriken, shurikenPrefab},
                {PlayerMagicType.SmallShockwave, smallShockwavePrefab},
                {PlayerMagicType.WaterSplash, waterSplashPrefab},
            };
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
        if (instance != this)
        {
            return;
        }

        isGameOver = false;
        OnGameOver += GameOver;
        SceneManager.sceneLoaded += LoadPlayerStat;
        SceneManager.sceneLoaded += OnSceneLoaded;

        playerInputActions.Enable();
        playerInputActions.Player.Restart.performed += Restart;
    }
    // Disable되면 함수 구독 해제
    void OnDisable()
    {
        if (instance != this)
        {
            return;
        }

        OnGameOver -= GameOver;
        SceneManager.sceneLoaded -= LoadPlayerStat;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        playerInputActions.Disable();
        playerInputActions.Player.Restart.performed -= Restart;
    }

    // 씬 로드시 Player를 받아와 위치를 spawnPos에 설정
    // 씬 로드기에 플레이어 off면 == 사망시 on 및 다시 초기화
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // UI Set
        UIManager.uIManager.ScreeUISet();
        bGMCtrl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BGMCtrl>();

        // 포탈을 통해서 넘어온 경우 해당 맵의 firstSpawnPos를 찾아야 함
        if (usePortal == true)
        {
            firstSpawnPos = GameObject.FindGameObjectWithTag("FirstSpawnPos").transform;
            currentSpawnPos = new Vector2(firstSpawnPos.position.x, firstSpawnPos.position.y);
            usePortal = false;
        }
        else
        {
            currentSpawnPos = DataManager.dataManager.playerData.savedSpawnPos;
        }

        // 플레이어 위치, 저장된 스탯 set
        if(PlayerCtrl.player.gameObject.activeSelf == false)
        {
            PlayerCtrl.player.gameObject.SetActive(true);
            PlayerCtrl.player.Init();
        }
        PlayerSet();
        StartCoroutine(CameraSetAfterFrame());
    }
    
    public void Restart(InputAction.CallbackContext context)
    {
        if (isGameOver == true && player.activeSelf == false)
        {
            // 죽은 후 재시작
            SceneManager.LoadScene(DataManager.dataManager.playerData.savedStage);
            isGameOver = false;
            return;
        }

        if (isEnd == true)
        {
            // 엔딩 이후 처음으로
            Destroy(GameManager.instance.gameObject);
            Destroy(UtilityManager.utility.gameObject);
            Destroy(ItemManager.itemManager.gameObject);
            Destroy(DataManager.dataManager.gameObject);
            Destroy(UIManager.uIManager.gameObject);
            Destroy(PlayerCtrl.player.gameObject);

            SceneManager.LoadScene(0);
            isEnd = false;
        }
    }

    // 다시하기 세팅에서 사용
    private void PlayerSet()
    {
        if (PlayerCtrl.player != null && currentSpawnPos != null)
        {
            PlayerCtrl.player.transform.position = currentSpawnPos;
        }
    }

     // 가끔 카메라 생성보다 CameraSet()이 먼저 호출되는 경우 방지
    IEnumerator CameraSetAfterFrame()
    {
        yield return null;
        CameraSet();
    }
    public void CameraSet()
    {
        // 카메라 위치 설정 = player 위치
        cam = GameObject.FindGameObjectWithTag("Camera");
        camComp = cam.GetComponent<CinemachineCamera>();
        camComp.ForceCameraPosition(PlayerCtrl.player.transform.position, Quaternion.identity);

        // 콘파이너 찾기
        RaycastHit2D hit = Physics2D.Raycast(PlayerCtrl.player.transform.position, Vector2.up, 0.5f);
        if(hit.collider.CompareTag("Confiner"))
        {
            cameraCtrl = cam.GetComponent<CameraCtrl>();
            PolygonCollider2D polygonCollider2D = hit.collider.GetComponent<PolygonCollider2D>();
            cameraCtrl.ConfinerChanger(polygonCollider2D);
        }
    }

    // 저장된 스테이트 로드 및 적용
    public void LoadPlayerStat(Scene scene, LoadSceneMode mode)
    {
        DataManager.dataManager.Load();

        PlayerCtrl.player.maxHP = DataManager.dataManager.playerData.maxHP;
        PlayerCtrl.player.currentHP = DataManager.dataManager.playerData.currentHP;
        PlayerCtrl.player.maxMana = DataManager.dataManager.playerData.maxMana;
        PlayerCtrl.player.currentMana = DataManager.dataManager.playerData.currentMana;
        PlayerCtrl.player.money = DataManager.dataManager.playerData.money;
        PlayerCtrl.player.playerMove.maxJump = DataManager.dataManager.playerData.maxJump;
        PlayerCtrl.player.playerAttack.attackDamage = DataManager.dataManager.playerData.attackDamage;

        var playerMagic = PlayerCtrl.player.playerAttack.usingMagic;
        var savedMagic = DataManager.dataManager.playerData.usingMagic;
        for (int i = 0; i < playerMagic.Length; i++)
        {
            playerMagic[i] = null;  // 초기화

            string id = savedMagic[i];
            if (string.IsNullOrEmpty(id))
            {
                continue;
            }

            if (Enum.TryParse(id, out PlayerMagicType type) && magicPrefabs.TryGetValue(type, out var prefab))
            {
                UtilityManager.utility.CreatePlayerMagicPool(prefab);
                playerMagic[i] = prefab;
            }
        }
    }

    // SpawnPos 셋업
    public void SetSpawnPos(Vector2 newSpawnPos)
    {
        currentSpawnPos = newSpawnPos;
    }


    // GameOver 되면 UI 호출
    private void GameOver()
    {
        isGameOver = true;
        bGMCtrl.PlayGameOverBGM();
        UIManager.uIManager.ClosePlayerUI();
        UIManager.uIManager.GameOverScreen();
    }

    // 다른 객체에서 OnGameOver 호출 시에 사용
    public void GameOverTrigger()
    {
        OnGameOver?.Invoke();
    }

    public void End()
    {
        isEnd = true;
        UIManager.uIManager.EndScreen();
    }
}
