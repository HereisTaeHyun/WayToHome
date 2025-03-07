using UnityEngine;
using System.Collections;
public class MeleeAttck : MonoBehaviour
{
    // 공격에 관한 메서드 모음
    // 근접 공격, 원거리 직선 공격, 원거리 포물선 공격 계획

    // public 변수
    public float attackPower;

    // private 변수
    protected Rigidbody2D rb2D;
    protected GameObject attackCollier;
    protected Vector2 attackCollierPos;
    protected Vector2 lastDir = Vector2.right;
    protected readonly int attackHash = Animator.StringToHash("Attack");
    protected readonly int attackDirHash = Animator.StringToHash("AttackDir");

    [SerializeField] protected float baseAttackPower = -1.0f;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        attackPower = baseAttackPower;

        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);
    }

    // 근접 공격, 코루틴으로 공격 범위 콜라이더 생성 후 일정 시간 후 종료, 현재는 0.2초
    public virtual void MeleeAttack()
    {
    }

    protected virtual IEnumerator MeleeAttackOn()
    {
        yield return new WaitForSeconds(0.2f);
    }
}