using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class DragonCtrl : MonoBehaviour
{
    // private 변수
    private bool canAttack;
    private float coolTime = 2.0f;

    [SerializeField] private Transform standingPosSet;
    private List<Transform> standingPoses = new List<Transform>();

    // magic List에 마법 저장, 스폰 포인트는 딕셔너리 관리
    Dictionary<MagicType, List<Transform>> magicSpawnPosDict;
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();
    private int maxMagic = 5;

    [SerializeField] private List<Transform> fireBallSpawnPoses;
    [SerializeField] private List<Transform> fireMissileSpawnPoses;
    [SerializeField] private List<Transform> fireCannonSpawnPoses;
    [SerializeField] private List<Transform> shockWaveSpawnPoses;
    private Transform meteorSpawnPos;

    private ObjectPool<GameObject> fireBallPool;
    private ObjectPool<GameObject> fireMissilePool;
    private ObjectPool<GameObject> fireCannonPool;
    private ObjectPool<GameObject> shockWavePool;


    void Start()
    {
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
    }

#region magic
// 마법 관련 로직들 정리
// 마법은 3번을 1세트로 사용
// 마법을 3번 시전하면 standingPoses 중 하나를 골라 비행 이동

    // 마법 쿨 타임
    private IEnumerator CoolTimeCheck()
    {
        canAttack = false;
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }
#endregion
}
