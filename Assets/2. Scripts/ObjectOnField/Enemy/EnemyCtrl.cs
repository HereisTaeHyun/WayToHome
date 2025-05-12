using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCtrl : MonoBehaviour
{
    // Enemy의 공통 행동: 추적, 사망, 아이템 드롭

    // public 변수
    [NonSerialized] public Rigidbody2D rb2D;
    [NonSerialized] public bool isDie;

    // protected 변수
#region private
    [SerializeField] protected float enemyPushPower;
    [SerializeField] protected float stunTime;
    [SerializeField] protected float MaxHP;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected GameObject[] dropItem;
    [SerializeField] protected float[] itemWeight;
    [SerializeField] protected AudioClip enemyGetHitSFX;
    [SerializeField] protected AudioClip enemyDieSFX;
    protected Dictionary<GameObject, float> itemInformation = new Dictionary<GameObject, float>();
    protected bool canMove;
    protected float currentHP;

    // 플레이어 감지 변수
    [SerializeField] protected float scanningRadius = 10.0f;
    private LayerMask detectLayer;
    private RaycastHit2D[] rayHits = new RaycastHit2D[10];

    protected readonly int dirHash = Animator.StringToHash("MoveDir");
    protected readonly int hitHash = Animator.StringToHash("HitDir");
    protected readonly int hitTrigger = Animator.StringToHash("TakeHit");

    // 다른 객체에서 읽기 위한 변수
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
        isDie = false;
        canMove = true;
        currentHP = MaxHP;
        detectLayer = LayerMask.GetMask("Player", "Ground", "Wall");

        
        // 드롭 아이템 정보 딕셔너리 형성
        for(int i = 0; i < dropItem.Length; i++)
        {
            itemInformation.Add(dropItem[i], itemWeight[i]);
        }
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
    
    // 사정 거리 내부에 집입하는 경우 따라가기 메서드
    protected virtual void FollowingTarget(float moveSpeed, float scanningRadius)
    {
    }

    // HP 변경 처리
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
    
    // 아이템 드롭 처리
    protected virtual GameObject ItemDrop(Dictionary<GameObject, float> item)
    {
        // 유틸리티 매니저의 ItemNormalizer로 가중치를 100으로 정규화
        item = UtilityManager.utility.ItemNormalizer(item);

        // 순서에 따른 영향 제거하기 위해 정렬
        item.OrderByDescending(x => x.Value);

        // 0 ~ 100 선택하여 확률을 음수로 만드는 키가 있으면 반환
        float randomValue = Random.Range(0, 100);
        foreach(var elem in item)
        {
            randomValue -= elem.Value;
            if(randomValue <= 0)
            {
                return elem.Key;
            }
        }
        return null;
    }

    // 사망 처리
    protected virtual void EnemyDie()
    {
        gameObject.SetActive(false);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanningRadius);
    }
}
