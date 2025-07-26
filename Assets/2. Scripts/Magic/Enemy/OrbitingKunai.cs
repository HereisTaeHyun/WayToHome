using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class OrbitingKunai : MagicBase
{
    public MagicType magicType;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 3.0f;

    private float orbitRadius = 5.0f;
    private float orbitSpeed = 180.0f;
    private Vector2 orbitCenter;
    private bool isOrbit;

    protected override void Start()
    {
        base.Start();

        damage = -10.0f;
        moveSpeed = 10.0f;
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

    // aimTime 동안 플레이어 바라보다 발사되는 로직
    private IEnumerator Aim()
    {
        while (isOrbit)
        {
            // 바라봄 축 설정
            moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.transform.position - transform.position);
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 회전할 궤도 설정
            orbitCenter = PlayerCtrl.player.transform.position + Vector3.up;
            transform.RotateAround(orbitCenter, Vector3.forward, orbitSpeed * Time.deltaTime);
            yield return null;
        }

        isLaunch = true;
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
        lifeSpan = 3.0f;

        isLaunch = false;
        isOrbit = true;

        orbitCenter = PlayerCtrl.player.transform.position + Vector3.up;
        transform.position = orbitCenter + Vector2.right * orbitRadius;

        StartCoroutine(Aim());
    }

    public void Fire()
    {
        isLaunch = true;
        isOrbit = false;
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
