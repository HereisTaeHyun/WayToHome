using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// 하늘을 날면서 타겟을 추적하는 적에 대한 클래스
public class FlyingEyeCtrl : EnemyCtrl
{
    Collider2D coll;
    Collider2D playerColl;
    private readonly int dieHash = Animator.StringToHash("Die");

    void Awake()
    {
        Init();

        coll = GetComponent<Collider2D>();
        playerColl = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    }

    
    void FixedUpdate()
    {
        if (canMove)
        {
            FollowingTarget(moveSpeed, scanningRadius);
        }
    }

    protected override void FollowingTarget(float moveSpeed, float scanningRadius)
    {
        if(GameManager.instance.readIsGameOver == false && isDie == false)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius)
            {
                // 이동 방향 벡터 설정
                Vector2 enemyMoveDir = UtilityManager.utility.DirSet(target.transform.position - transform.position);
                anim.SetFloat("MoveDir", enemyMoveDir.x);

                // 플레이어에게 이동
                Vector2 newPosition = Vector2.MoveTowards(rb2D.position, target.position, moveSpeed * Time.fixedDeltaTime);
                rb2D.MovePosition(newPosition);
            }
        }
    }

    // HP 변경 처리
    public override void ChangeHP(float value)
    {
        base.ChangeHP(value);
    }

    // 적이 피격당했을 때
    protected override IEnumerator EnemyGetHit()
    {
        canMove = false;
        Vector2 hitVector =  UtilityManager.utility.DirSet(transform.position - target.transform.position);

        // 타격에 따른 애니메이션 재생
        anim.SetTrigger(hitTrigger);
        anim.SetFloat(hitHash, hitVector.x);

        // 타격 받은 방향으로 밀려남
        rb2D.AddForce(hitVector * enemyPushPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunTime);
        rb2D.linearVelocity = Vector2.zero;
        canMove = true;
    }

    protected override GameObject ItemDrop(Dictionary<GameObject, float> item)
    {
        return base.ItemDrop(item);
    }

    // 사망 처리
    protected override void EnemyDie()
    {
        GameObject selectedItem = ItemDrop(itemInformation);
        Instantiate(selectedItem, transform.position, transform.rotation);
        StartCoroutine(DieStart());
    }

    // 사망 절차 진행, 물리 영향 제거 후 사망 애니메이션 재생
    // Flying Eye는 공중 비행형이기에 지면까지 떨어지는 시간이 필요함
    private IEnumerator DieStart()
    {
        // 사망 및 중력 적용
        isDie = true;
        rb2D.gravityScale = 1.0f;
        anim.SetTrigger(dieHash);
        Physics2D.IgnoreCollision(coll, playerColl, true);
        yield return new WaitForSeconds(0.5f);

        // 물리 제거 후 파괴
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = false;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
