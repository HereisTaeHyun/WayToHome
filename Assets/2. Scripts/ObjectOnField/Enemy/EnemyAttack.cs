using UnityEngine;
using System.Collections;

public class EnemyAttack : MeleeAttack
{
    private GroundCtrl groundCtrl;
    private Animator anim;

    void Start()
    {
        groundCtrl = GetComponent<GroundCtrl>();
        anim = GetComponent<Animator>();
        attackDamage = groundCtrl.readDamage;

        Init();
    }
    public override void Attack()
    {
        Vector2 attackDir = UtilityManager.utility.DirSet(groundCtrl.readTarget.transform.position - transform.position);

        // 공격 방향에 따른 attackCollier 위치 결정
        attackCollierPos = attackCollier.transform.localPosition;
        attackCollierPos.x = Mathf.Abs(attackCollierPos.x) * attackDir.x;
        attackCollier.transform.localPosition = attackCollierPos;

        // 이전 공격 콜라이더가 없을 경우 ActiveAttack
        if(attackCollier.activeSelf == false)
        {
            anim.SetTrigger(attackHash);
        }
    }

    protected override void EnableAttackCollider()
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        attackCollier.SetActive(true);
    }
    protected override void DisableAttackCollider()
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        attackCollier.SetActive(false);
    }
}
