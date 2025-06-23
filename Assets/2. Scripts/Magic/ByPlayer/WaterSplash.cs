using UnityEngine;
using UnityEngine.Pool;


public class WaterSplash : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private Vector2 moveDir;
    private Collider2D coll2D;

    private void Awake()
    {
        costMana = 20.0f;
    }

    protected override void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        coll2D.enabled = false;
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
        Vector3 spawnPos = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.position;
        if(PlayerCtrl.player.lastMoveDir.x == 1)
        {
            spawnPos.x += 1.0f;
        }
        else if(PlayerCtrl.player.lastMoveDir.x == -1)
        {
            spawnPos.x -= 1.0f;
        }

        transform.position = spawnPos;
        transform.rotation = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.rotation;

        if (coll2D == null)
        {
            coll2D = GetComponentInChildren<Collider2D>();
        }
        DisableAttackCollider();

        originPool = pool;
        isPool = false;
    }

    // 애니메이션 이벤트로 제어
    public void EnableAttackCollider()
    {
        coll2D.enabled = true;
    }

    public void DisableAttackCollider()
    {
        coll2D.enabled = false;
    }
}
