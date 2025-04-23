using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FireBall : MonoBehaviour
{
    // 인근을 스캐닝하여 가까이 가서 공격하는 타입의 적 엔티티

    // public 변수
    public float maxHP = 2.0f;
    public float currentHP;
    public bool isHited;
    public bool canMove = true;

    // private 변수
    private bool isPool;
    private bool canAttack;
    private float moveSpeed = 3.0f;
    private float scanningRadius = 10.0f;
    private float damage = -1.0f;
    private static float ON_HIT_PUSH_POWER = 5.0F;
    private float STOP_TIME = 0.5f;
    private Animator anim;
    private Transform target;
    private PlayerCtrl playerCtrl;
    private Rigidbody2D rb2D;
    private ObjectPool<GameObject> originPool;
    private readonly int fireBallOffHash = Animator.StringToHash("FireBallOff");

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
        rb2D = GetComponent<Rigidbody2D>();
        isHited = false;
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        FollowingTarget(moveSpeed, scanningRadius);
    }

    // 초기화 및 풀 전달, 일종의 Init 역할
    public void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
        isHited = false;
        canAttack = true;
        currentHP = maxHP;
        if(canMove == false)
        {
            canMove = true;
        }
    }

    private void FollowingTarget(float moveSpeed, float scanningRadius)
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius && canMove == true)
            {
                // 플레이어에게 이동
                Vector2 newPosition = Vector2.MoveTowards(rb2D.position, target.position, moveSpeed * Time.fixedDeltaTime);
                rb2D.MovePosition(newPosition);
            }
            else if (Vector2.Distance(transform.position, target.position) > scanningRadius)
            {
                ReturnToPool();
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
                if(playerCtrl.readInvincible != true && canAttack == true)
                {
                    playerCtrl.ChangeHP(damage);
                    ReturnToPool();
                }
            }
            else if(other.gameObject.CompareTag("PlayerMelee"))
            {
                isHited = true;
                GetHit(-1);
            }
        }
    }

    private void ReturnToPool()
    {
        if(isPool == true)
        {
            return;
        }
        else
        {
            canMove = false;
            canAttack = false;
            anim.SetTrigger(fireBallOffHash);
        }
    }
    // FireBallOff 애니메이션 이벤트로 재생
    private void ReturnAfterAnim()
    {
        isPool = true;
        originPool.Release(gameObject);
    }

    // 플레이어 밀리에 타격 시
    private void GetHit(float value)
    {
        currentHP += value;

        if (currentHP <= 0)
        {
            ReturnToPool();
        }
        else
        {
            StartCoroutine(PushOnHit());
        }
    }

    IEnumerator PushOnHit()
    {
        canMove = false;
        Vector2 hitVector =  UtilityManager.utility.AllDirSet(transform.position - target.transform.position);

        // 타격 받은 방향으로 밀려남
        rb2D.AddForce(hitVector * ON_HIT_PUSH_POWER, ForceMode2D.Impulse);
        yield return new WaitForSeconds(STOP_TIME);
        rb2D.linearVelocity = Vector2.zero;
        canMove = true;
        isHited = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanningRadius);
    }
}
