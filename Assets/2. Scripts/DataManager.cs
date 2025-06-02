using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어 데이터 구조 클래스
[System.Serializable]
public class PlayerData
{
    public float maxHP = 10.0f;
    public float currentHP;
    public int money = 0;
    public int maxJump = 1;
    public float attackDamage = -1.0f;
    public Vector2 savedSpawnPos;
    public string savedStage;
}

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
        if (dataManager == null)
        {
            dataManager = this;
            playerData = new PlayerData();
            playerData.currentHP = playerData.maxHP;
            savePath = Application.persistentDataPath + "/save.json";
        }
        else if (dataManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Save()
    {
        playerData.maxHP = PlayerCtrl.player.maxHP;
        playerData.currentHP = PlayerCtrl.player.currentHP;
        playerData.money = PlayerCtrl.player.money;
        playerData.maxJump = PlayerCtrl.player.playerMove.maxJump;
        playerData.attackDamage = PlayerCtrl.player.playerAttack.attackDamage;
        playerData.savedSpawnPos = GameManager.instance.readCurrentSpawnPos;
        playerData.savedStage = SceneManager.GetActiveScene().name;

        string jsonData = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, jsonData);
    }

    public bool Load()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            return true;
        }
        return false;
    }

    public void NewGame()
    {
        playerData.maxHP = 10.0f;
        playerData.currentHP = playerData.maxHP;
        playerData.money = 0;
        playerData.maxJump = 1;
        playerData.attackDamage = -1.0f;
        playerData.savedSpawnPos = new Vector2(-102.83f, -15.06f);
        playerData.savedStage = "1. First Stage";

        string jsonData = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, jsonData);
    }
}
