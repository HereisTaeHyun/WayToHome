using UnityEngine;

public class PlayerProtecter : MonoBehaviour
{
    // 싱글톤 선언
    public static PlayerProtecter playerProtecter = null;
    void Awake()
    {
        if(playerProtecter == null)
        {
            playerProtecter = this;
        }
        else if(playerProtecter != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
