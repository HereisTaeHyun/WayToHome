using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public Dictionary<GameObject, float> ItemNormalizer(Dictionary<GameObject, float> inputItem)
    {
        List<GameObject> keys = new List<GameObject>(inputItem.Keys);
        float sumValues = inputItem.Sum(item => item.Value);
        foreach(GameObject elem in keys)
        {
            inputItem[elem] = (inputItem[elem] / sumValues) * 100.0f;
        }
        return inputItem;
    }
}
