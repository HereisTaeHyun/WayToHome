using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    // 싱글톤 선언
    public static ScreenUI screenUI = null;
    void Awake()
    {
        if(screenUI == null)
        {
            screenUI = this;
        }
        else if(screenUI != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
