using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class GroundCtrl : EnemyCtrl
{
    [SerializeField] private float jumpSpeed;
    private int maxJump = 1;
    private int jumpCount;
    private bool isMove;
    private Vector2 newVelocity;
    private EnemyAttack enemyAttack;
    protected readonly int moveOnHash = Animator.StringToHash("OnMove");

    void Awake()
    {
        Init();
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
        if(target != null)
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

                // 공격 메서드 실행
                enemyAttack.Attack();

                // 움직이는 방향을 받아온 후 움직임 실행
                Vector2 enemyMoveDir = DirSet(target.transform.position - transform.position);
                isMove = true;

                newVelocity.Set(enemyMoveDir.x * moveSpeed, rb2D.linearVelocity.y);
                rb2D.linearVelocity = newVelocity;
                
                anim.SetBool(moveOnHash, isMove);
                anim.SetFloat("MoveDir", enemyMoveDir.x);
            }
            else
            {
                // scanningRadius 외부면 행동 불필요
                isMove = false;
                anim.SetBool(moveOnHash, isMove);
            }
        }
        else
        {
            isMove = false;
            anim.SetBool(moveOnHash, isMove);
        }
    }
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
            EnemyDie();
        }
    }

    // 사망 처리
    protected override void EnemyDie()
    {
        float itemChoose = Random.Range(0, 100);
        if (itemChoose < 90)
        {
            Instantiate(dropItem[0], transform.position, transform.rotation);
        }
        else if (itemChoose >= 90)
        {
            Instantiate(dropItem[1], transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
