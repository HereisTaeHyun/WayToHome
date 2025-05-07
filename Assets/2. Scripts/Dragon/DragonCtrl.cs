using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using System.Diagnostics;

public class DragonCtrl : MonoBehaviour
{
    // private 변수
    private Animator anim;
    private Vector2 moveDir;
    private bool canAttack;
    private float coolTime = 3.0f;

    [SerializeField] private Transform standingPosSet;
    private List<Transform> standingPoses = new List<Transform>();

    // magic List에 마법 저장, 스폰 포인트는 딕셔너리 관리
    // Dictionary<MagicType, List<Transform>> magicSpawnPosDict;
    private List<MagicType> usingMagic;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int maxMagic = 5;

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
        };

        // 마법 풀 생성
        // 인덱스 번호는 위 마법 위치 딕셔너리와 같은 순서
        UtilityManager.utility.CreatePool(ref fireBallPool, magicList[0], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref fireMissilePool, magicList[1], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref fireCannonPool, magicList[2], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref shockWavePool, magicList[3], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref meteorPool, magicList[4], maxMagic, maxMagic);

        canAttack = true;
    }

    void Update()
    {
        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        anim.SetFloat(moveDirHash, moveDir.x);

        if(canAttack == true)
        {
            // 마법을 선택 후 스위칭하여 마법 함수 실행
            int magicIdx = Random.Range(0, usingMagic.Count);
            MagicType currentMagic = usingMagic[magicIdx];

            switch(currentMagic)
            {
                case MagicType.FireBall:
                    break;
                case MagicType.FireMissile:
                    break;
                case MagicType.FireCannon:
                    break;
                case MagicType.ShockWave:
                    break;
                case MagicType.Meteor:
                    break;
            }

            // 공격 후 3초간 휴식
            StartCoroutine(CoolTimeCheck());
        }
    }

#region magic
// 마법 관련 로직들 정리
// 마법은 3번을 1세트로 사용
// 마법을 3번 시전하면 standingPoses 중 하나를 골라 비행 이동
// 연속으로 동일 마법 사용은 막을 것

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
        GameObject fireBall = UtilityManager.utility.GetFromPool(fireBallPool, maxMagic);

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
            GameObject fireMissile = UtilityManager.utility.GetFromPool(fireMissilePool, maxMagic);

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
        GameObject fireCannon = UtilityManager.utility.GetFromPool(fireCannonPool, maxMagic);

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
        GameObject shockWave = UtilityManager.utility.GetFromPool(shockWavePool, maxMagic);

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
        GameObject meteor = UtilityManager.utility.GetFromPool(meteorPool, maxMagic);

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
