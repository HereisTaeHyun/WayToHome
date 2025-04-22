using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FireBall : MonoBehaviour
{
    // 인근을 스캐닝하여 가까이 가서 공격하는 타입의 적 엔티티

    // public 변수

    // private 변수
    // damage는 EnemyCtrl 설정 값 이용
    public float currentHP;
    private float moveSpeed = 3.0f;
    private float scanningRadius = 10.0f;
    private float damage = -1.0f;
    private Transform target;
    private PlayerCtrl playerCtrl;
    private Rigidbody2D rb2D;
    private ObjectPool<GameObject> originPool;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        FollowingTarget(moveSpeed, scanningRadius);
    }

    public void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
    }

    private void FollowingTarget(float moveSpeed, float scanningRadius)
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius)
            {
                // 플레이어에게 이동
                Vector2 newPosition = Vector2.MoveTowards(rb2D.position, target.position, moveSpeed * Time.fixedDeltaTime);
                rb2D.MovePosition(newPosition);
            }
            else if (Vector2.Distance(transform.position, target.position) > scanningRadius)
            {
                originPool.Release(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                playerCtrl = other.GetComponent<PlayerCtrl>();

                // 플레이어가 무적이 아니라면 공격
                if(playerCtrl.readInvincible != true)
                {
                    playerCtrl.ChangeHP(damage);
                    originPool.Release(gameObject);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanningRadius);
    }
}
