using System;
using UnityEngine;

public class TrackingMine : MonoBehaviour
{
    // 인근을 스캐닝하여 Player Layer 감지 시 가까이 가서 폭파하는 타입의 적 엔티티
    // 이후 가까이 간 후 폭파하는 타입, 가까이 간 후 사격하는 타입으로 구분 예정
    // OnCollision하는 경우 폭파 객체 활성화?
    // 플레이어가 사살하는 경우 item 랜덤 드랍 기능 필요

    public float MaxHP = 10.0f;
    public float currentHP;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float scanningRadius = 10.0f;
    [SerializeField] private float range = 10.0f;
    [SerializeField] private float attack = -2.0f;
    [SerializeField] private float expPower = 5.0f;
    EnemyTracking enemyTracking;

    // Player에 영향 미치는 부분
    PlayerCtrl playerCtrl;
    Rigidbody2D playerRb;
    
    void Start()
    {
        currentHP = MaxHP;
        enemyTracking = GetComponent<EnemyTracking>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 플레이어가 scanningRadius 내부면 moveSpeed만큼 이동 시작, 스캔 범위 밖이면 추적 안 함
        enemyTracking.FollowingTarget(moveSpeed, scanningRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            // 폭파
            Explosion();
            // 폭파 후 오브젝트는 삭제
            Destroy(gameObject);
        }
    }

    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         Explosion();
    //     }
    // }

    private void Explosion()
    {
        // 에셋 찾으면 파티클 Instantiate 후 destroy 추가 필요

        // Player Layer인 3번 레이어가 range 내부에 있으면 target으로 가져오기
        Collider2D target = Physics2D.OverlapCircle(transform.position, range, 1 << 3);
        playerCtrl = target.GetComponent<PlayerCtrl>();
        playerRb = target.GetComponent<Rigidbody2D>();

        // Player에게 데미지 가해
        playerCtrl.ChangeHP(attack);

        // 폭파력에 따라 밀려남
        Vector2 expVector = target.transform.position - transform.position;;
        playerRb.AddForce(expVector * expPower, ForceMode2D.Impulse);

        Debug.Log("Addimpuse?");
    }
}
