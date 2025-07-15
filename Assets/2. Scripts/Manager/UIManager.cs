using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerInput playerInputActions;

    [Header("PlayerUsingUI")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private GameObject screenUI;
    [SerializeField] private GameObject screenPanel;
    [SerializeField] private Image screenImage;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI explainText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI damageText;
    public Image HpBar => hpBar;
    public Image ManaBar => manaBar;
    public TextMeshProUGUI HpText => hpText;
    public TextMeshProUGUI ManaText => manaText;
    public TextMeshProUGUI MoneyText => moneyText;
    public TextMeshProUGUI DamageText => damageText;
    public GameObject ScreenUI => screenUI;
    public GameObject ScreenPanel => screenPanel;

    [Header("MagicShopUI")]
    [SerializeField] private GameObject magicShopUI;
    [SerializeField] private Button[] magicButtons;
    [SerializeField] private GameObject[] magicPrefabs;
    public GameObject MagicShopUI => magicShopUI;
    private MagicShop currentMagicShop;

    [Header("StallUI")]
    [SerializeField] private GameObject stallUI;
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private SellingStat[] sellingStats;
    public GameObject StallUI => stallUI;
    private Stall currentStall;

    [Header("MenuUI")]
    [SerializeField] private GameObject menuUI;

    private float alphaChangeTime = 1.5f;
    private static float GAME_OVER_IMAGE_ALPHA = 0.8f;
    private static float FADE_OUT_ALPHA = 1.0f;

    public static UIManager uIManager = null;
    void Awake()
    {
        if (uIManager == null)
        {
            uIManager = this;
            playerInputActions = new PlayerInput();
        }
        else if (uIManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Enable되면 함수 구독 및 UI 초기화
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // Disable되면 함수 구독 해제
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerUI.SetActive(true);
        magicShopUI.SetActive(false);
        stallUI.SetActive(false);
    }

    
    public void ScreeUISet()
    {
        StopAllCoroutines();

        stateText.text = "";
        explainText.text = "";

        screenImage = screenPanel.GetComponent<Image>();
        Color currentColor = new Color32(0, 0, 0, 0);
        screenImage.color = currentColor; 
    }

    public void GameOverScreen()
    {
        stateText.text = "You Died";
        explainText.text = "Press R to Restart";

        // 사망시 UI 점진적으로 짙어지게
        Color currentColor = new Color32(255, 30, 30, 0);
        screenImage.color = currentColor; 
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, GAME_OVER_IMAGE_ALPHA, alphaChangeTime));
    }

    public void EndScreen()
    {
        stateText.text = "Thanks for your play";
        explainText.text = "Press R to return to menu";

        screenImage = screenPanel.GetComponent<Image>();
        Color currentColor = new Color32(0, 0, 0, 0);
        screenImage.color = currentColor; 
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, GAME_OVER_IMAGE_ALPHA, alphaChangeTime));
    }

    #region PlayerUI
    public void ClosePlayerUI()
    {
        if (GameManager.instance.readIsGameOver == true)
        {
            playerUI.SetActive(false);
        }
    }

    public void RefreshPlayerUI()
    {
        playerUI.SetActive(true);
        PlayerCtrl.player.DisplayHP();
        PlayerCtrl.player.DisplayMana();
    }
    #endregion

    #region MagicShop
    public void OpenMagicShopUI(MagicShop magicShop)
    {
        for (int i = 0; i < magicButtons.Length; i++)
        {
            int idx = i;
            magicButtons[idx].onClick.AddListener(() => BuyMagic(magicPrefabs[idx]));
        }

        currentMagicShop = magicShop;
        magicShopUI.SetActive(true);
    }
    public void CloseMagicShopUI()
    {
        currentMagicShop = null;
        magicShopUI.SetActive(false);
    }
    private void BuyMagic(GameObject magicPrefab)
    {
        if (currentMagicShop == null)
        {
            return;
        }
        currentMagicShop.BuyMagic(magicPrefab);
    }
    #endregion

    #region Stall
    public void OpenStallUI(Stall stall)
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int idx = i;
            itemButtons[idx].onClick.AddListener(() => BuyItem(sellingStats[idx]));
        }

        currentStall = stall;
        stallUI.SetActive(true);
    }
    public void CloseStallUI()
    {
        currentStall = null;
        stallUI.SetActive(false);
    }
    private void BuyItem(SellingStat sellingStat)
    {
        if (currentStall == null)
        {
            return;
        }
        currentStall.BuyItem(sellingStat);
    }
    #endregion

    #region MenuUI
    public void OpenMenuUI()
    {
        PlayerCtrl.player.canMove = false;
        PlayerCtrl.player.canAttack = false;

        Time.timeScale = 0.0f;
        menuUI.SetActive(true);
    }

    public void CloseMenuUI()
    {
        PlayerCtrl.player.canMove = true;
        PlayerCtrl.player.canAttack = true;

        Time.timeScale = 1.0f;
        menuUI.SetActive(false);
    }

    public void ReturnToMenuButton()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(ReturnToMenu());
    }
    
    private IEnumerator ReturnToMenu()
    {
        menuUI.SetActive(false);

        Color currentColor = new Color32(0, 0, 0, 0);
        screenImage.color = currentColor; 
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, FADE_OUT_ALPHA, alphaChangeTime));

        yield return new WaitForSeconds(alphaChangeTime);

        // 싱글톤들 리셋
        Destroy(GameManager.instance.gameObject);
        Destroy(UtilityManager.utility.gameObject);
        Destroy(ItemManager.itemManager.gameObject);
        Destroy(DataManager.dataManager.gameObject);
        Destroy(UIManager.uIManager.gameObject);
        Destroy(PlayerCtrl.player.gameObject);

        SceneManager.LoadScene(0);
    }

    public void RestartFromSaveButton()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(RestartFromSave());
    }

    private IEnumerator RestartFromSave()
    {
        menuUI.SetActive(false);
        
        Color currentColor = new Color32(0, 0, 0, 0);
        screenImage.color = currentColor; 
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, FADE_OUT_ALPHA, alphaChangeTime));

        yield return new WaitForSeconds(alphaChangeTime);

        SceneManager.LoadScene(DataManager.dataManager.playerData.savedStage);
    }
    #endregion
}
