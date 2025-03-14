using UnityEngine;
using System.Collections;

public class EnemyAttack : MeleeAttack
{
    private GroundCtrl groundCtrl;
    private Animator anim;

    void Awake()
    {
        groundCtrl = GetComponent<GroundCtrl>();
        anim = GetComponent<Animator>();
        baseAttackDamage = groundCtrl.readDamage;

        Init();
    }
    public override void Attack()
    {
        Vector2 attackDir = groundCtrl.DirSet(groundCtrl.readTarget.transform.position - transform.position);

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

    // 공격 coll 설정은 animation event로 사용 중
    private void EnableAttackCollider()
    {
        attackCollier.SetActive(true);
    }
    private void DisableAttackCollider()
    {
        attackCollier.SetActive(false);
    }
}
