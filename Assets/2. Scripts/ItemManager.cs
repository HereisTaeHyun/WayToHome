using UnityEngine;
using UnityEngine.Pool;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject healItem;
    [SerializeField] private GameObject moneyItem;
    [SerializeField] private GameObject goldItem;
    [SerializeField] private GameObject PremiumHealItem;
    [SerializeField] private GameObject maxHpPlusItem;
    [SerializeField] private GameObject attackPlusItem;
    [SerializeField] private GameObject maxJumpPlusItem;

    public ObjectPool<GameObject> healPool;
    public ObjectPool<GameObject> moneyPool;
    public ObjectPool<GameObject> goldPool;
    public ObjectPool<GameObject> premiumHealPool;
    public ObjectPool<GameObject> maxHpPlusPool;
    public ObjectPool<GameObject> attakPlusPool;
    public ObjectPool<GameObject> maxJumpPlusPool;

    // 싱글톤 선언
    public static ItemManager itemManager = null;
    void Awake()
    {
        if(itemManager == null)
        {
            itemManager = this;
        }
        else if(itemManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        UtilityManager.utility.CreatePool(ref healPool, healItem, 3, 3);
        UtilityManager.utility.CreatePool(ref moneyPool, moneyItem, 10, 10);
        UtilityManager.utility.CreatePool(ref goldPool, goldItem, 3, 3);
        UtilityManager.utility.CreatePool(ref premiumHealPool, PremiumHealItem, 3, 3);
        UtilityManager.utility.CreatePool(ref maxHpPlusPool, maxHpPlusItem, 3, 3);
        UtilityManager.utility.CreatePool(ref attakPlusPool, attackPlusItem, 3, 3);
        UtilityManager.utility.CreatePool(ref maxJumpPlusPool, maxJumpPlusItem, 1, 1);
    }

    // 각 아이템 드롭하는 오브젝트들이 어떤 풀에서 가져와야할지 알게 하기 위해 작성해보기
    // 아이템을 받고 스위치로 어떤 아이템인지에 따라 pool 지정해주기
    // private void SelectPool
}
