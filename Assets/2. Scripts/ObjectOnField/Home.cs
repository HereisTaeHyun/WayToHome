using UnityEngine;

public class Home : MonoBehaviour
{
    private bool isEnd;
    private float distance;
    void Start()
    {
        isEnd = false;
    }

    // 플레이어와 집이 충분히 가까워지면 End 메서드 활성화
    void Update()
    {
        distance = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);

        if(distance <= 9.0f && isEnd == false)
        {
            End();
        }
    }

    private void End()
    {
        isEnd = true;
        Debug.Log("End");
    }
}
