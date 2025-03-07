using UnityEngine;
using System.Collections;

public class GroundCtrl : EnemyCtrl
{
    private bool isMove;
    protected readonly int moveOnHash = Animator.StringToHash("OnMove");
    void Awake()
    {
        Init();
        isMove = false;
    }

    
    void FixedUpdate()
    {
        if (canMove)
        {
            FollowingTarget(moveSpeed, scanningRadius);
        }
    }

    // enemyMoveDir이 음수면 왼쪽 양수면 오른쪽
    protected override void FollowingTarget(float moveSpeed, float scanningRadius)
    {
        if(target != null)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius)
            {
                Vector2 enemyMoveDir = DirSet(target.transform.position - transform.position);
                isMove = true;
                anim.SetBool(moveOnHash, true);
                anim.SetFloat("MoveDir", enemyMoveDir.x);
            }
            else
            {
                anim.SetBool(moveOnHash, false);
            }
        }
    }

    // HP 변경 처리 (데미지 적용)
    public override void ChangeHP(float value)
    {
        base.ChangeHP(value);
    }

    // 적이 피격당했을 때
    protected override IEnumerator EnemyGetHit()
    {
        canMove = false;
        Vector2 hitVector =  DirSet(target.transform.position - transform.position);

        // 타격에 따른 애니메이션 재생
        anim.SetTrigger(hitTrigger);
        anim.SetFloat(hitHash, hitVector.x);

        // 타격 받은 방향으로 밀려남
        rb2D.AddForce(hitVector * enemyPushPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunTime);
        rb2D.linearVelocity = Vector2.zero;
        canMove = true;
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
