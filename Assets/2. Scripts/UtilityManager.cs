using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

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

        
    // UI 알파 변경자   
    public IEnumerator ChangeAlpha(Image changeTarget, float targetAlpah, float changeTime)
    {
        Color currentColor = changeTarget.color;
        float time = 0.0f;

        // 현재 알파가 목표 알파보다 작은 동안 점진적으로 알파 값 변경
        while(currentColor.a <= targetAlpah)
        {
            time += Time.deltaTime / changeTime;
            currentColor.a = Mathf.Lerp(0.0f, targetAlpah, time);
            changeTarget.color = currentColor;
            yield return null;
        }
    }
}
