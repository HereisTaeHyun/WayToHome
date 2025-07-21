using System.Collections;
using UnityEngine;

public class MamaMush : EnemyCtrl
{
    private SpriteRenderer spriteRenderer;
    private bool ableBlink;
    private float blinkTime = 0.1f;
    private bool isMove;
    private Vector2 newVelocity;
    [SerializeField] float attackRange;
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    private readonly int moveOnHash = Animator.StringToHash("OnMove");
    private readonly int dieHash = Animator.StringToHash("Die");

    void Start()
    {
        Init();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ableBlink = true;
        isMove = false;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            FollowingTarget(moveSpeed, scanningRadius);
        }
    }

    // 이동 및 점프 처리
    // enemyMoveDir이 음수면 왼쪽 양수면 오른쪽
    protected override void FollowingTarget(float moveSpeed, float scanningRadius)
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
    // HP 변경 처리
    public override void ChangeHP(float value)
    {
        if(ableBlink == true)
        {
            StartCoroutine(UtilityManager.utility.BlinkOnDamage(spriteRenderer, ableBlink, blinkTime));
        }
        currentHP = Mathf.Clamp(currentHP + value, 0, maxHP);
        UtilityManager.utility.PlaySFX(enemyGetHitSFX);

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
