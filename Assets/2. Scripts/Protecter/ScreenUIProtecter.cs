using UnityEngine;

public class ScreenUIProtecter : MonoBehaviour
{
    // 싱글톤 선언
    public static ScreenUIProtecter screenUIProtecter = null;
    void Awake()
    {
        if(screenUIProtecter == null)
        {
            screenUIProtecter = this;
        }
        else if(screenUIProtecter != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
