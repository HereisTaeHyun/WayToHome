using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;
using System;

// 플레이어 데이터 구조 클래스
[System.Serializable]
public class PlayerData
{
    public float maxHP = 10.0f;
    public float currentHP;
    public float maxMana = 100.0f;
    public float currentMana;
    public int money = 0;
    public int maxJump = 1;
    public float attackDamage = -1.0f;
    public Vector2 savedSpawnPos;
    public string savedStage;
    public List<int> openedChests;
    public List<int> diedEnemy;
    public string[] usingMagic = new string[2];
    
    public void Reset()
    {
        maxHP = 100f;
        currentHP = maxHP;
        maxMana = 100f;
        currentMana = maxMana;
        money = 0;
        maxJump = 1;
        attackDamage = -10f;
        usingMagic = new string[2];
        savedSpawnPos = new(-126f, -15.06f);
        savedStage = "1. First Stage";
        openedChests = new List<int>();
        diedEnemy = new List<int>();
    }
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

            // 암호화 키 생성
            InitCrypto();

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
        playerData.maxMana = PlayerCtrl.player.maxMana;
        playerData.currentMana = PlayerCtrl.player.currentMana;
        playerData.money = PlayerCtrl.player.money;
        playerData.maxJump = PlayerCtrl.player.playerMove.maxJump;
        playerData.attackDamage = PlayerCtrl.player.playerAttack.attackDamage;
        playerData.savedSpawnPos = GameManager.instance.readCurrentSpawnPos;
        playerData.savedStage = SceneManager.GetActiveScene().name;

        var playerMagic = PlayerCtrl.player.playerAttack.usingMagic;
        for(int i = 0; i < playerData.usingMagic.Length; i++)
        {
            if(playerMagic[i])
            {
                var magic = playerMagic[i].GetComponent<PlayerMagicBase>();
                playerData.usingMagic[i] = magic.playerMagicType.ToString();
            }
            else
            {
                playerData.usingMagic[i] = "";
            }
        }

        string jsonData = JsonUtility.ToJson(playerData);
        string encryptedJson = Encryptor(jsonData);
        File.WriteAllText(savePath, encryptedJson);
    }

    public bool Load()
    {
        if (File.Exists(savePath))
        {
            string encryptedJson  = File.ReadAllText(savePath);
            string jsonData = Decryptor(encryptedJson);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            return true;
        }
        return false;
    }

    public void NewGame()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        playerData.Reset();

        string jsonData = JsonUtility.ToJson(playerData);
        string encryptedJson = Encryptor(jsonData);
        File.WriteAllText(savePath, encryptedJson);
    }
    #endregion

    #region 암복호화
    private void InitCrypto()
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
            File.WriteAllBytes(keyPath, key);
            File.WriteAllBytes(ivPath, iv);
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

    private string Encryptor(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(encrypted);
        }
    }

    private string Decryptor(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] cipherByte = Convert.FromBase64String(cipherText);

            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(cipherByte, 0, cipherByte.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
    #endregion
}
