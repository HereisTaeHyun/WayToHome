using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class OrbitingKunai : MagicBase
{
    public MagicType magicType;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 3.0f;

    private float orbitRadius = 1.2f;
    private float orbitSpeed = 180f;
    private float orbitHeight = 1.0f;
    private float aimTime = 3.0f;
    private Vector2 orbitCenter;

    protected override void Start()
    {
        base.Start();

        damage = -10.0f;
        moveSpeed = 3.0f;
    }
    protected override void FixedUpdate()
    {
        orbitCenter = PlayerCtrl.player.transform.position + Vector3.up * orbitHeight;
        transform.RotateAround(orbitCenter, Vector3.forward, orbitSpeed * Time.deltaTime);

        aimTime -= Time.deltaTime;

        // 이동 로직 및 바라봄 축 설정
        if (isLaunch == true)
        {
            MoveMagic();
            lifeSpan -= Time.deltaTime;
        }
        
        if (lifeSpan <= 0)
        {
            ReturnToOriginPool();
        }
    }

        // aimTime 동안 플레이어 바라보다 발사되는 로직
    private IEnumerator Aim()
    {
        while (aimTime >= 0)
        {
            // 바라봄 축 설정
            orbitCenter = PlayerCtrl.player.transform.position + Vector3.up * orbitHeight;
            transform.RotateAround(orbitCenter, Vector3.forward, orbitSpeed * Time.deltaTime);
            aimTime -= Time.deltaTime;
            yield return null;
        }
        isLaunch = true;
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
        aimTime = 3.0f;
        lifeSpan = 3.0f;

        moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.transform.position - transform.position);
        orbitCenter = PlayerCtrl.player.transform.position + Vector3.up * orbitHeight;
        transform.position = orbitCenter + Vector2.right * orbitRadius;

        StartCoroutine(Aim());
    }

    private void MoveMagic()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 이동
            newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
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
                if(PlayerCtrl.player.readIsInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                    ReturnToOriginPool();
                }
            }
            // 
            else if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground"))
            {
                ReturnToOriginPool();
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
