using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MagicShopNPC_Boss : BossCtrl
{
    private float rageHP;
    [SerializeField] AudioClip rageSFX;
    private Coroutine blinkRoutine;

    private ObjectPool<GameObject> orbitingKunaiPool;
    private ObjectPool<GameObject> flyingShurikenPool;

    [SerializeField] private List<MagicType> usingMagic;
    [SerializeField] private List<MagicType> phase1UsingMagic;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int magicCountInPool = 50;

    // 위치 저장 셋
    [SerializeField] private Transform flyingShurikenSpawnPos;

    [SerializeField] private Transform warpPointSet;
    private float warpChargingDistance = 8.0f;
    private float warpChargingTime = 10.0f;
    private List<Transform> warpPoints = new List<Transform>();
    private float distanceToPlayer;
    private bool isWarpCharging;
    private int lastWarpIdx;

    private static float MAGIC_WAIT_TIME = 0.5f; // 마법 사용과 애니메이션간 타이밍 맞추기에 사용
    private WaitForSeconds waitMagic;

    // 마법 개별 컴포넌트
    private OrbitingKunai orbitingKunaiComp;
    private FlyingShuriken flyingShurikenComp;

    // 오디오 관련
    [SerializeField] private AudioClip warpSFX;

    private Vector2 moveDir;
    private readonly int dieHash = Animator.StringToHash("Die");
    private readonly int attackHash = Animator.StringToHash("Attack");

    protected override void Init()
    {
        base.Init();
    }

    void Start()
    {
        Init();

        usingMagic = phase1UsingMagic;

        foreach (Transform warpPoint in warpPointSet)
        {
            warpPoints.Add(warpPoint);
        }

        // 마법 풀 생성
        UtilityManager.utility.CreatePool(ref orbitingKunaiPool, magicList[0], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref flyingShurikenPool, magicList[1], magicCountInPool, magicCountInPool);

        rageHP = maxHP * 0.6f;

        coolTime = 8.0f;
        waitMagic = new WaitForSeconds(MAGIC_WAIT_TIME);

        if (DataManager.dataManager.playerData.diedEnemy.Contains(enemyID))
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);

        // 보스 또는 플레이어가 사망이면 return
        if (GameManager.instance.readIsGameOver == true || isDie == true)
        {
            return;
        }

        if (canMove)
        {
            anim.SetFloat(dirHash, moveDir.x);

            // 플레이어가 일정 거리 이내면 워프 준비
            distanceToPlayer = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);
            if (distanceToPlayer <= warpChargingDistance && isWarpCharging == false)
            {
                StartCoroutine(ChargingWarp());
            }
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

        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        anim.SetFloat(dirHash, moveDir.x);

        PlayerCtrl.player.playerMove.ForceIdle();
        PlayerCtrl.player.canMove = false;

        canMove = false;
        rb2D.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(2.0f);

        PlayerCtrl.player.canMove = true;
        canMove = true;
        canAttack = true;
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

        if (currentHP != 0 && currentHP % 100 == 0)
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

    #region Movement & Position
    private IEnumerator ChargingWarp()
    {
        isWarpCharging = true;
        float time = 0f;

        while (time < warpChargingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        UseWarp();
    }

    private void UseWarp()
    {
        isWarpCharging = false;

        // 이동해야 하는 포인트 초기화
        int warpPointIdx;
        do
        {
            warpPointIdx = Random.Range(0, warpPoints.Count);
        }
        while (warpPointIdx == lastWarpIdx);
        lastWarpIdx = warpPointIdx;

        // 워프
        Transform pointToWarp = warpPoints[warpPointIdx];
        transform.position = pointToWarp.position;
        UtilityManager.utility.PlaySFX(warpSFX);
    }
    #endregion

    #region Pattern
    // 분노
    private void GetRage()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }
        isRage = true;
        UtilityManager.utility.PlaySFX(rageSFX);

        coolTime = 4.0f;

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
                case MagicType.OrbitingKunai:
                    StartCoroutine(UseOrbitingKunai());
                    anim.SetTrigger(attackHash);
                    break;
                case MagicType.FlyingShuriken:
                    StartCoroutine(UseFlyingShuriken());
                    anim.SetTrigger(attackHash);
                    break;
            }
        }
        else if (isRage == true)
        {
                switch (currentMagic)
            {
                case MagicType.OrbitingKunai:
                    StartCoroutine(UseOrbitingKunai());
                    anim.SetTrigger(attackHash);
                    break;
                case MagicType.FlyingShuriken:
                    StartCoroutine(UseFlyingShuriken(3));
                    anim.SetTrigger(attackHash);
                    break;
            }
        }
    }

    // OrbitingKunai 배치, 얘는 위치 셋업이 마법 쪽에서 이루어지니 소환까지만
    private IEnumerator UseOrbitingKunai(int repeat = 10, float interval = 0.2f)
    {
        canMove = false;
        yield return waitMagic;
        WaitForSeconds wait = new WaitForSeconds(interval);
        UtilityManager.utility.PlaySFX(warpSFX);
        canMove = true;

        List<OrbitingKunai> orbitingKunaiList = new List<OrbitingKunai>();

        for (int i = 0; i < repeat; i++)
        {
            GameObject orbitingKunai = UtilityManager.utility.GetFromPool(orbitingKunaiPool, magicCountInPool);
            if (orbitingKunai != null)
            {
                orbitingKunaiComp = orbitingKunai.GetComponent<OrbitingKunai>();
                orbitingKunaiComp.SetPool(orbitingKunaiPool);
                orbitingKunaiList.Add(orbitingKunaiComp);
            }
            if (i < repeat - 1)
            {
                yield return wait;
            }
        }

        yield return new WaitForSeconds(3.0f);

        foreach (OrbitingKunai kunai in orbitingKunaiList)
        {
            if (kunai != null)
            {
                kunai.Fire();
            }
        }
    }

    // FlyingShuriken 배치
    private IEnumerator UseFlyingShuriken(int repeat = 1, float interval = 0.4f)
    {
        canMove = false;
        yield return waitMagic;
        WaitForSeconds wait = new WaitForSeconds(interval);
        UtilityManager.utility.PlaySFX(warpSFX);
        canMove = true;

        for (int i = 0; i < repeat; i++)
        {
            GameObject flyingShuriken = UtilityManager.utility.GetFromPool(flyingShurikenPool, magicCountInPool);
            if (flyingShuriken != null)
            {
                flyingShurikenComp = flyingShuriken.GetComponent<FlyingShuriken>();
                flyingShurikenComp.SetPool(flyingShurikenPool, (Vector2)flyingShurikenSpawnPos.position);
            }
            if (i < repeat - 1)
            {
                yield return wait;
            }
        }
    }
    #endregion
}
