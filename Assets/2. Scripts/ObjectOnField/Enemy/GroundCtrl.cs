using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

// 지상 적에 대한 클래스, 현재는 버섯, 해골에 사용 생각 중
public class GroundCtrl : EnemyCtrl
{
    [SerializeField] private float jumpSpeed;
    private int maxJump = 1;
    private int jumpCount;
    private bool isMove;
    private bool isDie;
    [SerializeField] private bool canAttack;
    private Vector2 newVelocity;
    private EnemyAttack enemyAttack;
    private float attackRange = 1.3f;
    [SerializeField] private LayerMask playerLayer;
    private readonly int moveOnHash = Animator.StringToHash("OnMove");
    private readonly int dieHash = Animator.StringToHash("Die");

    void Awake()
    {
        Init();
        isMove = false;
        isDie = false;
        canAttack = false;
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
        if(target != null && isDie == false)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius)
            {
                // 플레이어가 적보다 높으면 Jump 메서드 실행
                if(target.position.y > transform.position.y)
                {
                    Jump();
                    jumpCount += 1;
                }

                // 움직이는 방향 벡터 받아 오기
                Vector2 enemyMoveDir = DirSet(target.transform.position - transform.position);

                // ray 사용하여 사거리 체크
                AttackRangeCheck(enemyMoveDir);

                // 움직임 적용
                isMove = true;
                newVelocity.Set(enemyMoveDir.x * moveSpeed, rb2D.linearVelocity.y);
                rb2D.linearVelocity = newVelocity;

                // 사거리 내부면 공격 메서드 및 공격 애니메이션 실행
                if(canAttack == true)
                {
                    enemyAttack.Attack();
                    anim.SetFloat("MoveDir", enemyMoveDir.x);
                }
                else // 사거리 외부면 움직임 애니메이션 실행
                {
                    anim.SetBool(moveOnHash, isMove);
                    anim.SetFloat("MoveDir", enemyMoveDir.x);
                }

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

        // 타격 벡터 계산 및 anim 재생
        Vector2 hitVector =  DirSet(target.transform.position - transform.position);
        anim.SetTrigger(hitTrigger);
        anim.SetFloat(hitHash, hitVector.x);

        // 체력 0 이하면 사망처리
        if (currentHP <= 0)
        {
            isDie = true;
            EnemyDie();
        }
    }

    // Ray를 통한 사거리 체크
    private void AttackRangeCheck(Vector2 enemyMoveDir)
    {
        RaycastHit2D attackRangeCheck = Physics2D.Raycast(transform.position, enemyMoveDir, attackRange, playerLayer);
        if(attackRangeCheck)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }

    // 사망 처리
    protected override void EnemyDie()
    {
        // 아이템 확률 계산 및 드롭, 확장성이 전무하여 수정 필요성 있음, 확률 가중치를 정하고 전체의 합을 정규화하여 확률 계산 가능할까?
        float itemChoose = Random.Range(0, 100);
        if (itemChoose < 90)
        {
            Instantiate(dropItem[0], transform.position, transform.rotation);
        }
        else if (itemChoose >= 90)
        {
            Instantiate(dropItem[1], transform.position, transform.rotation);
        }
        StartCoroutine(DestroyObject());
    }
    // 사망 절차 진행, 물리 영향 제거 후 사망 애니메이션 재생
    private IEnumerator DestroyObject()
    {
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        anim.SetTrigger(dieHash);
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
