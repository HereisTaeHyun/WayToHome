using UnityEngine;

public class MiddleBossMeleeAttack : MeleeAttack
{
    // 중간 보스의 근접 공격 컨트롤
    private MiddleBossCtrl middleBossCtrl;
    private Animator anim;

    void Start()
    {
        middleBossCtrl = GetComponent<MiddleBossCtrl>();
        anim = GetComponent<Animator>();

        Init();
    }
    public override void Attack()
    {
        Vector2 attackDir = UtilityManager.utility.DirSet(middleBossCtrl.readTarget.transform.position - transform.position);

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
        attackCollier.SetActive(true);
    }
    protected override void DisableAttackCollider()
    {
        attackCollier.SetActive(false);
    }
}
