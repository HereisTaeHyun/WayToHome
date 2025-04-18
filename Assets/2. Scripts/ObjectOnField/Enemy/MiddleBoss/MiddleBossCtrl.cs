using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MiddleBossCtrl : EnemyCtrl
{
    // public 변수
    // private 변수
    private float distance;
    [SerializeField] private Transform warpPointSet;
    private List<Transform> warpPoints = new List<Transform>();
    private MiddleBossMeleeAttack middleBossMeleeAttack;
    [SerializeField] float attackRange;
    [SerializeField] private LayerMask playerLayer;

    private bool canAttack;
    private float coolTime = 1.0f;

    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    void Start()
    {
        Init();

        foreach(Transform warpPoint in warpPointSet)
        {
            warpPoints.Add(warpPoint);
        }
        warpPoints.ToArray();

        middleBossMeleeAttack = GetComponent<MiddleBossMeleeAttack>();
        canAttack = true;
    }

    void Update()
    {
        // 게임 오버가 아닐 경우 행동
        if(GameManager.instance.readIsGameOver == true)
        {
            return;
        }

        // distance에 따라 행동 분리
        distance = Vector2.Distance(target.position, transform.position);

        if(distance > scanningRadius)
        {
            Warp();
        }
        else
        {
            // radious 내부라면 바라보기 + 공격
            Vector2 moveDir = UtilityManager.utility.DirSet(target.transform.position - transform.position);
            anim.SetFloat(moveDirHash, moveDir.x);

            // 사거리 내부면 근접 공격, 외부면 마법 공격
            // 근접 공격은 밀어내기 효과, 외부는 둔화 효과 고민 중
            MeleeAttackAbleCheck(moveDir);
        }
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
            transform.position = pointAbleAttack.position;
        }
    }

    // Ray를 통한 사거리 체크
    private void MeleeAttackAbleCheck(Vector2 moveDir)
    {
        // ray 내부면 근접, 아니면 마법
        RaycastHit2D attackTypeCheck = Physics2D.Raycast(transform.position, moveDir, attackRange, playerLayer);
        if(attackTypeCheck && canAttack == true)
        {
            middleBossMeleeAttack.Attack();
            StartCoroutine(CoolTimeCheck());
            anim.SetFloat(moveDirHash, moveDir.x);
        }
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
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);

        // 타격 벡터 계산 및 sfx, anim 재생
        UtilityManager.utility.PlaySFX(enemyGetHitSFX);

        // 체력 0 이하면 사망처리
        if (currentHP <= 0)
        {
            EnemyDie();
        }
    }
}
