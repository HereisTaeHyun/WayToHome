using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using UnityEngine.LightTransport;
using System.Text;
using System;

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
    public List<int> openedChests;
    public List<int> didedEnemy;
}

public class DataManager : MonoBehaviour
{

    // 데이터를 json화하여 저장할 예정
    // save는 DataManager에서 하지만 SceneLoad 문제로 Load는 GameManager에서
    public PlayerData playerData { get; private set; }
    private byte[] key;
    private byte[] iv;
    int keyLength = 32;
    int ivLength = 16;
    private static string savePath;
    private static string keyPath;
    private static string ivPath;

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
            keyPath = Application.persistentDataPath + "/aesKey.dat";
            ivPath = Application.persistentDataPath + "/aesIV.dat";
        }
        else if (dataManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    #region 세이브로드
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
        playerData.openedChests = new List<int>();
        playerData.didedEnemy = new List<int>();

        string jsonData = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, jsonData);
    }
    #endregion

    #region 암복호화
    private void Crypto()
    {
        if (File.Exists(keyPath) && File.Exists(ivPath))
        {
            key = File.ReadAllBytes(keyPath);
            iv = File.ReadAllBytes(ivPath);
        }
        else
        {
            key = GenerateRandomByte(keyLength);
            iv = GenerateRandomByte(ivLength);
        }
    }

    // 랜덤 바이트 배열 생성기
    private byte[] GenerateRandomByte(int length)
    {
        byte[] randomBytes = new byte[length];
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }
        return randomBytes;
    }

    private string Encrypter(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
            return Convert.ToBase64String(encrypted);
        }
    }

    private string Decrypter(string cipherText)
    {
        byte[] cipherByte = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(cipherByte, 0, cipherByte.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
    #endregion
}
