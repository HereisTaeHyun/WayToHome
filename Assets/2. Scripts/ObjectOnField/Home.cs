using UnityEngine;

public class Home : MonoBehaviour
{
    private float distance;
    void Start()
    {
        
    }

    // 플레이어와 집이 충분히 가까워지면 End 메서드 활성화
    void Update()
    {
        distance = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);

        Debug.Log(distance);

        if(distance <= 5.0f)
        {
            End();
        }
    }

    private void End()
    {
        Debug.Log("End");
    }
}
