using UnityEngine;
using UnityEngine.Pool;

public class PlayerAttack : MeleeAttack
{
    // 공격에 관한 메서드 모음

    // public 변수
    // 마법 관련
    public GameObject[] UsingMagic = new GameObject [2];

    // private 변수
    private Animator playerAnim;
    [SerializeField] private AudioClip attackSFX;
    private readonly int attckHash = Animator.StringToHash("Attack");

    // 마법 관련
    private Transform magicSpawnPos;
    private ObjectPool<GameObject> firstMagicPool;
    private ObjectPool<GameObject> secondMagicPool;

    private Kunai kunaiComp;
    private Shuriken shurikenComp;

    public override void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);

        magicSpawnPos = transform.Find("MagicSpawnPos").transform;

        playerAnim = GetComponent<Animator>();
    }

    // 근접 공격, 공격 범위 콜라이더 생성 후 일정 시간 후 종료
    public override void Attack()
    {
        // 공격 방향 설정
        Vector2 attackDir = PlayerCtrl.player.lastMoveDir;

        if (attackCollier.activeSelf == false && PlayerCtrl.player.isMagic == false)
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
            playerAnim.SetTrigger(attackHash);
            playerAnim.SetFloat(attackDirHash, attackDir.x);
        }
        else if (PlayerCtrl.player.isMagic == true)
        {
            // 공격 방향에 따른 magicSpawnPos 위치 결정
            Vector3 spawnPos = magicSpawnPos.localPosition;
            spawnPos.x = Mathf.Abs(spawnPos.x) * attackDir.x;
            magicSpawnPos.localPosition = spawnPos;

            Debug.Log(magicSpawnPos.transform.position);
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