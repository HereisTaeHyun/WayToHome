using System;
using UnityEngine;

public class TrackingMine : MonoBehaviour
{
    // 인근을 스캐닝하여 가까이 가서 폭파하는 타입의 적 엔티티
    // 이후 가까이 간 후 폭파하는 타입, 가까이 간 후 사격하는 타입으로 구분 예정

    // public 변수

    // private 변수
    // range는 tag를 가져오는 범위로 EnemyCrtl 객체의 scanningRadius와 동일해야 함
    [SerializeField] private float expPower = 8.0f;
    private float scanningRadius;
    private float attack;
    EnemyCtrl enemyCtrl;

    // Player에 영향 미치는 부분
    PlayerCtrl playerCtrl;
    Rigidbody2D playerRb;

    void Start()
    {
        enemyCtrl = GetComponent<EnemyCtrl>();
        scanningRadius = enemyCtrl.readScanningRadius;
        attack = enemyCtrl.readAttack;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 폭파 후 오브젝트는 삭제
            Explosion();
            Destroy(gameObject);
        }
    }

    private void Explosion()
    {
        // 에셋 찾으면 파티클 Instantiate 후 destroy 추가 필요, 아직 에셋은 안찾았음

        // Player Layer인 3번 레이어가 range 내부에 있으면 target으로 가져오기
        Collider2D target = Physics2D.OverlapCircle(transform.position, scanningRadius, 1 << 3);
        playerCtrl = target.GetComponent<PlayerCtrl>();
        playerRb = target.GetComponent<Rigidbody2D>();

        // 왼쪽 or 오른쪽에 따라 폭파력에 따라 밀려남
        Vector2 playerMineVector = target.transform.position - transform.position;
        if(playerMineVector.x >= 0)
        {
            playerMineVector = new Vector2(1, 1);
        }
        else
        {
            playerMineVector = new Vector2(-1, 1);
        }
        playerRb.AddForce(playerMineVector * expPower, ForceMode2D.Impulse);

        
        // Player에게 데미지 가해
        playerCtrl.ChangeHP(attack);
    }
}
