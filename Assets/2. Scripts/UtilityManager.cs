using UnityEngine;

public class UtilityManager : MonoBehaviour
{
    // 싱글톤 선언
    public static UtilityManager utility = null;
    void Awake()
    {
        if(utility == null)
        {
            utility = this;
        }
        else if(utility != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public Vector2 DirSet(Vector2 move)
    {
        Vector2 moveDir = new Vector2(0, 0);
        if(Mathf.Approximately(move.x, 0) == false)
        {
            moveDir.Set(move.x, 0);
            moveDir.Normalize();
        }
        return moveDir;
    }

    // public float[] ItemProbabilityNormalizer()
    // {
    // }
}
