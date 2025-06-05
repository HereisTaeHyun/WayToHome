using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Pool;
using Image = UnityEngine.UI.Image;

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

    private GameObject screenUI;
    private GameObject screenPanel;
    private TextMeshProUGUI stateText;
    private TextMeshProUGUI restartText;
    private Image screenImage;
    private float alphaChangeTime = 1.5f;
    private static float GAME_OVER_IMAGE_ALPHA = 0.8f;

    private GameObject player;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;

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

            // 플레이어 초기화
            PlayerCtrl.player.Init();
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
        OnGameOver += GameOver;
        SceneManager.sceneLoaded += LoadPlayerStat;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // Disable되면 함수 구독 해제
    void OnDisable()
    {
        OnGameOver -= GameOver;
        SceneManager.sceneLoaded -= LoadPlayerStat;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드시 Player를 받아와 위치를 spawnPos에 설정
    // 씬 로드기에 플레이어 off면 == 사망시 on 및 다시 초기화
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // UI Set
        ScreeUISet();

        // 포탈을 통해서 넘어온 경우 해당 맵의 firstSpawnPos를 찾아야 함
        if(usePortal == true)
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

    void Update()
    {
        // if isGameOver = true;일 경우 다시하기 진입 가능하도록
        if(Input.GetButtonDown("Restart") && isGameOver == true && player.activeSelf == false)
        {
            SceneManager.LoadScene(DataManager.dataManager.playerData.savedStage);
            isGameOver = false;
        }
        
        // if isEnd = true;일 경우 첫 씬 진입 가능하도록
        if(Input.GetButtonDown("Restart") && isEnd == true)
        {
            // 싱글톤들 리셋
            Destroy(GameManager.instance.gameObject);
            Destroy(UtilityManager.utility.gameObject);
            Destroy(ItemManager.itemManager.gameObject);
            Destroy(PlayerCtrl.player.gameObject);

            SceneManager.LoadScene(0);
            isEnd = false;
        }
    }

    // 다시하기 세팅에서 사용
    private void PlayerSet()
    {
        if(PlayerCtrl.player != null && currentSpawnPos != null)
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

    private void ScreeUISet()
    {
        screenUI = GameObject.FindGameObjectWithTag("ScreenUI");
        screenPanel = screenUI.transform.Find("ScreenPanel").gameObject;
        stateText = screenPanel.transform.Find("StateText").gameObject.GetComponent<TextMeshProUGUI>();
        restartText = screenPanel.transform.Find("RestartText").gameObject.GetComponent<TextMeshProUGUI>();

        stateText.text = "You Died";
        restartText.text = "Press R to Restart";

        screenImage = screenPanel.GetComponent<Image>();
        Color currentColor = new Color32(255, 255, 255, 0);
        screenImage.color = currentColor; 

        screenPanel.SetActive(false);
    }

    // 저장된 스테이트 로드 및 적용
    public void LoadPlayerStat(Scene scene, LoadSceneMode mode)
    {
        DataManager.dataManager.Load();

        PlayerCtrl.player.maxHP = DataManager.dataManager.playerData.maxHP;
        PlayerCtrl.player.currentHP = DataManager.dataManager.playerData.currentHP;
        PlayerCtrl.player.money = DataManager.dataManager.playerData.money;
        PlayerCtrl.player.playerMove.maxJump = DataManager.dataManager.playerData.maxJump;
        PlayerCtrl.player.playerAttack.attackDamage = DataManager.dataManager.playerData.attackDamage;
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
        screenPanel.SetActive(true);
        // 사망시 UI 점진적으로 짙어지게
        Color currentColor = new Color32(255, 30, 30, 0);
        screenImage.color = currentColor; 
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, GAME_OVER_IMAGE_ALPHA, alphaChangeTime));
    }

    // 다른 객체에서 OnGameOver 호출 시에 사용
    public void GameOverTrigger()
    {
        OnGameOver?.Invoke();
    }

    public void End()
    {
        isEnd = true;
        screenPanel.SetActive(true);
        stateText.text = "Thanks for your play";
        restartText.text = "Press R to return to menu";

        screenImage = screenPanel.GetComponent<Image>();
        Color currentColor = new Color32(255, 255, 255, 0);
        screenImage.color = currentColor; 
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, GAME_OVER_IMAGE_ALPHA, alphaChangeTime));
    }
}
