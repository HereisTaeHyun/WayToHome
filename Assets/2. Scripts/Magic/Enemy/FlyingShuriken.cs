using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class FlyingShuriken : MagicBase
{
    public MagicType magicType;
    private float lifeSpan = 6.0f;
    private Vector2 startPos;
    private Vector2 targetPos;
    private float orbitRadius;
    private float orbitSpeed = 180.0f;
    private Vector2 orbitCenter;
    private bool isOrbit;
    private float orbitAngle;


    protected override void Start()
    {
        base.Start();

        damage = -10.0f;
        moveSpeed = 10.0f;
    }
    protected override void FixedUpdate()
    {

        MoveMagic();
        lifeSpan -= Time.deltaTime;
        
        if (lifeSpan <= 0)
        {
            ReturnToOriginPool();
        }
    }


    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
    }

    // startPos를 줘야 하기에 오버로딩 필요
    public void SetPool(ObjectPool<GameObject> pool, Vector2 castingPos)
    {
        SetPool(pool);

        lifeSpan = 6.0f;
        isOrbit = false;
        startPos = castingPos;

        orbitCenter = (startPos + targetPos) * 0.5f;
        orbitRadius = Vector2.Distance(startPos, orbitCenter);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void MoveMagic()
    {
        if (GameManager.instance.readIsGameOver == false && isOrbit == false)
        {
            // 이동
            isOrbit = true;
            StartCoroutine(OrbitMove());
        }
    }
    
    private IEnumerator OrbitMove()
    {
        while (isOrbit)
        {
        // 회전 궤도 설정
        orbitAngle += orbitSpeed * Time.deltaTime;

        Vector2 offset = new Vector2(
            Mathf.Cos(orbitAngle * Mathf.Deg2Rad),
            Mathf.Sin(orbitAngle * Mathf.Deg2Rad)
        ) * orbitRadius;
        transform.position = orbitCenter + offset;

        if (Vector2.Distance(transform.position, startPos) < 0.05f)
        {
            ReturnToOriginPool();
            yield break;
        }

        yield return null;
        }
    }

    // Player에 닿으면 공격
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.instance.readIsGameOver == false)
        {
            if (other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if (PlayerCtrl.player.readIsInvincible != true)
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
