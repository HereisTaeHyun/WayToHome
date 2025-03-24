using UnityEngine;

public class PlayerUIProtecter : MonoBehaviour
{
    // 싱글톤 선언
    public static PlayerUIProtecter playerUIProtecter = null;
    void Awake()
    {
        if(playerUIProtecter == null)
        {
            playerUIProtecter = this;
        }
        else if(playerUIProtecter != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
