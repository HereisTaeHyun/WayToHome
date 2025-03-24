using UnityEngine;

public class CameraProtecter : MonoBehaviour
{
    // 싱글톤 선언
    public static CameraProtecter cameraProtecter = null;
    void Awake()
    {
        if(cameraProtecter == null)
        {
            cameraProtecter = this;
        }
        else if(cameraProtecter != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
