using UnityEngine;

public class BackgroundProtecter : MonoBehaviour
{
    // 싱글톤 선언
    public static BackgroundProtecter backgroundProtecter = null;
    void Awake()
    {
        if(backgroundProtecter == null)
        {
            backgroundProtecter = this;
        }
        else if(backgroundProtecter != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
