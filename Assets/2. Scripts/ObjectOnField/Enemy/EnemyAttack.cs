using UnityEngine;
using System.Collections;

public class EnemyAttack : MeleeAttack
{
    private GroundCtrl groundCtrl;
    private Animator enemyAnim;

    void Awake()
    {
        Init();

        groundCtrl = GetComponent<GroundCtrl>();
        enemyAnim = GetComponent<Animator>();
    }
    public override void Attack()
    {
        Vector2 attackDir = groundCtrl.DirSet(lastDir);

        // 공격 방향에 따른 attackCollier 위치 결정
        attackCollierPos = attackCollier.transform.localPosition;
        attackCollierPos.x = Mathf.Abs(attackCollierPos.x) * attackDir.x;
        attackCollier.transform.localPosition = attackCollierPos;
    }

    protected override IEnumerator ActiveAttack()
    {
        // 공격 시에는 공격 콜라이더 생성 후 종료
        yield return new WaitForSeconds(0.2f);
        attackCollier.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackCollier.SetActive(false);
    }
}
