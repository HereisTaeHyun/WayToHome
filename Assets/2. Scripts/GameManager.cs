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
    // 게임 오버 된 후 재시작하면 active false인 적을 true로 하는 식으로 씬 초기화 생각 중

    // public 변수
    // 게임 오버 이벤트
    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver;

    // private 변수
    [SerializeField] private GameObject gameOverPanel;
    private Image gameOverImage;
    private float alphaChangeTime = 1.5f;
    private static float GAME_OVER_IMAGE_ALPHA = 0.8f;
    private bool isGameOver;
    public bool readIsGameOver {get {return isGameOver;}}

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
