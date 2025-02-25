using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // 공격에 관한 메서드 모음
    // 근접 공격, 원거리 직선 공격, 원거리 포물선 공격 계획

    // public 변수
    public float attackPower;

    // private 변수
    private Rigidbody2D rb;
    private PlayerCtrl playerCtrl;
    private Animator playerAnim;
    private GameObject attackCollier;
    Vector2 attackCollierPos;
    private Vector2 lastDir = Vector2.right;
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int attackDirHash = Animator.StringToHash("AttackDir");

    [SerializeField] private float baseAttackPower = -1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCtrl = GetComponent<PlayerCtrl>();
        playerAnim = GetComponent<Animator>();
        attackPower = baseAttackPower;

        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);
    }

    // 근접 공격, 코루틴으로 공격 범위 콜라이더 생성 후 일정 시간 후 종료, 현재는 0.2초
    public void MeleeAttack()
    {

        // 공격 방향 설정
        float h = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(h, 0);
        if(h != 0)
        {
            lastDir = move;
        }
        Vector2 attackDir = playerCtrl.MoveDirSet(lastDir);

        if(Input.GetButtonDown("Fire1") && attackCollier.activeSelf == false)
        {
            // 공격시 해당 위치에 정지, 제어권 반환은 코루틴 끝날때
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            // 공격 방향에 따른 attackCollier 위치 결정
            attackCollierPos = attackCollier.transform.localPosition;
            attackCollierPos.x = Mathf.Abs(attackCollierPos.x) * attackDir.x;
            attackCollier.transform.localPosition = attackCollierPos;

            // 공격 활성화
            playerAnim.SetTrigger("Attack");
            playerAnim.SetFloat(attackDirHash, attackDir.x);
            StartCoroutine(MeleeAttackOn());
        }
    }
    private IEnumerator MeleeAttackOn()
    {
        // 공격 시에는 공격 콜라이더 생성 후 공격 시에 설정한 Constraints 재설정
        yield return new WaitForSeconds(0.2f);
        attackCollier.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackCollier.SetActive(false);

        // Z축 고정만 남기고 나머지는 해제
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
