using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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

    public static UIManager uIManager = null;
    void Awake()
    {
        if (uIManager == null)
        {
            uIManager = this;
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

    public void OpenMagicShopUI()
    {
        magicShopUI.SetActive(true);
    }
    public void CloseMagicShopUI()
    {
        magicShopUI.SetActive(false);
    }
}
