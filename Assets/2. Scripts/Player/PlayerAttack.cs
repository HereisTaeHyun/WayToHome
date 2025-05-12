using UnityEngine;

public class PlayerAttack : MeleeAttack
{
    // 공격에 관한 메서드 모음

    // public 변수

    // private 변수
    private Animator playerAnim;
    [SerializeField] private AudioClip attackSFX;

    public override void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);
        
        playerAnim = GetComponent<Animator>();
    }

    // 근접 공격, 공격 범위 콜라이더 생성 후 일정 시간 후 종료
    public override void Attack()
    {

        // 공격 방향 설정
        float h = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(h, 0);
        if(h != 0)
        {
            lastDir = move;
        }
        Vector2 attackDir = UtilityManager.utility.HorizontalDirSet(lastDir);

        if(Input.GetButtonDown("Fire1") && attackCollier.activeSelf == false)
        {
            // 공격시 해당 위치에 정지, 제어권 반환은 코루틴 끝날때
            PlayerCtrl.player.canMove = false;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            // 공격 방향에 따른 attackCollier 위치 결정
            attackCollierPos = attackCollier.transform.localPosition;
            attackCollierPos.x = Mathf.Abs(attackCollierPos.x) * attackDir.x;
            attackCollier.transform.localPosition = attackCollierPos;

            // 공격 활성화
            UtilityManager.utility.PlaySFX(attackSFX);
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
        PlayerCtrl.player.canMove = true;
    }
}