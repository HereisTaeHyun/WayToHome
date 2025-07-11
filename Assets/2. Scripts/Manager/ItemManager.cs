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
    public ObjectPool<GameObject> attackPlusPool;
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

    private void Start()
    {
        UtilityManager.utility.CreateDoNotDestroyPool(ref healPool, healItem, 10, 10);
        UtilityManager.utility.CreateDoNotDestroyPool(ref moneyPool, moneyItem, 10, 10);
        UtilityManager.utility.CreateDoNotDestroyPool(ref goldPool, goldItem, 10, 10);
        UtilityManager.utility.CreateDoNotDestroyPool(ref premiumHealPool, PremiumHealItem, 10, 10);
        UtilityManager.utility.CreateDoNotDestroyPool(ref maxHpPlusPool, maxHpPlusItem, 10, 10);
        UtilityManager.utility.CreateDoNotDestroyPool(ref attackPlusPool, attackPlusItem, 10, 10);
        UtilityManager.utility.CreateDoNotDestroyPool(ref maxJumpPlusPool, maxJumpPlusItem, 10, 10);

        PoolData.Add(healItem, healPool);
        PoolData.Add(moneyItem, moneyPool);
        PoolData.Add(goldItem, goldPool);
        PoolData.Add(PremiumHealItem, premiumHealPool);
        PoolData.Add(maxHpPlusItem, maxHpPlusPool);
        PoolData.Add(attackPlusItem, attackPlusPool);
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
