using UnityEngine;
using UnityEngine.Pool;

public class Kunai : PlayerMagicBase
{
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 5.0f;

    private void Awake()
    {
        costMana = 20.0f;
    }

    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
        damage = PlayerCtrl.player.playerAttack.attackDamage;
    }

    private void FixedUpdate()
    {
        MoveMagic();

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

    private void MoveMagic()
    {
        newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        rb2D.linearVelocity = newVelocity;
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        transform.position = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.position;
        transform.rotation = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.rotation;

        moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.aimPos);
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        lifeSpan = 5.0f;
        originPool = pool;
        isPool = false;
    }
}
