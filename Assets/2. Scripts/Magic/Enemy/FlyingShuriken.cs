using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class FlyingShuriken : MagicBase
{
    public MagicType magicType;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 6.0f;


    protected override void Start()
    {
        base.Start();

        damage = -10.0f;
        moveSpeed = 8.0f;
    }
    protected override void FixedUpdate()
    {
        if(isLaunch == true)
        {
            MoveMagic();
            lifeSpan -= Time.deltaTime;
        }
        if(lifeSpan <= 0)
        {
            ReturnToOriginPool();
        }
    }


    public override void SetPool(ObjectPool<GameObject> pool)
    {
    originPool = pool;
    isPool = false;
    isLaunch = false;
    lifeSpan = 3.0f;

    moveDir = Vector2.zero;
    newVelocity = Vector2.zero;

    transform.position = Vector3.zero;
    transform.rotation = Quaternion.identity;
    }

    public void Fire()
    {
        isLaunch = true;
    }

    private void MoveMagic()
    {
        if (GameManager.instance.readIsGameOver == false)
        {
            // 이동
            newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
            rb2D.linearVelocity = newVelocity;
        }
    }

    // Player에 닿으면 공격
    protected override void OnTriggerStay2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if(PlayerCtrl.player.readIsInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                }
            }
        }
    }

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
            anim.SetTrigger(magicOffHash);
        }
    }
    // FireMagicOff 애니메이션 이벤트로 재생
    private void ReturnAfterAnim()
    {
        UtilityManager.utility.ReturnToPool(originPool, gameObject);
        isPool = true;
        rb2D.simulated = true;
    }
}
