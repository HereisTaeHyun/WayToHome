using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    // 충돌 방식 Enemy의 공통 행동에 대한 변수들, 사격 방식 Enemy는 추후 ShootingEnemyCtrl로 새로 제작 예정
    // 추적, 사망 및 아이템 드롭이 들어 갈 예정

    // public 변수

    // private 변수
    [SerializeField] Transform target;
    [SerializeField] float MaxHP = 10.0f;
    [SerializeField] float currentHP;
    [SerializeField] private float moveSpeed;

    // readonly 변수
    [SerializeField] private float scanningRadius;
    public float readScanningRadius {get {return scanningRadius;}}
    [SerializeField] private float attack;
    public float readAttack {get {return attack;}}

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentHP = MaxHP;
    }

    void FixedUpdate()
    {
        FollowingTarget(moveSpeed, scanningRadius);
    }
    
    // 사정 거리 내부에 집입하는 경우 따라가기 메서드
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
