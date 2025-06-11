using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Pool;


public class WaterSplash : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private Vector2 moveDir;
    private Collider2D coll2D;

    protected override void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        coll2D.enabled = false;

        moveSpeed = 5.0f;
        damage = -1.0f;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.ChangeHP(damage);
        }
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        if (coll2D == null)
        {
            coll2D = GetComponentInChildren<Collider2D>();
        }
        coll2D.enabled = false;

        originPool = pool;
        isPool = false;
    }

    // 애니메이션 이벤트로 제어
    public void EnableAttackCollider()
    {
        coll2D.enabled = true;
    }
}
