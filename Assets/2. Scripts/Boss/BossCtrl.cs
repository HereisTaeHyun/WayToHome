using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using System;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class BossCtrl : MonoBehaviour, IDamageable, IDie
{
    // Enemy의 공통 행동: 추적, 사망, 아이템 드롭

    // public 변수
    [NonSerialized] public Rigidbody2D rb2D;
    public bool isDie { get; protected set; }

    // protected 변수
    #region private
    [SerializeField] protected float maxHP;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected AudioClip enemyGetHitSFX;
    [SerializeField] protected AudioClip enemyDieSFX;
    [SerializeField] protected BossRoomSensor bossRoomSensor;
    protected float currentHP;
    protected readonly int dirHash = Animator.StringToHash("MoveDir");
    protected readonly int hitHash = Animator.StringToHash("HitDir");
    protected readonly int hitTrigger = Animator.StringToHash("TakeHit");
    protected SpriteRenderer spriteRenderer;
    protected bool ableBlink = true;
    protected float blinkTime = 0.1f;
    protected bool canAttack = true;
    protected bool canMove = true;
    protected float coolTime;
    protected LayerMask detectLayer;
    protected RaycastHit2D[] rayHits = new RaycastHit2D[10];


    // 다른 객체에서 읽기 위한 변수
    protected int enemyID;
    public int readEnemyID { get { return enemyID; } }
    protected Animator anim;
    public Animator readAnim { get { return anim; } }
    #endregion

    protected virtual void Init()
    {
        enemyID = Animator.StringToHash($"{SceneManager.GetActiveScene().name}_{gameObject.name}");
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        isDie = false;
        canMove = true;
        ableBlink = true;

        currentHP = maxHP;
        detectLayer = LayerMask.GetMask("Player", "Ground", "Wall");
    }

    public virtual void ChangeHP(float value)
    {

    }

    // ray를 쏘아 첫 대상이 플레이어인지 감지 = 시야 개념
    protected bool SeeingPlayer()
    {
        Vector2 direction = PlayerCtrl.player.transform.position - transform.position;
        Vector2 directionNorm = UtilityManager.utility.AllDirSet(direction);
        float distance = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);
        int count = Physics2D.RaycastNonAlloc(transform.position, directionNorm, rayHits, distance, detectLayer);

        Debug.DrawRay(transform.position, directionNorm * distance, Color.red, 0.1f);

        // ray에 닿은 존재가 있으며 첫 충돌이 playerLayer라면 true
        if (count > 0)
        {
            var hit = rayHits[0];

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // 사망 처리
    protected virtual void EnemyDie()
    {
        
    }
}
