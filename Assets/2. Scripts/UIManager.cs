using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager uIManager = null;
    void Awake()
    {
        if (uIManager == null)
        {
            uIManager = this;
        }
        else if (uIManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
