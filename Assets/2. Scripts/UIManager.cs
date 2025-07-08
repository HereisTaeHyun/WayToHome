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

    [Header("MagicShop")]
    [SerializeField] private GameObject magicShopUI;
    [SerializeField] private Button[] magicButtons;
    [SerializeField] private GameObject[] magicPrefabs;
    public GameObject MagicShopUI => magicShopUI;
    private MagicShop currentMagicShop;

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

    void Start()
    {
        for (int i = 0; i < magicButtons.Length; i++)
        {
            int idx = i;
            magicButtons[idx].onClick.AddListener(() => BuyMagic(magicPrefabs[idx]));
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerUI.SetActive(true);
        magicShopUI.SetActive(false);
    }

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

    public void OpenMagicShopUI(MagicShop magicShop)
    {
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
}
