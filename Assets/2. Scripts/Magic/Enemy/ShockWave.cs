using UnityEngine;
using UnityEngine.Pool;

public class ShockWave : MagicBase
{
    public MagicType magicType;
    
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();

        moveSpeed = 4.0f;
        damage = -10.0f;
        anim.SetFloat(moveDirHash, moveDir.x);
    }

    protected override void FixedUpdate()
    {
        MoveMagic();
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
        
        // 음직임 방향 설정
        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
    }

    private void MoveMagic()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 애니메이션 설정에 필요한 방향
            if(moveDir.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if(moveDir.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            // 이동
            anim.SetFloat(moveDirHash, moveDir.x);
            newVelocity.Set(moveDir.x * moveSpeed, rb2D.linearVelocity.y);
            rb2D.linearVelocity = newVelocity;
        }
    }

    // Player, Wall\ 등에 닿으면 풀 리턴
    protected override void OnTriggerStay2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if(PlayerCtrl.player.readInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                }
            }
            // 
            else if(other.gameObject.CompareTag("Wall"))
            {
                ReturnToOriginPool();
            }
        }
    }
}
