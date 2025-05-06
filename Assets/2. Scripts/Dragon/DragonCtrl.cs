using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class DragonCtrl : MonoBehaviour
{
    // private 변수
    private Animator anim;
    private bool canAttack;
    private float coolTime = 3.0f;

    [SerializeField] private Transform standingPosSet;
    private List<Transform> standingPoses = new List<Transform>();

    // magic List에 마법 저장, 스폰 포인트는 딕셔너리 관리
    Dictionary<MagicType, List<Transform>> magicSpawnPosDict;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int maxMagic = 5;

    // 위치 저장 셋
    [SerializeField] private List<Transform> fireBallSpawnPoses;
    [SerializeField] private List<Transform> fireMissileSpawnPoses;
    [SerializeField] private List<Transform> fireCannonSpawnPoses;
    [SerializeField] private List<Transform> shockWaveSpawnPoses;
    // 마법이 실제 시행될 개별 위치
    private Transform fireBallSpawnPos;
    private Transform meteorSpawnPos;

    private ObjectPool<GameObject> fireBallPool;
    private ObjectPool<GameObject> fireMissilePool;
    private ObjectPool<GameObject> fireCannonPool;
    private ObjectPool<GameObject> shockWavePool;

    // 마법 개별 컴포넌트
    private FireBall fireBallComp;
    private FireMissile fireMissileComp;
    private FireCannon fireCannonComp;
    private ShockWave shockWaveComp;

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
        magicSpawnPosDict = new Dictionary<MagicType, List<Transform>>()
        {
            {MagicType.FireBall, fireBallSpawnPoses},
            {MagicType.FireMissile, fireMissileSpawnPoses},
            {MagicType.FireCannon, fireCannonSpawnPoses},
            {MagicType.ShockWave, shockWaveSpawnPoses},
        };

        // 마법 풀 생성
        // 인덱스 번호는 위 마법 위치 딕셔너리와 같은 순서
        UtilityManager.utility.CreatePool(ref fireBallPool, magicList[0], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref fireMissilePool, magicList[1], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref fireCannonPool, magicList[2], maxMagic, maxMagic);
        UtilityManager.utility.CreatePool(ref shockWavePool, magicList[3], maxMagic, maxMagic);

        canAttack = true;
    }

    void Update()
    {
        Vector2 moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        anim.SetFloat(moveDirHash, moveDir.x);

        if(canAttack == true)
        {
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

            // fireBall에개 돌아와야 하는 풀 전달하기, 초기화
            fireBallComp.SetPool(fireBallPool);

            // 파이어볼 셋업
            int idx = Random.Range(0, fireBallSpawnPoses.Count);
            fireBallSpawnPos = fireBallSpawnPoses[idx];
            fireBall.transform.position = fireBallSpawnPos.position;
            fireBall.transform.rotation = fireBallSpawnPos.rotation;
        }
    }

    private void UseFireMissile()
    {
        foreach(Transform fireMissileSpawnPos in fireMissileSpawnPoses)
        {
            GameObject fireMissile = UtilityManager.utility.GetFromPool(fireMissilePool, maxMagic);

            if(fireMissile != null)
            {
                fireMissileComp = fireMissile.GetComponent<FireMissile>();
                fireMissileComp.SetPool(fireMissilePool);
                fireMissile.transform.position = fireMissileSpawnPos.transform.position;
                fireMissile.transform.rotation = fireMissileSpawnPos.transform.rotation;
            }
        }
    }

    // private void UseFireCannon()
    // {

    // }
#endregion
}
