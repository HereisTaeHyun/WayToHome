using UnityEngine;
using UnityEngine.Pool;

public class FireHammer : MagicBase
{
    public MagicType magicType;
    private Collider2D attackCollider;
    private bool isAttack;
    private float attackTriggerTime = 5.0f;
    [SerializeField] private AudioClip attackSFX;
    private int attackHash = Animator.StringToHash("Attack");
    private int spawnHash = Animator.StringToHash("Spawn");

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        damage = -10.0f;
        attackCollider = GetComponent<Collider2D>();
    }

    protected override void FixedUpdate()
    {
        if (isAttack == true)
        {
            return;
        }

        attackTriggerTime -= Time.deltaTime;

        if (attackTriggerTime <= 0)
        {
            Attack();
            UtilityManager.utility.PlaySFX(attackSFX);
            isAttack = true;
        }
    }

    private void Attack()
    {
        anim.SetTrigger(attackHash);
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
        isAttack = false;
        attackTriggerTime = 5.0f;

        anim.SetTrigger(spawnHash);
    }

    // Player에 닿으면 공격
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.instance.readIsGameOver == false)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // 플레이어가 무적이 아니라면 공격
                if (PlayerCtrl.player.readIsInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                }
            }
        }
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    // 마법이 끝나고 리턴
    public void ReturnAfterAnim()
    {
        UtilityManager.utility.ReturnToPool(originPool, gameObject);
        isPool = true;
        rb2D.simulated = true;
    }
}
