using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class DragonCtrl : MonoBehaviour
{
    // private 변수
    private float maxHP = 50.0f;
    private float currentHP;
    private Rigidbody2D rb2D;
    private Animator anim;

    // 이동 및 상태 관련 변수
    private Vector2 moveDir;
    private bool canAttack;
    [SerializeField] private Transform standingPosSet;
    private List<Transform> standingPoses = new List<Transform>();
    private Transform movingPos;
    private float flyUpDownSpeed = 5.0f;
    private float flyingSpeed = 10.0f;
    private Vector2 newVelocity;

    // 마법 사용시마다 증가, magicCount == 5이면 standingPos 중 하나로 이동
    private int magicCount;
    private int magicCountUntilMove;


    // magic List에 마법 저장, 스폰 포인트는 딕셔너리 관리
    // Dictionary<MagicType, List<Transform>> magicSpawnPosDict;
    private List<MagicType> usingMagic;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int magicCountInPool = 5;
    private float coolTime = 3.0f;

    // 위치 저장 셋
    [SerializeField] private List<Transform> fireBallSpawnPoses;
    [SerializeField] private List<Transform> fireMissileSpawnPoses;
    [SerializeField] private List<Transform> fireCannonSpawnPoses;
    [SerializeField] private List<Transform> shockWaveSpawnPoses;
    // 마법이 실제 시행될 개별 위치, missile은 개별 생성이 아니기에 여기 없음
    private Transform fireBallSpawnPos;
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

    // 애니메이션 관련
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");


    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

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
        currentHP = maxHP;
        canAttack = true;
        magicCountUntilMove = 0;
        magicCountUntilMove = 5;
    }

    void Update()
    {
        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);

        // 땅에 있는지 비행 상태인지 체크하여 분기
        // 마법을 5 회 사용하면 비행, 아니면 지상에서 바라보기
        if(magicCount == magicCountUntilMove)
        {
            Fly();
        }
        else
        {
            anim.SetFloat(moveDirHash, moveDir.x);
        }

        if(canAttack == true)
        {
            // 마법을 선택 후 스위칭하여 마법 함수 실행
            int magicIdx = Random.Range(0, usingMagic.Count);
            MagicType currentMagic = usingMagic[magicIdx];
            switch(currentMagic)
            {
                case MagicType.FireBall:
                    UseFireBall();
                    break;
                case MagicType.FireMissile:
                    UseFireMissile();
                    break;
                case MagicType.FireCannon:
                    UseFireCannon();
                    break;
                case MagicType.ShockWave:
                    UseShockWave();
                    break;
                case MagicType.Meteor:
                    UseMeteor();
                    break;
            }

            // 공격 후 3초간 휴식
            magicCount += 1;
            StartCoroutine(CoolTimeCheck());
        }
    }

#region move

    private void Fly()
    {
        Debug.Log("비행 함수 호출됨");
        // 마법 사용 횟수 초기화 후 타겟 목표를 잡아 이동
        // 비행 시 canAttack false하고 이동 끝나면 canAttack true
        magicCount = 0;

        rb2D.gravityScale = 0.0f;
        newVelocity.Set(rb2D.linearVelocity.x, flyUpDownSpeed * Vector2.up.y);
        rb2D.linearVelocity = newVelocity;

        // int moveIdx = Random.Range(0, standingPoses.Count);
        // movingPos = standingPoses[moveIdx];
    }

#endregion

#region magic
// 마법 관련 로직들 정리
// 마법은 5번을 1세트로 사용
// 마법을 5번 시전하면 standingPoses 중 하나를 골라 비행 이동

    // 마법 쿨 타임
    private IEnumerator CoolTimeCheck()
    {
        canAttack = false;
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }

    private void UseFireBall()
    {
        // 풀 오브젝트 가져오기
        GameObject fireBall = UtilityManager.utility.GetFromPool(fireBallPool, magicCountInPool);

        if(fireBall != null)
        { 
            fireBallComp = fireBall.GetComponent<FireBall>();

            // 파이어볼 셋업
            int idx = Random.Range(0, fireBallSpawnPoses.Count);
            fireBallSpawnPos = fireBallSpawnPoses[idx];
            fireBall.transform.position = fireBallSpawnPos.position;
            fireBall.transform.rotation = fireBallSpawnPos.rotation;

            // fireBall에개 돌아와야 하는 풀 전달하기, 초기화
            fireBallComp.SetPool(fireBallPool);
        }
    }

    private void UseFireMissile()
    {
        // 각 위치 순회하여 미사일 배치
        foreach(Transform fireMissileSpawnPos in fireMissileSpawnPoses)
        {
            // 파이어 미사일은 생성 로직상 다른 마법보다 많이 필요함, 하나만 필요하니 일단 하드코딩 처리
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

    // 파이어 캐논, 쇼크웨이브는 위치가 플레이어가 왼쪽인지 오른쪽인지에 따라 발사 위치 결정
    private void UseFireCannon()
    {
        GameObject fireCannon = UtilityManager.utility.GetFromPool(fireCannonPool, magicCountInPool);

        if(fireCannon != null)
        {
            fireCannonComp = fireCannon.GetComponent<FireCannon>();
            if(moveDir.x < 0)
            {
                fireCannonSpawnPos = fireCannonSpawnPoses[0];
            }
            else if(moveDir.x > 0)
            {
                fireCannonSpawnPos = fireCannonSpawnPoses[1];
            }

            fireCannon.transform.position = fireCannonSpawnPos.transform.position;
            fireCannon.transform.rotation = fireCannonSpawnPos.transform.rotation;

            fireCannonComp.SetPool(fireCannonPool);
        }
    }

    private void UseShockWave()
    {
        GameObject shockWave = UtilityManager.utility.GetFromPool(shockWavePool, magicCountInPool);

        if(shockWave != null)
        {
            shockWaveComp = shockWave.GetComponent<ShockWave>();
            if(moveDir.x < 0)
            {
                shockWaveSpawnPos = shockWaveSpawnPoses[0];
            }
            else if(moveDir.x > 0)
            {
                shockWaveSpawnPos = shockWaveSpawnPoses[1];
            }

            shockWave.transform.position = shockWaveSpawnPos.transform.position;
            shockWave.transform.rotation = shockWaveSpawnPos.transform.rotation;

            shockWaveComp.SetPool(shockWavePool);
        }
    }

    private void UseMeteor()
    {
        GameObject meteor = UtilityManager.utility.GetFromPool(meteorPool, magicCountInPool);

        if(meteor != null)
        {
            meteorComp = meteor.GetComponent<Meteor>();

            // 플레이어 머리 위에 생성
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
