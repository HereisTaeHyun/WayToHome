using UnityEngine;

public class EnemyTracking : MonoBehaviour
{
    // public 변수
    public Transform target;

    // private 변수

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // 따라가기 메서드
    public void FollowingTarget(float moveSpeed, float scanningRadius)
    {
        if(target != null)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }
    }
}
