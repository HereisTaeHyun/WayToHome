using UnityEngine;
public class MeleeAttack : MonoBehaviour
{
    // 공격에 관한 메서드

    // public 변수
    public float attackDamage;

    // private 변수
    protected Rigidbody2D rb2D;
    protected GameObject attackCollier;
    protected Vector2 attackCollierPos;
    protected Vector2 lastDir = Vector2.right;
    protected readonly int attackHash = Animator.StringToHash("Attack");
    protected readonly int attackDirHash = Animator.StringToHash("AttackDir");

    public virtual void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();

        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);
    }

    // 근접 공격, 코루틴으로 공격 범위 콜라이더 생성 후 일정 시간 후 종료, 현재는 0.2초
    public virtual void Attack()
    {
    }
    
    // 공격 coll 설정은 animation event로 사용 중
    protected virtual void EnableAttackCollider()
    {
    }
    protected virtual void DisableAttackCollider()
    {
    }
}