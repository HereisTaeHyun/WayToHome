using UnityEngine;

public class Meteor : MagicBase
{
    public MagicType magicType;

    private MeteorOff meteorOff;
    private Vector2 newVelocity;
    private SpriteRenderer spriteRenderer;
    private Color color;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
        damage = -30.0f;

        meteorOff = GetComponentInChildren<MeteorOff>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    protected override void FixedUpdate()
    {
        MoveMagic();
    }

    private void MoveMagic()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 이동
            newVelocity.Set(Vector2.down.x * moveSpeed, Vector2.down.y * moveSpeed);
            rb2D.linearVelocity = newVelocity;
        }
    }

    // Player, Wall, Ground 등에 닿으면 풀 리턴
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
            else if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground"))
            {
                ReturnToOriginPool();
            }
        }
    }

    // 풀로 되돌리기
    protected override void ReturnToOriginPool()
    {
        if(isPool == true)
        {
            return;
        }
        else
        {
            isPool = true;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.simulated = false;

            color.a = 0.0f;
            spriteRenderer.color = color;

            meteorOff.Explode();
        }
    }
    // FireMagicOff 애니메이션 이벤트로 재생
    public void ReturnAfterAnim()
    {
        UtilityManager.utility.ReturnToPool(originPool, gameObject);
        isPool = true;
        rb2D.simulated = true;

        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
