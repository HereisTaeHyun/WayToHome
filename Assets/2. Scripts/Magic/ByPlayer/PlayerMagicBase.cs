using UnityEngine;
using UnityEngine.Pool;

public enum PlayerMagicType
{
    Kunai,
    Shuriken,
    SmallShockwave,
}
public class PlayerMagicBase : MonoBehaviour
{
    protected bool isPool;
    protected float moveSpeed;
    protected float damage;
    protected Animator anim;
    protected Rigidbody2D rb2D;
    protected ObjectPool<GameObject> originPool;
    private readonly int magicOffHash = Animator.StringToHash("MagicOff");

    // moveSpeed, damage는 자식 객체에서 선언할 것
    // isLaunch 플래그는 마법 타입에 따라 SetPool에서 결정할 것
    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // 사용자 클래스에서 초기화 및 풀 전달, 일종의 Init 역할도 겸함
    public virtual void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.ChangeHP(damage);
        }
    }

    // 특정 조건 하에서 이미 Pool 이내인지 체크 후 아니라면 Pool에 넣기
    protected virtual void ReturnToOriginPool()
    {
        if (isPool == true)
        {
            return;
        }
        else
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.simulated = false;
            anim.SetTrigger(magicOffHash);
        }
    }
    
    // MagicOff 애니메이션 이벤트로 재생
    private void ReturnAfterAnim()
    {
        UtilityManager.utility.ReturnToPool(originPool, gameObject);
        rb2D.simulated = true;
        isPool = true;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
