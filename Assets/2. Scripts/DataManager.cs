using UnityEngine;

// 플레이어 데이터 구조 클래스
[System.Serializable]
public class PlayerData
{
    public float maxHP = 10.0f;
    public float currentHP;
    public int money = 0;
    public int maxJump = 1;
    public float attackDamage = -1.0f;
    public Vector2 currentSpawnPos;
}

    // [NonSerialized] public float baseMaxHP = 10.0f;
    // [NonSerialized] public float baseCurrentHP;
    // [NonSerialized] public int baseMoney = 0;
    // [NonSerialized] public int baseMaxJump = 1;
    // [NonSerialized] public float baseDamage = -1.0f;

public class DataManager : MonoBehaviour
{

    // 데이터를 json화하여 저장할 예정
    // save는 DataManager에서 하지만 SceneLoad 문제로 Load는 GameManager에서
    public PlayerData playerData { get; private set; }
    private static string savePath;

    // 싱글톤 선언
    public static DataManager dataManager = null;
    void Awake()
    {
        if(dataManager == null)
        {
            dataManager = this;
        }
        else if(dataManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        savePath = Application.persistentDataPath + "/save.json";
        playerData = new PlayerData();
        playerData.currentHP = playerData.maxHP;
    }

    public void Save()
    {
        playerData.maxHP = PlayerCtrl.player.maxHP;
        playerData.currentHP = PlayerCtrl.player.currentHP;
        playerData.money = PlayerCtrl.player.money;
        playerData.maxJump = PlayerCtrl.player.playerMove.maxJump;
        playerData.attackDamage = PlayerCtrl.player.playerAttack.attackDamage;
        playerData.currentSpawnPos = GameManager.instance.readCurrentSpawnPos;
    }
}
