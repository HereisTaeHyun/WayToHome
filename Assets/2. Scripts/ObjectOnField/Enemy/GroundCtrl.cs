using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

// 지상 적에 대한 클래스, 현재는 버섯, 해골에 사용 생각 중
public class GroundCtrl : EnemyCtrl
{
    private float jumpSpeed = 5.0f;
    private int maxJump = 1;
    private int jumpCount;
    private bool isMove;
    private Vector2 newVelocity;
    private EnemyAttack enemyAttack;
    private LayerMask playerLayer;
    [SerializeField] float attackRange;
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");
    private readonly int moveOnHash = Animator.StringToHash("OnMove");
    private readonly int dieHash = Animator.StringToHash("Die");

    void Start()
    {
        Init();
        playerLayer = LayerMask.GetMask("Player");
        isMove = false;
        enemyAttack = GetComponent<EnemyAttack>();
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
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, PlayerCtrl.player.transform.position) < scanningRadius && SeeingPlayer())
            {
                // 플레이어가 적보다 높으면 Jump 메서드 실행
                if(PlayerCtrl.player.transform.position.y > transform.position.y)
                {
                    Jump();
                    jumpCount += 1;
                }

                // 움직이는 방향 벡터 받아 오기
                Vector2 enemyMoveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);

                // 움직임 적용
                isMove = true;
                newVelocity.Set(enemyMoveDir.x * moveSpeed, rb2D.linearVelocity.y);
                rb2D.linearVelocity = newVelocity;

                // ray 사용하여 사거리 체크
                AttackAbleCheck(enemyMoveDir);
            }
            else
            {
                // scanningRadius 외부면 행동 불필요
                isMove = false;
                anim.SetBool(moveOnHash, isMove);
            }
        }
        // 적이 죽었다면 움직일 필요 없음
        else
        {
            isMove = false;
            anim.SetBool(moveOnHash, isMove);
        }
    }
    // Jump 횟수가 남았으면 Jump, Ground에 닿으면 Jump 횟수 초기화
    private void Jump()
    {
        if(jumpCount < maxJump)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpSpeed);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            jumpCount = 0;
        }  
    }

    // HP 변경 처리
    public override void ChangeHP(float value)
    {
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);

        // 타격 벡터 계산 및 sfx, anim 재생
        Vector2 hitVector =  UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        UtilityManager.utility.PlaySFX(enemyGetHitSFX);
        anim.SetTrigger(hitTrigger);
        anim.SetFloat(hitHash, hitVector.x);

        // 체력 0 이하면 사망처리
        if (currentHP <= 0)
        {
            EnemyDie();
        }
    }

    // Ray를 통한 사거리 체크
    private void AttackAbleCheck(Vector2 enemyMoveDir)
    {
        RaycastHit2D attackRangeCheck = Physics2D.Raycast(transform.position, enemyMoveDir, attackRange, playerLayer);
        if(attackRangeCheck)
        {
            enemyAttack.Attack();
            anim.SetFloat(moveDirHash, enemyMoveDir.x);
        }
        else
        {
            anim.SetBool(moveOnHash, isMove);
            anim.SetFloat(moveDirHash, enemyMoveDir.x);
        }
    }

    // 사망 처리
    protected override void EnemyDie()
    {
        // 아이템 확률 계산 및 드롭
        GameObject selectedItem = ItemDrop(itemInformation);
        UtilityManager.utility.SetItemFromPool(transform, selectedItem);

        StartCoroutine(DieStart());
    }
    // 사망 절차 진행, 사운드 재생 및 물리 영향 제거 후 사망 애니메이션 재생
    private IEnumerator DieStart()
    {
        isDie = true;
        UtilityManager.utility.PlaySFX(enemyDieSFX);
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }
}
