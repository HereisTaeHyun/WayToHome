using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class MiddleBossCtrl : EnemyCtrl
{
    // public 변수
    // private 변수
    private SpriteRenderer spriteRenderer;
    private bool ableBlink;
    private static float BLINK_TIME = 0.1f;
    private float distance;
    [SerializeField] private Transform warpPointSet;
    private List<Transform> warpPoints = new List<Transform>();
    private MiddleBossMeleeAttack middleBossMeleeAttack;
    private Transform magicSpawnPos;
    private ObjectPool<GameObject> magicPool;
    private int maxMagic = 3;
    private FireBall fireBallComp;
    private bool isStun;
    private int maxMagicResist = 3;
    private int getHitbyMagic; // 일정 이상 마법에 타격시 isStun
    [SerializeField] float melleAttackRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AudioClip warpSfx;
    [SerializeField] private GameObject fireBall;
    private readonly int dieHash = Animator.StringToHash("Die");

    private bool canAttack;
    private float coolTime = 2.0f;

    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    void Start()
    {
        Init();
        spriteRenderer =GetComponent<SpriteRenderer>();

        foreach(Transform warpPoint in warpPointSet)
        {
            warpPoints.Add(warpPoint);
        }
        warpPoints.ToArray();

        // 공격 관련 초기화
        middleBossMeleeAttack = GetComponent<MiddleBossMeleeAttack>();
        magicSpawnPos = transform.Find("MagicSpawnPos");
        UtilityManager.utility.CreatePool(ref magicPool, fireBall, maxMagic, maxMagic);

        canAttack = true;
        ableBlink = true;
    }

    void Update()
    {
        // 게임 오버가 아닐 경우 행동
        if(GameManager.instance.readIsGameOver == true || isStun == true)
        {
            return;
        }

        // distance에 따라 행동 분리
        distance = Vector2.Distance(target.position, transform.position);

        if(distance > scanningRadius)
        {
            Warp();
        }
        if(distance <= scanningRadius)
        {
            // radious 내부라면 바라보기 + 공격
            Vector2 moveDir = UtilityManager.utility.DirSet(target.transform.position - transform.position);
            anim.SetFloat(moveDirHash, moveDir.x);

            // 사거리 내부면 근접 공격, 외부면 마법 공격
            if(distance < melleAttackRange && canAttack == true)
            {
                MeleeAttackAble(moveDir);
                // StartCoroutine(CoolTimeCheck());
            }
            else if(distance > melleAttackRange && canAttack == true)
            {
                UseFireBall();
                StartCoroutine(CoolTimeCheck());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("FireBall"))
        {
            fireBallComp = collision.gameObject.GetComponent<FireBall>();
            if(fireBallComp.isHited == true)
            {
                getHitbyMagic += 1;
                if(getHitbyMagic == maxMagicResist)
                {
                    StartCoroutine(MagicStunTimer());
                }
            }
        }
    }

    IEnumerator MagicStunTimer()
    {
        isStun = true;
        yield return new WaitForSeconds(stunTime);
        isStun = false;
        getHitbyMagic = 0;
    }

    private void Warp()
    {
        // 이동해야 하는 포인트 초기화
        Transform pointAbleAttack = null;

        // 이동 가능한 포인트 순회
        foreach(Transform point in warpPoints)
        {
            // 포인트가 타겟을 공격 가능한 거리에 있게 하면 변수에 할당
            float currentChecking = Vector2.Distance(target.position, point.position);
            if(currentChecking < scanningRadius)
            {
                pointAbleAttack = point;
            }
        }

        // 할당된 변수로 이동
        if(pointAbleAttack != null)
        {
            UtilityManager.utility.PlaySFX(warpSfx);
            transform.position = pointAbleAttack.position;
        }
    }

    // 마법 공격
    private void UseFireBall()
    {
        // 풀 오브젝트 가져오기
        GameObject fireBall = UtilityManager.utility.GetFromPool(magicPool, maxMagic);

        if(fireBall != null)
        { 
            fireBallComp = fireBall.GetComponent<FireBall>();

            // fireBall에개 돌아와야 하는 풀 전달하기, 초기화
            fireBallComp.SetPool(magicPool);

            // 파이어볼 셋업
            fireBall.transform.position = magicSpawnPos.position;
            fireBall.transform.rotation = magicSpawnPos.rotation;
        }
    }

    // 근접 공격격
    private void MeleeAttackAble(Vector2 moveDir)
    {
        middleBossMeleeAttack.Attack();
        anim.SetFloat(moveDirHash, moveDir.x);
    }

    private IEnumerator CoolTimeCheck()
    {
        canAttack = false;
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }

    // HP 변경 처리
    public override void ChangeHP(float value)
    {
        if(ableBlink == true)
        {
            StartCoroutine(BlinkOnDamage());
        }
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);

        // 타격 sfx 재생
        UtilityManager.utility.PlaySFX(enemyGetHitSFX);

        // 체력 0 이하면 사망처리
        if (currentHP <= 0)
        {
            EnemyDie();
        }
    }

    // 무적 시간 동안 깜빡거리기 코루틴
    IEnumerator BlinkOnDamage()
    {
        ableBlink = false;
        bool isBlink = false;
        Color color = spriteRenderer.color;

        float maxBlinkTime = 1.0f; // 깜빡이는 총 시간
        float currentBlinkTIme = 0.0f;

        // 데미지를 입으면 깜빡임
        while(currentBlinkTIme < maxBlinkTime)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if(isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if(isBlink == false)
            {
                color.a = 1.0f;
                spriteRenderer.color = color;
                isBlink = true;
            }
            currentBlinkTIme += BLINK_TIME;
            yield return new WaitForSeconds(BLINK_TIME);
        }
        // 기본 상태로 초기화
        ableBlink = true;
        color.a = 1.0f;
        spriteRenderer.color = color;
    }

    // 사망 처리
    protected override void EnemyDie()
    {
        StartCoroutine(DieStart());
    }
    // 사망 절차 진행, 사운드 재생 및 물리 영향 제거 후 사망 애니메이션 재생
    private IEnumerator DieStart()
    {
        isDie = true;
        UtilityManager.utility.PlaySFX(enemyDieSFX);
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
