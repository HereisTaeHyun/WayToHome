using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerAttack : MeleeAttack
{
    // 공격에 관한 메서드 모음

    // public 변수
    // 마법 관련
    public GameObject[] UsingMagic = new GameObject[2];

    // private 변수
    [SerializeField] private AudioClip attackSFX;
    private readonly int attckHash = Animator.StringToHash("Attack");

    // 마법 관련
    private GameObject selectedMagic;
    public Transform magicSpawnPos { get; private set; }
    private int selectedMagicIdx;
    private int maxMagic = 20;

    private PlayerMagicBase magicComp;

    public override void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);

        magicSpawnPos = transform.Find("MagicSpawnPos").transform;
    }

    void OnEnable()
    {
        PlayerCtrl.player.SelectMagic += SelectMagic;
    }
    void OnDisable()
    {
        PlayerCtrl.player.SelectMagic -= SelectMagic;
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
            PlayerCtrl.player.playerAnim.SetTrigger(attackHash);
            PlayerCtrl.player.playerAnim.SetFloat(attackDirHash, attackDir.x);
        }
        else if (PlayerCtrl.player.isMagic == true)
        {
            // 공격시 해당 위치에 정지, 제어권 반환은 코루틴 끝날때
            PlayerCtrl.player.canMove = false;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            // 공격 방향에 따른 magicSpawnPos 위치 결정
            Vector3 spawnPos = magicSpawnPos.localPosition;
            spawnPos.x = Mathf.Abs(spawnPos.x) * attackDir.x;
            magicSpawnPos.localPosition = spawnPos;

            UtilityManager.utility.PlaySFX(attackSFX);
            PlayerCtrl.player.playerAnim.SetTrigger(attackHash);
            PlayerCtrl.player.playerAnim.SetFloat(attackDirHash, attackDir.x);

            StartCoroutine(CastMagic());
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

    private void SelectMagic(int idx)
    {
        selectedMagicIdx = idx;
    }

    // 마법 사용
    private IEnumerator CastMagic()
    {
        selectedMagic = UsingMagic[selectedMagicIdx];

        yield return new WaitForSeconds(0.3f);

        var pool = UtilityManager.utility.CreatePlayerMagicPool(selectedMagic, maxMagic, maxMagic);
        var magic = UtilityManager.utility.GetFromPool(pool, maxMagic);
        if (magic != null)
        {
            magicComp = magic.GetComponentInChildren<PlayerMagicBase>();
            magicComp.SetPool(pool);
            PlayerCtrl.player.currentMana = Mathf.Clamp(PlayerCtrl.player.currentMana - magicComp.costMana, 0, PlayerCtrl.player.maxMana);
        }

        yield return new WaitForSeconds(0.3f);
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerCtrl.player.canMove = true;
    }
}