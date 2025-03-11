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
        Vector2 attackDir = groundCtrl.DirSet(groundCtrl.readTarget.transform.position - transform.position);;

        // 공격 방향에 따른 attackCollier 위치 결정
        attackCollierPos = attackCollier.transform.localPosition;
        attackCollierPos.x = Mathf.Abs(attackCollierPos.x) * attackDir.x;
        attackCollier.transform.localPosition = attackCollierPos;

        if(attackCollier.activeSelf == false)
        {
            StartCoroutine(ActiveAttack());
            anim.SetTrigger(attackHash);
        }
    }

    protected override IEnumerator ActiveAttack()
    {
        // 공격 시에는 공격 콜라이더 생성 후 종료
        yield return new WaitForSeconds(0.5f);
        attackCollier.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackCollier.SetActive(false);
    }
}
