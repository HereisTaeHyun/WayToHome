using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerAttack : MeleeAttack
{
    // 공격에 관한 메서드 모음

    // public 변수
    // 마법 관련
    public GameObject[] usingMagic = new GameObject[2];
    public int selectedMagicIdx;

    // private 변수
    [SerializeField] private AudioClip attackSFX;    

    // 마법 관련
    private GameObject selectedMagic;
    public Transform magicSpawnPos { get; private set; }
    private int maxMagic = 20;

    public override void Init()
    {
        rb2D = GetComponent<Rigidbody2D>();
        attackCollier = transform.Find("MeleeAttack").gameObject;
        attackCollier.SetActive(false);

        magicSpawnPos = transform.Find("MagicSpawnPos").transform;
        PlayerCtrl.player.InvokeSelectMagic(selectedMagicIdx);
    }

    void OnEnable()
    {
        PlayerCtrl.player.SelectMagic += SelectMagicIdx;
    }
    void OnDisable()
    {
        PlayerCtrl.player.SelectMagic -= SelectMagicIdx;
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

    private void SelectMagicIdx(int idx)
    {
        selectedMagicIdx = idx;
    }

    // 마법 사용
    private IEnumerator CastMagic()
    {
        if (usingMagic == null)
        {
            yield break;
        }
        if (selectedMagicIdx < 0 || selectedMagicIdx >= usingMagic.Length)
        {
            yield break;
        }

        selectedMagic = usingMagic[selectedMagicIdx];
        if (selectedMagic == null)
        {
            yield break;
        }

        // 마법 마나 비용 추출
        var prefabComp = selectedMagic.GetComponent<PlayerMagicBase>();
        float cost = prefabComp.costMana;

        // 플레이어가 가진 마나보다 비용이 많으면 break
        if (prefabComp.costMana > PlayerCtrl.player.currentMana)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.3f);

        // 마법 실제 사용 부분
        var pool = UtilityManager.utility.CreatePlayerMagicPool(selectedMagic);
        var magicObject  = UtilityManager.utility.GetFromPool(pool, maxMagic);

        if (magicObject != null)
        {
            var instComp = magicObject.GetComponent<PlayerMagicBase>();
            instComp.SetPool(pool);
            PlayerCtrl.player.currentMana = Mathf.Clamp(PlayerCtrl.player.currentMana - cost, 0, PlayerCtrl.player.maxMana);
            PlayerCtrl.player.DisplayMana();
        }

        yield return new WaitForSeconds(0.3f);
        rb2D.constraints          = RigidbodyConstraints2D.FreezeRotation;
        PlayerCtrl.player.canMove = true;
    }
}