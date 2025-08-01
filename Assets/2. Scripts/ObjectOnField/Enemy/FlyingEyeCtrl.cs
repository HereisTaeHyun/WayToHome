using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 하늘을 날면서 타겟을 추적하는 적에 대한 클래스
public class FlyingEyeCtrl : EnemyCtrl
{
    [SerializeField] private float pushPower;
    private Collider2D coll;
    private Collider2D playerColl;
    private readonly int fallHash = Animator.StringToHash("Fall");
    private readonly int dieHash = Animator.StringToHash("Die");

    void Start()
    {
        Init();

        coll = GetComponent<Collider2D>();
        playerColl = PlayerCtrl.player.GetComponent<Collider2D>();
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
            if(Vector2.Distance(transform.position, PlayerCtrl.player.transform.position) < scanningRadius && SeeingPlayer())
            {
                // 이동 방향 벡터 설정
                Vector2 enemyMoveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
                anim.SetFloat("MoveDir", enemyMoveDir.x);

                // 플레이어에게 이동
                Vector2 newPosition = Vector2.MoveTowards(rb2D.position, PlayerCtrl.player.transform.position, moveSpeed * Time.fixedDeltaTime);
                rb2D.MovePosition(newPosition);
            }
        }
    }

    // HP 변경 처리
    public override void ChangeHP(float value)
    {
        base.ChangeHP(value);
        UtilityManager.utility.PlaySFX(enemyGetHitSFX);
    }

    // 적이 피격당했을 때
    protected override IEnumerator EnemyGetHit()
    {
        canMove = false;
        Vector2 hitVector =  UtilityManager.utility.AllDirSet(transform.position - PlayerCtrl.player.transform.position);

        // 타격에 따른 애니메이션 재생
        anim.SetTrigger(hitTrigger);
        anim.SetFloat(hitHash, hitVector.x);

        // 타격 받은 방향으로 밀려남
        rb2D.AddForce(hitVector * pushPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunTime);
        rb2D.linearVelocity = Vector2.zero;
        canMove = true;
    }

    protected override GameObject ItemDrop(Dictionary<GameObject, float> item)
    {
        return base.ItemDrop(item);
    }

#region Die
    // 사망 처리
    protected override void EnemyDie()
    {
        // 아이템 확률 계산 및 드롭
        if(isDie == false)
        {
            GameObject selectedItem = ItemDrop(itemInformation);
            UtilityManager.utility.SetItemFromPool(transform, selectedItem);
        }

        StartCoroutine(DieStart());
    }

    // 사망 절차 진행
    private IEnumerator DieStart()
    {
        // 사망 및 중력 적용
        DataManager.dataManager.playerData.diedEnemy.Add(enemyID);
        UtilityManager.utility.PlaySFX(enemyDieSFX);

        isDie = true;
        rb2D.gravityScale = 1.0f;

        anim.SetTrigger(fallHash);
        Physics2D.IgnoreCollision(coll, playerColl, true);

        // 잠시 후 setfalse
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Ground") || other.collider.CompareTag("Platform"))
        {
            // 사망한 적이 땅에 닿으면
            if(isDie == true)
            {
                UtilityManager.utility.PlaySFX(enemyDieSFX);
                anim.SetTrigger(dieHash);
                rb2D.bodyType = RigidbodyType2D.Kinematic;
                rb2D.simulated = false;
            }
        }
    }
#endregion
}
