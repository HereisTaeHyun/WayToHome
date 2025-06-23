using UnityEngine;
using UnityEngine.Pool;

public class SmallShockwave : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    public float lifeSpan = 1.5f;

    private void Awake()
    {
        costMana = 40.0f;
    }

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();

        moveSpeed = 5.0f;
        damage = PlayerCtrl.player.playerAttack.attackDamage / 2;
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
        if(other.TryGetComponent<IDamageable>(out var target))
        {
            target.ChangeHP(damage);
        } 
        else if(other.gameObject.CompareTag("Wall"))
        {
            ReturnToOriginPool();
        }
    }

    private void MoveMagic()
    {
        // 애니메이션 설정에 필요한 방향
        if (moveDir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDir.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        // 이동
        newVelocity.Set(moveDir.x * moveSpeed, rb2D.linearVelocity.y);
        rb2D.linearVelocity = newVelocity;
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        transform.position = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.position;
        transform.rotation = PlayerCtrl.player.playerAttack.magicSpawnPos.transform.rotation;

        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.lastMoveDir);
        
        lifeSpan = 1.5f;
        originPool = pool;
        isPool = false;
    }
}
