using UnityEngine;

    // 플레이어 데이터 구조 클래스
[System.Serializable]
public class PlayerData
{
    public float MaxHP;
    public float currentHP;
    public int money;
    public int maxJump;
    public float attackDamage;
    public Vector2 currentSpawnPos;
}

public class DataManager : MonoBehaviour
{
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
    }
}
