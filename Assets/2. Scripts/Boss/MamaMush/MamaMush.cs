using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MamaMush : BossCtrl
{
    private float rageHP;
    [SerializeField] AudioClip rageSFX;
    private Coroutine blinkRoutine;

    private ObjectPool<GameObject> bodyImpactPool;
    private ObjectPool<GameObject> poisonPool;
    private ObjectPool<GameObject> poisonRainPool;

    [SerializeField] private List<MagicType> usingMagic;
    [SerializeField] private List<MagicType> phase1UsingMagic;
    [SerializeField] private List<MagicType> phase2UsingMagic;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int magicCountInPool = 10;

    private int poisongRainCountInPool = 20;
    private float poisonRainSpawnDuration = 5.0f;
    private float minPoisonRainInterval = 0.05f;
    private float maxPoisonRainInterval = 0.25f;
    [SerializeField] private ParticleSystem poisonRainParticle;


    // 위치 저장 셋
    [SerializeField] private Transform bodyImpactSpawnPos;
    [SerializeField] private Transform poisonSpawnPos;
    [SerializeField] private GameObject poisonRainSpawnArea;
    private BoxCollider2D poisonSpawnAreaCollider;
    private Bounds poisonSpawnAreaBounds;

    // 마법 개별 컴포넌트
    private BodyImpact bodyImpactComp;
    private Poison poisonComp;
    private PoisonRain poisonRainComp;

    // 마법 사운드
    [SerializeField] private AudioClip bodyimpactSFX;

    private float jumpSpeed = 10.0f;
    private bool isGround;
    private bool isMove;
    private Vector2 newVelocity;
    private Vector2 moveDir;
    private readonly int moveOnHash = Animator.StringToHash("OnMove");
    private readonly int dieHash = Animator.StringToHash("Die");

    protected override void Init()
    {
        base.Init();
    }

    void Start()
    {
        Init();

        usingMagic = phase1UsingMagic;

        // 마법 풀 생성
        // 인덱스 번호는 위 마법 위치 딕셔너리와 같은 순서
        UtilityManager.utility.CreatePool(ref bodyImpactPool, magicList[0], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref poisonPool, magicList[1], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref poisonRainPool, magicList[2], poisongRainCountInPool, poisongRainCountInPool);

        rageHP = maxHP * 0.6f;

        isGround = true;
        coolTime = 5.0f;
        poisonRainParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        poisonSpawnAreaCollider = poisonRainSpawnArea.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // 보스 또는 플레이어가 사망이면 return
        if (GameManager.instance.readIsGameOver == true || isDie == true)
        {
            return;
        }

        if (canMove)
        {
            FollowingTarget(moveSpeed);
        }

        if (canAttack == true && SeeingPlayer())
        {
            // 공격 후 다음 공격까지 휴식
            UseRandomMagic();
            StartCoroutine(CoolTimeCheck());
        }
    }

    // 플레이어가 보스룸에 진입시 할 행동
    public override void PlayerEntered()
    {
        StartCoroutine(EnterRoutine());
    }

    protected IEnumerator EnterRoutine()
    {
        canAttack = false;
        isMove = true;
        anim.SetBool(moveOnHash, isMove);
        anim.SetFloat(dirHash, moveDir.x);

        while (Mathf.Abs(PlayerCtrl.player.transform.position.x - transform.position.x) > 10.0f)
        {
            newVelocity.Set(moveDir.x * moveSpeed, rb2D.linearVelocity.y);
            rb2D.linearVelocity = newVelocity;
            yield return null;
        }

        PlayerCtrl.player.playerMove.ForceIdle();
        PlayerCtrl.player.canMove = false;

        isMove = false;
        canMove = false;
        rb2D.linearVelocity = Vector2.zero;
        anim.SetBool(moveOnHash, isMove);

        yield return new WaitForSeconds(2.0f);

        // 얘가 개막 패턴으로 적합함
        StartCoroutine(UseBodyImpact());
        StartCoroutine(CoolTimeCheck());

        PlayerCtrl.player.canMove = true;
        canMove = true;
    }

    // 이동 및 점프 처리
    // moveDir이 음수면 왼쪽 양수면 오른쪽
    private void FollowingTarget(float moveSpeed)
    {
        // 타겟이 존재하고 살아 있을 경우 움직임
        if (GameManager.instance.readIsGameOver == false && isDie == false)
        {
            // 플레이어가 존 내부면 moveSpeed만큼씩 이동 시작
            if (SeeingPlayer())
            {
                // 움직이는 방향 벡터 받아 오기
                moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);

                if (Math.Abs(PlayerCtrl.player.transform.position.x - transform.position.x) >= 0.15)
                {
                    // 움직임 적용
                    isMove = true;

                    anim.SetBool(moveOnHash, isMove);
                    anim.SetFloat(dirHash, moveDir.x);

                    newVelocity.Set(moveDir.x * moveSpeed, rb2D.linearVelocity.y);
                    rb2D.linearVelocity = newVelocity;
                }
                else
                {
                    isMove = false;
                    anim.SetBool(moveOnHash, isMove);

                    newVelocity.Set(0f, rb2D.linearVelocity.y);
                    rb2D.linearVelocity = newVelocity;
                }
            }
        }
        // 플레이어가 시야에 없으면 움직일 필요 없음
        else
        {
            isMove = false;
            anim.SetBool(moveOnHash, isMove);
        }
    }

    // HP 변경 처리
    public override void ChangeHP(float value)
    {
        currentHP = Mathf.Clamp(currentHP + value, 0, maxHP);

        // 타격 벡터 계산 및 sfx, anim 재생
        Vector2 hitVector = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        UtilityManager.utility.PlaySFX(enemyGetHitSFX);

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }
        blinkRoutine = StartCoroutine(UtilityManager.utility.BlinkOnDamage(spriteRenderer, blinkTime));

        if (currentHP % 100 == 0)
        {
            anim.SetTrigger(hitTrigger);
            anim.SetFloat(hitHash, hitVector.x);
        }

        if (isRage == false && currentHP <= rageHP)
        {
            GetRage();
        }

        // 체력 0 이하면 사망처리
        if (currentHP <= 0)
        {
            EnemyDie();
        }
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
        DataManager.dataManager.playerData.diedEnemy.Add(enemyID);

        bossRoomSensor.SetBossClear();
        UtilityManager.utility.PlaySFX(enemyDieSFX);

        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    #region 패턴 관련
    // 분노
    private void GetRage()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine); 
        }
        isRage = true;
        UtilityManager.utility.PlaySFX(rageSFX);
        usingMagic = phase2UsingMagic;
        moveSpeed = 5.0f;
        spriteRenderer.color = new Color32(255, 140, 140, 255);
    }

    // 마법 공격 후 일정 시간 동안 쿨타임 처리
    private IEnumerator CoolTimeCheck()
    {
        canAttack = false;
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }
    
    // 마법을 선택 후 스위칭하여 마법 함수 실행
    private void UseRandomMagic()
    {
        int magicIdx = Random.Range(0, usingMagic.Count);
        MagicType currentMagic = usingMagic[magicIdx];

        if (isRage == false)
        {
            switch (currentMagic)
            {
                case MagicType.BodyImpact:
                    StartCoroutine(UseBodyImpact());
                    break;
                case MagicType.Poison:
                    StartCoroutine(UsePoison(3));
                    break;
            }
        }
        else if (isRage == true)
        {
            switch (currentMagic)
            {
                case MagicType.BodyImpact:
                    StartCoroutine(UseBodyImpact(3));
                    break;
                case MagicType.Poison:
                    StartCoroutine(UsePoison(9));
                    break;
                case MagicType.PoisonRain:
                    StartCoroutine(UsePoisonRain());
                    break;
            }
        }
    }

    // BodyImpact 마법을 스폰 위치에 따라 생성 및 초기화
    private IEnumerator UseBodyImpact(int repeat = 1)
    {
        for (int i = 0; i < repeat; i++)
        {
            isGround = false;
            Jump();

            yield return new WaitUntil(() => isGround);
            SpawnBodyImpact();

            canMove = false;
            rb2D.linearVelocity = Vector2.zero;
            isMove = false;
            anim.SetBool(moveOnHash, isMove);
            yield return new WaitForSeconds(1.0f);
            canMove = true;
        }
    }

    private void SpawnBodyImpact()
    {
        GameObject bodyImpact = UtilityManager.utility.GetFromPool(bodyImpactPool, magicCountInPool);
        if (bodyImpact == null)
        {
            return;
        }

        bodyImpactComp = bodyImpact.GetComponent<BodyImpact>();
        bodyImpact.transform.position = bodyImpactSpawnPos.transform.position;
        bodyImpact.transform.rotation = bodyImpactSpawnPos.transform.rotation;
        UtilityManager.utility.PlaySFX(bodyimpactSFX);

        bodyImpactComp.SetPool(bodyImpactPool);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = true;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = false;
        }
    }

    private void Jump()
    {
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpSpeed);
    }

    // Poison 마법을 스폰 위치에 따라 생성 및 초기화
    private IEnumerator UsePoison(int repeat = 3, float interval = 0.2f)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);

        for (int i = 0; i < repeat; i++)
        {
            GameObject poison = UtilityManager.utility.GetFromPool(poisonPool, magicCountInPool);
            if (poison != null)
            {
                poisonComp = poison.GetComponent<Poison>();
                poison.transform.position = poisonSpawnPos.transform.position;
                poison.transform.rotation = poisonSpawnPos.transform.rotation;
                poisonComp.SetPool(poisonPool);
            }
            if (i < repeat - 1)
            {
                yield return wait;
            }
        }
    }

    // PoisonRain 마법을 스폰 위치에 따라 생성 및 초기화
    private IEnumerator UsePoisonRain()
    {
        poisonRainParticle.Clear(true);
        poisonRainParticle.Play(true);

        yield return new WaitForSeconds(3.0f);

        poisonRainParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitUntil(() => poisonRainParticle.IsAlive(true) == false);

        float time = 0.0f;

        while (time < poisonRainSpawnDuration)
        {
            float wait = Random.Range(minPoisonRainInterval, maxPoisonRainInterval);
            yield return new WaitForSeconds(wait);
            time += wait;

            GameObject poisonRain = UtilityManager.utility.GetFromPool(poisonRainPool, poisongRainCountInPool);
            if (poisonRain != null)
            {
                poisonRainComp = poisonRain.GetComponent<PoisonRain>();
                poisonRain.transform.position = SetRandomPos();
                poisonRainComp.SetPool(poisonRainPool);
            }
        } 
    }

    // PoisonRain 마법의 위치를 무작위로 생성
    private Vector2 SetRandomPos()
    {
        poisonSpawnAreaBounds = poisonSpawnAreaCollider.bounds;

        float x = Random.Range(poisonSpawnAreaBounds.min.x, poisonSpawnAreaBounds.max.x);
        float y = poisonSpawnAreaBounds.center.y;

        return new Vector2(x, y);
    }

    #endregion
}
