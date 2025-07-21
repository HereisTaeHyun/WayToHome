using UnityEngine;
using UnityEngine.Pool;

public enum MagicType
{
    FireBall,
    FireCannon,
    FireMissile,
    Meteor,
    ShockWave,
    BodyImpact,
    Poison,
}

public class MagicBase : MonoBehaviour

{
    protected bool isLaunch;
    protected bool isPool;
    protected float moveSpeed;
    protected float damage;
    protected Animator anim;
    protected Rigidbody2D rb2D;
    protected ObjectPool<GameObject> originPool;

    // moveSpeed, damage는 자식 객체에서 정할 것
    // isLaunch 플래그는 마법 타입에 따라 SetPool에서 결정할 것
    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    
    // 마법의 런치 트리거가 작동된다면 isLaunch를 트루로 하여 이동 시작
    // 대체로 마법 모션 증 생성하여 마법 모션이 완료될떄 트리거 방식을 채택할 듯
    protected virtual void FixedUpdate()
    {
        if(isLaunch == true)
        {
            FollowingTarget();
        }
    }

    
    // 사용자 클래스에서 초기화 및 풀 전달, 일종의 Init 역할도 겸함
    public virtual void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
    }

    // 플레이어가 있는 방향으로 설정된 속도만큼 이동
    protected virtual void FollowingTarget()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb2D.position, PlayerCtrl.player.transform.position, moveSpeed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if(PlayerCtrl.player.readIsInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                    ReturnToOriginPool();
                }
            }
            // 
            else if(other.gameObject.CompareTag("Ground"))
            {
                ReturnToOriginPool();
            }
        }
    }

    // 특정 조건 하에서 이미 Pool 이내인지 체크 후 아니라면 Pool에 넣기
    protected virtual void ReturnToOriginPool()
    {
        if(isPool == true)
        {
            return;
        }
        else
        {
            UtilityManager.utility.ReturnToPool(originPool, gameObject);
            isPool = true;
        }
    }
}
