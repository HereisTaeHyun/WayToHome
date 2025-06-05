using UnityEngine;
using UnityEngine.Pool;

public enum PlayerMagicType
{
    Kunai,
    Shuriken,
}
public class PlayerMagicBase : MonoBehaviour
{
    public PlayerMagicType Id { get; }
    protected bool isPool;
    protected float moveSpeed;
    protected float damage;
    protected Animator anim;
    protected Rigidbody2D rb2D;
    protected ObjectPool<GameObject> originPool;

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

    protected virtual void OnTriggerStay2D(Collider2D other)
    {    
        if(other.TryGetComponent<IDamageable>(out var target))
        {
            target.ChangeHP(damage);
        } 
        else if(other.gameObject.CompareTag("Ground"))
        {
            ReturnToOriginPool();
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
