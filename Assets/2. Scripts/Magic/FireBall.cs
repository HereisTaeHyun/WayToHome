using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class FireBall : MagicBase
{
    // public 변수
    [NonSerialized] public bool isHited;
    public MagicType magicType;

    // private 변수
    private float maxHP = 2.0f;
    private float currentHP;
    private bool canAttack;
    private float scanningRadius = 10.0f;
    private static float ON_HIT_PUSH_POWER = 5.0F;
    private float STOP_TIME = 0.5f;
    private new ObjectPool<GameObject> originPool;
    private readonly int fireBallOffHash = Animator.StringToHash("FireBallOff");

    protected override void Start()
    {
        base.Start();

        moveSpeed = 3.0f;
        damage = -1.0f;
    }

    protected override void FixedUpdate()
    {
        if(isLaunch == false)
        {
            return;
        }
        FollowingTarget();
    }

    // 사용자 클래스에서 초기화 및 풀 전달, 일종의 Init 역할도 겸함
    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
        isHited = false;
        canAttack = true;
        currentHP = maxHP;
        if(isLaunch == false)
        {
            isLaunch = true;
        }
    }

    protected override void FollowingTarget()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, PlayerCtrl.player.transform.position) < scanningRadius)
            {
                // 플레이어에게 이동
                Vector2 newPosition = Vector2.MoveTowards(rb2D.position, PlayerCtrl.player.transform.position, moveSpeed * Time.deltaTime);
                rb2D.MovePosition(newPosition);
            }
            else if (Vector2.Distance(transform.position, PlayerCtrl.player.transform.position) > scanningRadius)
            {
                ReturnToOriginPool();
            }
        }
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if(PlayerCtrl.player.readInvincible != true && canAttack == true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                    ReturnToOriginPool();
                }
            }
            // 플레이어의 공격에 맞은 경우
            else if(other.gameObject.CompareTag("PlayerMelee") && isHited == false)
            {
                isHited = true;
                GetHit(-1);
            }
            // 플레이어에게 맞고 튕겨져 나간 파이어볼이 적과 부딪힘
            else if(other.gameObject.CompareTag("Enemy") && isHited == true)
            {
                GetHit(-2);
            }
        }
    }

    protected override void ReturnToOriginPool()
    {
        if(isPool == true)
        {
            return;
        }
        else
        {
            StopAllCoroutines();
            rb2D.linearVelocity = Vector2.zero;
            isLaunch = false;
            canAttack = false;
            anim.SetTrigger(fireBallOffHash);
        }
    }
    // FireBallOff 애니메이션 이벤트로 재생
    private void ReturnAfterAnim()
    {
        UtilityManager.utility.ReturnToPool(originPool, gameObject);
        isPool = true;
    }

    // 플레이어 밀리에 타격 시
    private void GetHit(float value)
    {
        currentHP += value;
        if (currentHP <= 0)
        {
            ReturnToOriginPool();
        }
        else
        {
            StartCoroutine(PushOnHit());
        }
    }

    IEnumerator PushOnHit()
    {
        isLaunch = false;
        Vector2 hitVector =  UtilityManager.utility.AllDirSet(transform.position - PlayerCtrl.player.transform.transform.position);

        // 타격 받은 방향으로 밀려남
        rb2D.AddForce(hitVector * ON_HIT_PUSH_POWER, ForceMode2D.Impulse);
        yield return new WaitForSeconds(STOP_TIME);
        rb2D.linearVelocity = Vector2.zero;
        isLaunch = true;
        isHited = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanningRadius);
    }
}
