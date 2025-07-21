using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MamaMush : BossCtrl
{
    private bool isMove;
    private Vector2 newVelocity;
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    private readonly int moveOnHash = Animator.StringToHash("OnMove");
    private readonly int dieHash = Animator.StringToHash("Die");

    protected override void Init()
    {
        base.Init();
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            FollowingTarget(moveSpeed);
        }
    }

    // 이동 및 점프 처리
    // enemyMoveDir이 음수면 왼쪽 양수면 오른쪽
    private void FollowingTarget(float moveSpeed)
    {
        // 타겟이 존재하고 살아 있을 경우 움직임
        if(GameManager.instance.readIsGameOver == false && isDie == false)
        {
            // 플레이어가 존 내부면 moveSpeed만큼씩 이동 시작
            if(SeeingPlayer())
            {
                // 움직이는 방향 벡터 받아 오기
                Vector2 enemyMoveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);

                // 움직임 적용
                isMove = true;

                anim.SetBool(moveOnHash, isMove);
                anim.SetFloat(moveDirHash, enemyMoveDir.x);

                newVelocity.Set(enemyMoveDir.x * moveSpeed, rb2D.linearVelocity.y);
                rb2D.linearVelocity = newVelocity;
            }
        }
        // 적이 죽었다면 움직일 필요 없음
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
        anim.SetTrigger(hitTrigger);
        anim.SetFloat(hitHash, hitVector.x);

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
        UtilityManager.utility.PlaySFX(enemyDieSFX);
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }
}
