using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using System;
using Unity.Cinemachine;

public class DragonCtrl : MonoBehaviour, IDamageable
{
    // public 변수
    public enum DragonState
    {
        Idle,
        StartFly,
        OnFly,
        EndFly,
    }
    [NonSerialized] public DragonState dragonState;

    // private 변수
    private float maxHP = 50.0f;
    private float currentHP;
    private bool isDie;
    private Rigidbody2D rb2D;
    private Animator anim;

    // 죽으면 Set할 포탈, 씬에 있는 active false해 둔 포탈임, 프레팝 아님
    [SerializeField] private GameObject portalOnScene;
    private GameObject portalSpawnPoint;

    // 이동 및 상태 관련 변수
    private Vector2 seeDir;
    private Vector2 moveDir;

    [SerializeField] private Transform standingPosSet;
    private List<Transform> standingPoses = new List<Transform>();
    private Vector2 nextPos;
    private Transform targetPos;
    private float flyUpDownSpeed = 5.0f;
    private float flyingSpeed = 10.0f;
    private Vector2 newPosition;
    private SpriteRenderer spriteRenderer;
    private bool ableBlink;
    private static float BLINK_TIME = 0.1f;

    [SerializeField] private CinemachineCamera cam;
    CinemachineConfiner2D confiner;

    // 공격 조건 변수
    private bool isFly;
    private bool canAttack;
    private LayerMask detectLayer;
    private RaycastHit2D[] rayHits = new RaycastHit2D[10];

    // 마법 사용시마다 증가, magicCount == 5이면 standingPos 중 하나로 이동
    private int magicCount;
    private int magicCountUntilMove = 5;


    // magic List에 마법 저장
    private List<MagicType> usingMagic;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int magicCountInPool = 5;
    private static float MAGIC_WAIT_TIME = 0.5f; // 마법 사용과 애니메이션간 타이밍 맞추기에 사용
    private float coolTime = 3.0f;

    // 위치 저장 셋
    [SerializeField] private List<Transform> fireBallSpawnPoses;
    [SerializeField] private List<Transform> fireMissileSpawnPoses;
    [SerializeField] private List<Transform> fireCannonSpawnPoses;
    [SerializeField] private List<Transform> shockWaveSpawnPoses;

    // 마법이 실제 시행될 개별 위치, ball과 missile은 개별 생성이 아니기에 여기 없음
    private Transform fireCannonSpawnPos;
    private Transform shockWaveSpawnPos;
    private Vector3 meteorSpawnPos;

    private ObjectPool<GameObject> fireBallPool;
    private ObjectPool<GameObject> fireMissilePool;
    private ObjectPool<GameObject> fireCannonPool;
    private ObjectPool<GameObject> shockWavePool;
    private ObjectPool<GameObject> meteorPool;

    // 마법 개별 컴포넌트
    private FireBall fireBallComp;
    private FireMissile fireMissileComp;
    private FireCannon fireCannonComp;
    private ShockWave shockWaveComp;
     private Meteor meteorComp;

    // 오디오 관련
    [SerializeField] private AudioClip getHitSFX;
    [SerializeField] private AudioClip dieSFX;
    [SerializeField] private AudioClip UseFireMagic;
    [SerializeField] private AudioClip UseMeteorMagic;
    [SerializeField] private AudioClip ShockWaveSFX;

    // 애니메이션 관련
    private readonly int seeDirHash = Animator.StringToHash("SeeDir");
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int attackTypeHash = Animator.StringToHash("AttackType");
    private readonly int flyHash = Animator.StringToHash("Fly");
    private readonly int flyStateHash = Animator.StringToHash("FlyState");
    private readonly int dieHash = Animator.StringToHash("Die");


    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalSpawnPoint = transform.Find("PortalSpawnPoint").gameObject;
        dragonState = DragonState.Idle;

        // 이동, 마법 필요 위치 전달 및 저장
        foreach(Transform standingPoint in standingPosSet)
        {
            standingPoses.Add(standingPoint);
        }
        usingMagic = new List<MagicType>()
        {
            {MagicType.FireBall},
            {MagicType.FireMissile},
            {MagicType.FireCannon},
            {MagicType.ShockWave},
            {MagicType.Meteor},
        };

        // 마법 풀 생성
        // 인덱스 번호는 위 마법 위치 딕셔너리와 같은 순서
        UtilityManager.utility.CreatePool(ref fireBallPool, magicList[0], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref fireMissilePool, magicList[1], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref fireCannonPool, magicList[2], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref shockWavePool, magicList[3], magicCountInPool, magicCountInPool);
        UtilityManager.utility.CreatePool(ref meteorPool, magicList[4], magicCountInPool, magicCountInPool);

        // 변수 초기화
        isDie = false;
        currentHP = maxHP;
        isFly = false;
        canAttack = true;
        ableBlink = true;
        magicCount = 0;
        detectLayer = LayerMask.GetMask("Player", "Ground", "Wall");
    }

    void Update()
    {
        if(isDie == true)
        {
            return;
        }

        seeDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        anim.SetFloat(seeDirHash, seeDir.x);

        // 땅에 있는지 비행 상태인지 체크하여 분기
        // 마법을 5 회 사용하면 비행, 아니면 지상에서 바라보기
        if(magicCount == magicCountUntilMove && !anim.GetCurrentAnimatorStateInfo(0).IsName("UseMagic") && !anim.IsInTransition(0))
        {
            Fly();
        }

        if(canAttack == true && SeeingPlayer())
        {
            // 마법을 선택 후 스위칭하여 마법 함수 실행
            int magicIdx = Random.Range(0, usingMagic.Count);
            MagicType currentMagic = usingMagic[magicIdx];
            switch(currentMagic)
            {
                case MagicType.FireBall:
                    StartCoroutine(UseFireBall());
                    anim.SetTrigger(attackHash);
                    anim.SetFloat(attackTypeHash, -1);
                    break;
                case MagicType.FireMissile:
                    StartCoroutine(UseFireMissile());
                    anim.SetTrigger(attackHash);
                    anim.SetFloat(attackTypeHash, -1);
                    break;
                case MagicType.FireCannon:
                    StartCoroutine(UseFireCannon());
                    anim.SetTrigger(attackHash);
                    anim.SetFloat(attackTypeHash, -1);
                    break;
                case MagicType.ShockWave:
                    StartCoroutine(UseShockWave());
                    anim.SetTrigger(attackHash);
                    anim.SetFloat(attackTypeHash, 1);
                    break;
                case MagicType.Meteor:
                    StartCoroutine(UseMeteor());
                    anim.SetTrigger(attackHash);
                    anim.SetFloat(attackTypeHash, -1);
                    break;
            }

            // 공격 후 3초간 휴식
            magicCount += 1;
            StartCoroutine(CoolTimeCheck());
        }
    }

    // ray를 쏘아 첫 대상이 플레이어인지 감지 = 시야 개념
    private bool SeeingPlayer()
    {
        Vector2 direction = PlayerCtrl.player.transform.position - transform.position;
        Vector2 directionNorm = UtilityManager.utility.AllDirSet(direction);
        float distance = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);
        int count = Physics2D.RaycastNonAlloc(transform.position, directionNorm, rayHits, distance, detectLayer);

        Debug.DrawRay(transform.position, directionNorm * distance, Color.red, 0.1f); 

        // ray에 닿은 존재가 있으며 첫 충돌이 playerLayer라면 true
        if (count > 0)
        {
            var hit = rayHits[0];

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        return false;
    }

#region HP, Die
    public virtual void ChangeHP(float value)
    {
        if (isFly == true)
        {
            return;
        }
        if (ableBlink == true)
            {
                StartCoroutine(BlinkOnDamage());
            }
        currentHP = Mathf.Clamp(currentHP + value, 0, maxHP);
        UtilityManager.utility.PlaySFX(getHitSFX);

        if (currentHP <= 0)
        {
            StartCoroutine(EnemyDie());
        }
    }

    
    // 데미지 입으면 깜빡거리기 코루틴
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

    private IEnumerator EnemyDie()
    {
        UtilityManager.utility.PlaySFX(dieSFX);
        isDie = true;
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        anim.SetTrigger(dieHash);

        yield return new WaitForSeconds(3.0f);

        // 사망 시 다음 스테이지로 가기 위한 포탈을 자기 위치에 생성
        portalOnScene.SetActive(true);
        portalOnScene.transform.position = portalSpawnPoint.transform.position;
    }

#endregion

#region Fly
// 드래곤이 공중으로 이동하는 비행 로직을 처리
    private void Fly()
    {
        isFly = true;
        canAttack = false;  
        switch(dragonState)
        {
            // 비행 시작 전 대기 상태에서 초기 상승 위치 설정
            case DragonState.Idle:
                nextPos = new Vector2(transform.position.x, transform.position.y + 13.0f);
                dragonState = DragonState.StartFly;
                break;

            // 위로 상승하면서 비행 시작, 목표 높이에 도달하면 다음 이동 위치 설정
            case DragonState.StartFly:
                anim.SetBool(flyHash, true);
                anim.SetInteger(flyStateHash, 0);

                newPosition = Vector2.MoveTowards(transform.position, nextPos, flyUpDownSpeed * Time.fixedDeltaTime);
                rb2D.MovePosition(newPosition);
                StartCoroutine(ZoomInOut(10.0f, 3.0f));

                if (Mathf.Approximately(transform.position.y, nextPos.y))
                {
                    dragonState = DragonState.OnFly;
                    int moveIdx = Random.Range(0, standingPoses.Count);
                    targetPos = standingPoses[moveIdx];
                }
                break;

            // 비행 상태에서 수평 방향으로 목표 위치까지 이동
            case DragonState.OnFly:
                anim.SetInteger(flyStateHash, 1);
                nextPos = new Vector2(targetPos.position.x, transform.position.y);
                newPosition = Vector2.MoveTowards(transform.position, nextPos, flyingSpeed * Time.fixedDeltaTime);

                Vector2 moveDirX = new Vector2(targetPos.position.x - transform.position.x, transform.position.y);
                moveDir = UtilityManager.utility.HorizontalDirSet(moveDirX);
                anim.SetFloat(moveDirHash, moveDir.x);

                rb2D.MovePosition(newPosition);

                if (Mathf.Approximately(transform.position.x, targetPos.position.x))
                {
                    dragonState = DragonState.EndFly;
                }
                break;

            // 목표 위치에 도달하면 천천히 하강하면서 착지
            case DragonState.EndFly:
                anim.SetInteger(flyStateHash, 2);
                nextPos = new Vector2(transform.position.x, targetPos.position.y);

                newPosition = Vector2.MoveTowards(transform.position, nextPos, flyUpDownSpeed * Time.fixedDeltaTime);
                rb2D.MovePosition(newPosition);
                StartCoroutine(ZoomInOut(3.5f, 3.0f));

                if (Mathf.Abs(transform.position.y - targetPos.position.y) < 0.1f)
                {
                    anim.SetInteger(flyStateHash, 3);
                    anim.SetBool(flyHash, false);
                    dragonState = DragonState.Idle;
                    StartCoroutine(CoolTimeCheck());
                    isFly = false;
                    magicCount = 0;
                }
                break;
        }
    }

    private IEnumerator ZoomInOut(float targetSize, float changeTime)
    {
        confiner = cam.GetComponent<CinemachineConfiner2D>();
        float time = 0.0f;
        var lens = cam.Lens;
        float currentSize = lens.OrthographicSize;

        while(time <= changeTime)
        {
            time += Time.deltaTime;
            float t = time/changeTime;

            lens.OrthographicSize = Mathf.Lerp(currentSize, targetSize, t);
            cam.Lens = lens;
            confiner?.InvalidateLensCache();
            yield return null;
        }

        lens.OrthographicSize = targetSize;
        cam.Lens = lens;
        confiner?.InvalidateLensCache();
    }

#endregion

#region magic
    // 마법 공격 후 일정 시간 동안 쿨타임 처리
    private IEnumerator CoolTimeCheck()
    {
        canAttack = false;
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }

    // FireBall 마법을 스폰 위치에 따라 생성 및 초기화
    private IEnumerator UseFireBall()
    {
        yield return new WaitForSeconds(MAGIC_WAIT_TIME);
        UtilityManager.utility.PlaySFX(UseFireMagic);
        foreach(Transform fireBallSpawnPos in fireBallSpawnPoses)
        {
            GameObject fireBall = UtilityManager.utility.GetFromPool(fireBallPool, magicCountInPool);
            
            if(fireBall != null)
            { 
                fireBallComp = fireBall.GetComponent<FireBall>();
                fireBall.transform.position = fireBallSpawnPos.position;
                fireBall.transform.rotation = fireBallSpawnPos.rotation;
                fireBallComp.SetPool(fireBallPool);
            }
        }
    }

    // FireMissile 마법을 각 지정된 위치에 생성 및 초기화
    private IEnumerator UseFireMissile()
    {
        yield return new WaitForSeconds(MAGIC_WAIT_TIME);
        UtilityManager.utility.PlaySFX(UseFireMagic);
        foreach(Transform fireMissileSpawnPos in fireMissileSpawnPoses)
        {
            GameObject fireMissile = UtilityManager.utility.GetFromPool(fireMissilePool, 10);

            if(fireMissile != null)
            {
                fireMissileComp = fireMissile.GetComponent<FireMissile>();
                fireMissile.transform.position = fireMissileSpawnPos.transform.position;
                fireMissile.transform.rotation = fireMissileSpawnPos.transform.rotation;
                fireMissileComp.SetPool(fireMissilePool);
            }
        }
    }

    // FireCannon 마법을 플레이어 방향에 따라 좌/우 위치에 생성
    private IEnumerator UseFireCannon()
    {
        yield return new WaitForSeconds(MAGIC_WAIT_TIME);
        UtilityManager.utility.PlaySFX(UseFireMagic);
        GameObject fireCannon = UtilityManager.utility.GetFromPool(fireCannonPool, magicCountInPool);

        if(fireCannon != null)
        {
            fireCannonComp = fireCannon.GetComponent<FireCannon>();
            if(seeDir.x < 0)
            {
                fireCannonSpawnPos = fireCannonSpawnPoses[0];
            }
            else if(seeDir.x > 0)
            {
                fireCannonSpawnPos = fireCannonSpawnPoses[1];
            }

            fireCannon.transform.position = fireCannonSpawnPos.transform.position;
            fireCannon.transform.rotation = fireCannonSpawnPos.transform.rotation;

            fireCannonComp.SetPool(fireCannonPool);
        }
    }

    // ShockWave 마법을 플레이어 방향에 따라 좌/우 위치에 생성
    private IEnumerator UseShockWave()
    {
        yield return new WaitForSeconds(1.7f);
        UtilityManager.utility.PlaySFX(ShockWaveSFX);
        GameObject shockWave = UtilityManager.utility.GetFromPool(shockWavePool, magicCountInPool);

        if(shockWave != null)
        {
            shockWaveComp = shockWave.GetComponent<ShockWave>();
            if(seeDir.x < 0)
            {
                shockWaveSpawnPos = shockWaveSpawnPoses[0];
            }
            else if(seeDir.x > 0)
            {
                shockWaveSpawnPos = shockWaveSpawnPoses[1];
            }

            shockWave.transform.position = shockWaveSpawnPos.transform.position;
            shockWave.transform.rotation = shockWaveSpawnPos.transform.rotation;

            shockWaveComp.SetPool(shockWavePool);
        }
    }

    // Meteor 마법을 플레이어 머리 위에 생성
    private IEnumerator UseMeteor()
    {
        yield return new WaitForSeconds(MAGIC_WAIT_TIME);
        UtilityManager.utility.PlaySFX(UseMeteorMagic);
        GameObject meteor = UtilityManager.utility.GetFromPool(meteorPool, magicCountInPool);

        if(meteor != null)
        {
            meteorComp = meteor.GetComponent<Meteor>();

            meteorSpawnPos = new Vector3
            (PlayerCtrl.player.transform.position.x, 
            PlayerCtrl.player.transform.position.y + 10f, 
            PlayerCtrl.player.transform.position.z);

            meteor.transform.position = meteorSpawnPos;

            meteorComp.SetPool(meteorPool);
        }
    }
#endregion
}
