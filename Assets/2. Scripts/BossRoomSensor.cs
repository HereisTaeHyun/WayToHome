using Unity.VisualScripting;
using UnityEngine;

public class BossRoomSensor : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;

    // 보스룸 오브젝트는 기본적으로 비활성화
    void Start()
    {
        boss.SetActive(false);
        leftWall.SetActive(false);
        rightWall.SetActive(false);
    }

    void Update()
    {
        if(boss == null)
        {
            leftWall.SetActive(false);
            rightWall.SetActive(false);
        }
    }

    // 플레이어가 센서 진입 시 보스룸 요소들 활성화
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && boss != null)
        {
            boss.SetActive(true);
            leftWall.SetActive(true);
            rightWall.SetActive(true);
        }
    }
}
