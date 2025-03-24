using System.Collections;
using UnityEngine;

public class PlayerAttack : MeleeAttack
{
    // 공격에 관한 메서드 모음

    // public 변수

    // private 변수
    private PlayerCtrl playerCtrl;
    private Animator playerAnim;

<<<<<<< HEAD
    public override void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);
        
        playerCtrl = GetComponent<PlayerCtrl>();
        playerAnim = GetComponent<Animator>();

        attackDamage = GameManager.instance.baseDamage;
=======
    // void Start()
    // {
    //     Init();

    //     attackDamage = playerCtrl.damage;
    //     playerCtrl = GetComponent<PlayerCtrl>();
    //     playerAnim = GetComponent<Animator>();
    // }

    public override void Init()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        playerAnim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);
        attackDamage = playerCtrl.damage;
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
    }

    // 근접 공격, 공격 범위 콜라이더 생성 후 일정 시간 후 종료, 현재는 0.2초
    public override void Attack()
    {

        // 공격 방향 설정
        float h = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(h, 0);
        if(h != 0)
        {
            lastDir = move;
        }
        Vector2 attackDir = UtilityManager.utility.DirSet(lastDir);

        if(Input.GetButtonDown("Fire1") && attackCollier.activeSelf == false)
        {
            // 공격시 해당 위치에 정지, 제어권 반환은 코루틴 끝날때
            playerCtrl.canMove = false;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            // 공격 방향에 따른 attackCollier 위치 결정
            attackCollierPos = attackCollier.transform.localPosition;
            attackCollierPos.x = Mathf.Abs(attackCollierPos.x) * attackDir.x;
            attackCollier.transform.localPosition = attackCollierPos;

            // 공격 활성화
            playerAnim.SetTrigger("Attack");
            playerAnim.SetFloat(attackDirHash, attackDir.x);
        }
    }

    // 공격 coll 설정은 animation event로 사용 중
    protected override void EnableAttackCollider()
    {
        attackCollier.SetActive(true);
    }
    protected override void DisableAttackCollider()
    {
        attackCollier.SetActive(false);
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerCtrl.canMove = true;
    }
}