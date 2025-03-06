using System.Collections;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    // Enemy의 공통 행동에 대한 변수들
    // 추적, 사망 및 아이템 드롭이 들어 갈 예정

    // public 변수
    public GameObject[] dropItem;

    // private 변수
    private Rigidbody2D rb2D;
    private Transform target;
    private bool canMove;
    [SerializeField] private float enemyPushPower;
    private float scanningRadius = 10.0f;
    [SerializeField] float MaxHP;
    [SerializeField] float currentHP;
    [SerializeField] private float moveSpeed;
    private readonly int dirHash = Animator.StringToHash("MoveDir");
    private readonly int hitHash = Animator.StringToHash("HitDir");
    private readonly int hitTrigger = Animator.StringToHash("TakeHit");

    // 다른 객체에서 읽기 필요한 변수
    private Animator enemyAnim;
    public Animator readEnemyAnim {get {return enemyAnim;}}
    [SerializeField] private float damage;
    public float readDamage {get {return damage;}}

    // target인 Player를 받아온 후 초기화
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        canMove = true;
        currentHP = MaxHP;
    }

    // 행동에 대한 메서드 넣을 예정
    void Update()
    {
        if(canMove == false)
        {
            return;
        }
        FollowingTarget(moveSpeed, scanningRadius);
    }
    
    // 사정 거리 내부에 집입하는 경우 따라가기 메서드
    public void FollowingTarget(float moveSpeed, float scanningRadius)
    {
        if(target != null)
        {
            // 플레이어가 scanningRadius 내부면 moveSpeed만큼씩 이동 시작
            if(Vector2.Distance(transform.position, target.position) < scanningRadius)
            {
                // 이동 방향 벡터 설정
                Vector2 enemyMoveDir = DirSet(transform.position - target.transform.position);
                enemyAnim.SetFloat("MoveDir", enemyMoveDir.x);

                // 플레이어에게 이동
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    public Vector2 DirSet(Vector2 move)
    {
        Vector2 moveDir = new Vector2(0, 0);
        if(Mathf.Approximately(move.x, 0) == false)
        {
            moveDir.Set(move.x, 0);
            moveDir.Normalize();
        }
        return moveDir;
    }

    public void ChangeHP(float value)
    {
        // 데미지를 받고 데미지가 0이거나 그 이하일 경우 사망
        StartCoroutine(enemyGetHIt());
        currentHP = Mathf.Clamp(currentHP + value, 0, MaxHP);
        Debug.Log($" {value} 데미지 가해짐");
        if(currentHP <= 0)
        {
            EnemyDie();
        }
    }
    private IEnumerator enemyGetHIt()
    {
        canMove = false;
        Vector2 hitVector =  DirSet(transform.position - target.transform.position);

        // 타격에 따른 애니메이션 재생
        enemyAnim.SetTrigger(hitTrigger);
        enemyAnim.SetFloat(hitHash, hitVector.x);

        // 타격 받은 방향으로 밀려남
        rb2D.AddForce(hitVector * enemyPushPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        rb2D.linearVelocity = Vector2.zero;
        canMove = true;
    }

    // 사망 현재는 단순 Destroy이나 이후 아이템 생성 필요
    private void EnemyDie()
    {
        float itemChoose = Random.Range(0, 100);
        if (itemChoose < 90)
        {
            Instantiate(dropItem[0], transform.position, transform.rotation);
        }
        else if (itemChoose >= 90)
        {
            Instantiate(dropItem[1], transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

}
