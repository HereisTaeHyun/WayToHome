using UnityEngine;
using UnityEngine.Pool;

public class Shuriken : PlayerMagicBase
{
    private Vector2 moveDir;
    private float lifeSpan = 5.0f;

    private void Awake()
    {
        costMana = 30.0f;
    }

    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
        damage = PlayerCtrl.player.playerAttack.attackDamage - 1;
    }

    private void FixedUpdate()
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0)
        {
            ReturnToOriginPool();
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.ChangeHP(damage);
            ReturnToOriginPool();
        }
        else if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Wall"))
        {
            ReturnToOriginPool();
        }
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        if (rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        transform.position = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.position;
        transform.rotation = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.rotation;

        moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.aimPos);
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        lifeSpan = 5.0f;
        originPool = pool;
        isPool = false;

        rb2D.linearVelocity = gameObject.transform.right * 10.0f;
    }
}
