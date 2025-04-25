using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemManager : MonoBehaviour
{
    
    public Dictionary<GameObject, ObjectPool<GameObject>> PoolData = new Dictionary<GameObject, ObjectPool<GameObject>>();
    public ObjectPool<GameObject> healPool;
    public ObjectPool<GameObject> moneyPool;
    public ObjectPool<GameObject> goldPool;
    public ObjectPool<GameObject> premiumHealPool;
    public ObjectPool<GameObject> maxHpPlusPool;
    public ObjectPool<GameObject> attakPlusPool;
    public ObjectPool<GameObject> maxJumpPlusPool;

    [SerializeField] private GameObject healItem;
    [SerializeField] private GameObject moneyItem;
    [SerializeField] private GameObject goldItem;
    [SerializeField] private GameObject PremiumHealItem;
    [SerializeField] private GameObject maxHpPlusItem;
    [SerializeField] private GameObject attackPlusItem;
    [SerializeField] private GameObject maxJumpPlusItem;


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
        UtilityManager.utility.CreatePool(ref maxJumpPlusPool, maxJumpPlusItem, 3, 3);

        PoolData.Add(healItem, healPool);
        PoolData.Add(moneyItem, moneyPool);
        PoolData.Add(goldItem, goldPool);
        PoolData.Add(PremiumHealItem, premiumHealPool);
        PoolData.Add(maxHpPlusItem, maxHpPlusPool);
        PoolData.Add(attackPlusItem, attakPlusPool);
        PoolData.Add(maxJumpPlusItem, maxJumpPlusPool);
    }

    public ObjectPool<GameObject> SelectPool(GameObject prefab)
    {
        if(PoolData.TryGetValue(prefab, out var pool))
        {
            return pool;
        }
        return null;
    }
}
