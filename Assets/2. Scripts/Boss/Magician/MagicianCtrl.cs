using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class MagicianCtrl : BossCtrl
{
    // public 변수

    // private 변수
    private Coroutine blinkRoutine;

    private float rageHP;
    [SerializeField] AudioClip rageSFX;


    private MagicianMeleeAttack magicianMeleeAttack;

    [SerializeField] private List<MagicType> usingMagic;
    [SerializeField] private List<MagicType> phase1UsingMagic;
    [SerializeField] private List<MagicType> phase2UsingMagic;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();

    private enum RangeType
    {
        Close,
        Long,
    }
    private Dictionary<RangeType, List<MagicType>> magicRangeInformation =
    new Dictionary<RangeType, List<MagicType>>
    {
        { RangeType.Close, new List<MagicType>{ MagicType.FireBall, MagicType.FireVortex } },
        { RangeType.Long,  new List<MagicType>{ MagicType.FireCannon, MagicType.FireVortex, MagicType.FireHammer } },
    };

    private int magicCountInPool = 20;

    [SerializeField] private Transform warpPointSet;
    private float warpChargingDistance = 8.0f;
    private float warpChargingTime = 10.0f;
    private List<Transform> warpPoints = new List<Transform>();
    private float distanceToPlayer;
    private bool isWarpCharging;
    private int lastWarpIdx;

    [SerializeField] private Transform magicSpawnPosSet;
    private List<Transform> magicSpawnPoses = new List<Transform>();
    private Transform magicSpawnPos;

    [SerializeField] private Transform fireHammerSpawnPosSet;
    private List<Transform> fireHammerSpawnPoses = new List<Transform>();
    private Transform fireHammerSpawnPos;
    private int lastfireHammerSpawnIdx;

    private Vector2 moveDir;


    private ObjectPool<GameObject> fireBallPool;
    private FireBall fireBallComp;
    private ObjectPool<GameObject> fireCannonPool;
    private FireCannon fireCannonComp;
    private ObjectPool<GameObject> fireVortexPool;
    private FireVortex fireVortexComp;
    private ObjectPool<GameObject> fireHammerPool;
    private FireHammer fireHammerComp;


    [SerializeField] float meleeAttackRange;
    [SerializeField] private AudioClip warpSFX;
    [SerializeField] private AudioClip fireMagicSFX;
    private readonly int dieHash = Animator.StringToHash("Die");

    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    [SerializeField] protected float damage;

    public float readDamage { get { return damage; } }

    protected override void Init()
    {
        base.Init();
    }

    void Start()
    {
        Init();


        foreach (Transform warpPoint in warpPointSet)
        {
            warpPoints.Add(warpPoint);
        }

        // 공격 관련 초기화
        usingMagic = phase1UsingMagic;
        magicianMeleeAttack = GetComponent<MagicianMeleeAttack>();

        foreach (Transform magicSpawnPos in magicSpawnPosSet)
        {
            magicSpawnPoses.Add(magicSpawnPos);
        }

        foreach (Transform fireHammerSpawnPos in fireHammerSpawnPosSet)
        {
            fireHammerSpawnPoses.Add(fireHammerSpawnPos);
        }

        UtilityManager.utility.CreatePool(ref fireBallPool, magicList[0], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref fireCannonPool, magicList[1], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref fireVortexPool, magicList[2], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref fireHammerPool, magicList[3], magicCountInPool, magicCountInPool);

        rageHP = maxHP * 0.6f;
        canAttack = true;
        coolTime = 4.0f;

        if (DataManager.dataManager.playerData.diedEnemy.Contains(enemyID))
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        distanceToPlayer = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);

        // 게임 오버나 스턴이 아닐 경우 행동
        if (GameManager.instance.readIsGameOver == true || isDie == true)
        {
            return;
        }

        if (canMove)
        {
            anim.SetFloat(dirHash, moveDir.x);

            // 플레이어가 일정 거리 이내면 워프 준비
            if (distanceToPlayer <= warpChargingDistance && isWarpCharging == false)
            {
                StartCoroutine(ChargingWarp());
            }
        }

        if (canAttack && SeeingPlayer())
        {
            if (distanceToPlayer <= meleeAttackRange)
            {
                MeleeAttackAble(moveDir);
            }
            else if (distanceToPlayer <= 10f)
            {
                UseMagicByRange(RangeType.Close);
            }
            else
            {
                UseMagicByRange(RangeType.Long);
            }
            // 공격 실행 후 쿨타임 진입
            StartCoroutine(CoolTimeCheck());
        }
    }

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
        usingMagic = phase2UsingMagic;

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

    private void UseMagicByRange(RangeType rangeType)
    {
        // usingMagic 중 rangeType을 가진 교집합의 리스트화
        var selectedMagic = usingMagic.Intersect(magicRangeInformation[rangeType]).ToList();

        int magicIdx = Random.Range(0, selectedMagic.Count);
        MagicType currentMagic = selectedMagic[magicIdx];

        UseRandomMagic(currentMagic);
    }

    private void UseRandomMagic(MagicType magicType)
    {
        switch (magicType)
        {
            case MagicType.FireBall:
                StartCoroutine(isRage ? UseFireBall(3) : UseFireBall(1));
                break;

            case MagicType.FireCannon:
                StartCoroutine(isRage ? UseFireCannon(3) : UseFireCannon(2));
                break;

            case MagicType.FireVortex:
                StartCoroutine(UseFireVortex(6));
                break;
                
            case MagicType.FireHammer:
                StartCoroutine(UseFireHammer(2));
                break;
        }
    }


    // UseFireBall 배치
    private IEnumerator UseFireBall(int repeat = 1, float interval = 0.1f)
    {
        // 풀 오브젝트 가져오기
        WaitForSeconds wait = new WaitForSeconds(interval);
        for (int i = 0; i < repeat; i++)
        {
            GameObject fireBall = UtilityManager.utility.GetFromPool(fireBallPool, magicCountInPool);

            if (fireBall != null)
            {
                fireBallComp = fireBall.GetComponent<FireBall>();

                // fireBall에개 돌아와야 하는 풀 전달하기, 초기화
                fireBallComp.SetPool(fireBallPool);

                // 파이어볼 셋업
                int idx = Random.Range(0, magicSpawnPoses.Count);
                magicSpawnPos = magicSpawnPoses[idx];
                fireBall.transform.position = magicSpawnPos.position;
                fireBall.transform.rotation = magicSpawnPos.rotation;
            }
            if (i < repeat - 1)
            {
                UtilityManager.utility.PlaySFX(fireMagicSFX);
                yield return wait;
            }
        }
    }

    // FireCannon 배치
    private IEnumerator UseFireCannon(int repeat = 1, float interval = 0.4f)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        for (int i = 0; i < repeat; i++)
        {
            foreach (Transform magicSpawnPos in magicSpawnPoses)
            {
                GameObject fireCannon = UtilityManager.utility.GetFromPool(fireCannonPool, magicCountInPool);

                if (fireCannon != null)
                {
                    fireCannonComp = fireCannon.GetComponent<FireCannon>();
                    fireCannon.transform.position = magicSpawnPos.position;
                    fireCannon.transform.rotation = magicSpawnPos.rotation;
                    fireCannonComp.SetPool(fireCannonPool);
                }
            }
            if (i < repeat - 1)
            {
                UtilityManager.utility.PlaySFX(fireMagicSFX);
                yield return wait;
            }
        }
    }

    // FireVortex 배치
    private IEnumerator UseFireVortex(int repeat = 1, float interval = 0.8f)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        for (int i = 0; i < repeat; i++)
        {
            GameObject fireVortex = UtilityManager.utility.GetFromPool(fireVortexPool, magicCountInPool);

            if (fireVortex != null)
            {
                fireVortexComp = fireVortex.GetComponent<FireVortex>();

                Vector2 fireVortexSpawnPos = new Vector3
                (PlayerCtrl.player.transform.position.x,
                PlayerCtrl.player.transform.position.y + 1.5f,
                PlayerCtrl.player.transform.position.z);

                fireVortex.transform.position = fireVortexSpawnPos;
                fireVortex.transform.rotation = PlayerCtrl.player.transform.rotation;
                fireVortexComp.SetPool(fireVortexPool);
                UtilityManager.utility.PlaySFX(fireMagicSFX);
            }
            if (i < repeat - 1)
            {
                yield return wait;
            }
        }
    }

    // FireHammer 배치
    private IEnumerator UseFireHammer(int repeat = 1, float interval = 0.8f)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        for (int i = 0; i < repeat; i++)
        {
            GameObject fireHammer = UtilityManager.utility.GetFromPool(fireHammerPool, magicCountInPool);

            if (fireHammer != null)
            {
                fireHammerComp = fireHammer.GetComponent<FireHammer>();

                int fireHammerSpawnPosIdx;
                do
                {
                    fireHammerSpawnPosIdx = Random.Range(0, fireHammerSpawnPoses.Count);
                }
                while (fireHammerSpawnPosIdx == lastfireHammerSpawnIdx);
                lastfireHammerSpawnIdx = fireHammerSpawnPosIdx;

                fireHammerSpawnPos = fireHammerSpawnPoses[fireHammerSpawnPosIdx];

                fireHammer.transform.position = fireHammerSpawnPos.position;
                fireHammer.transform.rotation = PlayerCtrl.player.transform.rotation;
                fireHammerComp.SetPool(fireHammerPool);
                UtilityManager.utility.PlaySFX(fireMagicSFX);
            }
            if (i < repeat - 1)
            {
                yield return wait;
            }
        }
    }

    // 근접 공격
    private void MeleeAttackAble(Vector2 moveDir)
    {
        magicianMeleeAttack.Attack();
        anim.SetFloat(moveDirHash, moveDir.x);
    }
    #endregion

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
}
