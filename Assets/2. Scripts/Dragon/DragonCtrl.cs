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

    [SerializeField] private Transform standingPointSet;
    private List<Transform> standingPoints = new List<Transform>();

    // magic List에 마법 저장, 스폰 포인트는 딕셔너리 관리
    [SerializeField] private List<GameObject> magicList = new List<GameObject>();

    [SerializeField] private List<Transform> fireBallSpawnPoses;
    [SerializeField] private List<Transform> fireMissileSpawnPoses;

    // 스폰 위치 단일인 마법들
    private Transform fireCannonSpawnPos;
    private Transform shockWaveSpawnPos;
    private Transform meteorSpawnPos;

    Dictionary<string, List<Transform>> magicSpawnPosDict;

    void Start()
    {
        foreach(Transform standingPoint in standingPointSet)
        {
            standingPoints.Add(standingPoint);
        }

        magicSpawnPosDict = new Dictionary<string, List<Transform>>()
        {
            {"FireBall", fireBallSpawnPoses},
            {"FireMissle", fireMissileSpawnPoses},
        };
    }

    private IEnumerator CoolTimeCheck()
    {
        canAttack = false;
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }
}
