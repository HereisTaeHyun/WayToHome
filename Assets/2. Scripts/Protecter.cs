using UnityEngine;

public class Protecter : MonoBehaviour
{
    // 싱글톤 선언
    public static Protecter protecter = null;
    void Awake()
    {
        if(protecter == null)
        {
            protecter = this;
        }
        else if(protecter != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
