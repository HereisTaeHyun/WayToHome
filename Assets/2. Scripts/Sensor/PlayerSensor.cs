using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    protected bool isEntered;

    protected virtual void Start()
    {
        isEntered = false;
    }
    
    // 플레이어가 센서 진입
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && isEntered == false)
        {
            isEntered = true;
        }
    }
}
