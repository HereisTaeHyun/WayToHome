using UnityEngine;

public class DataManager : MonoBehaviour
{
    // 싱글톤 선언
    public static DataManager playerData = null;
    void Awake()
    {
        if(playerData == null)
        {
            playerData = this;
        }
        else if(playerData != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
