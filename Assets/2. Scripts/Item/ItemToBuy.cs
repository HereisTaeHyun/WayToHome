using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ItemToBuy : MonoBehaviour
{
    // 구입 아이템, 드랍 또는 필드 아이템보다 고성능이지만 money 차감 필요
    // 자판기 NPC에서 획득 가능

    // public 변수

    // private 변수
    [SerializeField] private ItemToBuyType itemToBuyType;
    private enum ItemToBuyType
    {
        MaxHpPlus,
        AttackPlus,
        PremiumHeal,
        MaxJumpPlus,
    }
    private PlayerCtrl playerCtrl;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    private SpriteRenderer spriteRenderer; // spriteRenderer는 부모 객체에 있음
    private static float LIFESPAN = 120;
    private float remainLifespan;
    private static float BLINK_TIME = 0.3f;
    private bool isBlink;


    // 생성 후 120초 동안 필드에 존재
    private void Start()
    {
        remainLifespan = LIFESPAN;
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        isBlink = false;
    }
    private void Update()
    {
        remainLifespan -= Time.deltaTime;

        // 남은 수명이 10초 이하라면 깜빡거리기, 점프 추가 아이템은 중요 아이템으로 예외
        if(remainLifespan <= 10 && isBlink == false && itemToBuyType != ItemToBuyType.MaxJumpPlus)
        {
            isBlink = true;
            StartCoroutine(BlinkUntilDestroy());
        }
        // 남은 수명이 0 이하면 파괴, 점프 추가 아이템은 중요 아이템이니 파괴하지 않기
        if(remainLifespan <= 0 && itemToBuyType != ItemToBuyType.MaxJumpPlus)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 획득
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();
            playerMove = other.GetComponent<PlayerMove>();
            playerAttack = other.GetComponent<PlayerAttack>();

            switch(itemToBuyType)
            {
                case ItemToBuyType.MaxHpPlus: // 최대 체력 증가
                    playerCtrl.MaxHpPlus();
                    Destroy(transform.parent.gameObject);
                    break;

                case ItemToBuyType.PremiumHeal: // 체력 2 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.ChangeHP(2);
                        Debug.Log("체력 회복");
                        Destroy(transform.parent.gameObject);
                    }
                    else // 최대 체력이면 사용안됨
                    {
                        Debug.Log("이미 최대 체력");
                    }
                    break;
                
                case ItemToBuyType.AttackPlus: // 공격력 증가
                    playerCtrl.Attacklus();
                    Destroy(transform.parent.gameObject);
                    break;
                
                // playerMove에 영향
                case ItemToBuyType.MaxJumpPlus: // 점프 횟수 추가
                    playerCtrl.MaxJumpPlus();
                    Destroy(gameObject);
                    break;
            }
        }
    }

    // 수명이 얼마 남지 않았다면 깜빡거리기 시작
    IEnumerator BlinkUntilDestroy()
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;
        // 아직 시간이 남아 있지만 남은 시간이 적을 경우
        while(remainLifespan >= 0)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if(isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if(isBlink == false)
            {
                color.a = 1.0f;
                spriteRenderer.color = color;
                isBlink = true;
            }
            yield return new WaitForSeconds(BLINK_TIME);
        }
        // 기본 상태로 초기화
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
