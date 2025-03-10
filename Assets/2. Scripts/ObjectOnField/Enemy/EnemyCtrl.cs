using System.Collections;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    // Enemy의 공통 행동: 추적, 사망, 아이템 드롭

    // public 변수
    public GameObject[] dropItem;

    // protected 변수
#region private
    protected Rigidbody2D rb2D;
    protected bool canMove;
    [SerializeField] protected float enemyPushPower;
    [SerializeField] protected float stunTime;
    protected float scanningRadius = 10.0f;
    [SerializeField] protected float MaxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected float moveSpeed;
    protected readonly int dirHash = Animator.StringToHash("MoveDir");
    protected readonly int hitHash = Animator.StringToHash("HitDir");
    protected readonly int hitTrigger = Animator.StringToHash("TakeHit");

    // 다른 객체에서 읽기 위한 변수
    protected Transform target;
    public Transform readTarget {get {return target;}}
    protected Animator anim;
    public Animator readAnim {get {return anim;}}
    [SerializeField] protected float damage;
    public float readDamage {get {return damage;}}
#endregion

    // 초기화
    protected virtual void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        canMove = true;
        currentHP = MaxHP;
    }
    
    // 사정 거리 내부에 집입하는 경우 따라가기 메서드
    protected virtual void FollowingTarget(float moveSpeed, float scanningRadius)
    {
    }

    // 이동 방향 벡터 계산
    public virtual Vector2 DirSet(Vector2 move)
    {
        Vector2 moveDir = Vector2.zero;
        if (!Mathf.Approximately(move.x, 0))
        {
            moveDir.Set(move.x, 0);
            moveDir.Normalize();
        }
        return moveDir;
    }

    // HP 변경 처리 (데미지 적용)
    public virtual void ChangeHP(float value)
    {
        StartCoroutine(EnemyGetHit());
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);

        if (currentHP <= 0)
        {
            EnemyDie();
        }
    }

    // 적이 피격당했을 때
    protected virtual IEnumerator EnemyGetHit()
    {
        yield return new WaitForSeconds(stunTime);
    }

    // 사망 처리
    protected virtual void EnemyDie()
    {
        Destroy(gameObject);
    }
}
