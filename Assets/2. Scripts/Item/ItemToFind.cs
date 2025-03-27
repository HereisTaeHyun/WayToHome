using System.Collections;
using UnityEngine;

public class ItemToFind : MonoBehaviour
{
    // 필드에 배치 or 드랍, 접촉하면 획득되는 아이템

    // public 변수

    // private 변수
    [SerializeField] private ItemToFindType itemToFindType;
    private enum ItemToFindType
    {
        Heal,
        Money,
        Gold,
    }
    private PlayerCtrl playerCtrl;
    private SpriteRenderer spriteRenderer; // spriteRenderer는 부모 객체에 있음
    private static float LIFESPAN = 30;
    public float remainLifespan;
    private static float BLINK_TIME = 0.3f;
    private bool isBlink;

    // 생성 후 30초 동안 필드에 존재
    private void Start()
    {
        remainLifespan = LIFESPAN;
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        isBlink = false;
    }
    private void Update()
    {
        remainLifespan -= Time.deltaTime;

        // 남은 수명이 10초 이하라면 깜빡거리기
        if(remainLifespan <= 10 && isBlink == false)
        {
            isBlink = true;
            StartCoroutine(BlinkUntilDestroy());
        }

        // 남은 수명이 0 이하면 파괴
        if(remainLifespan <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
         // 충돌 물체가 플레이어일 경우 획득
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();

            switch(itemToFindType) // 아이템 사용
            {    
                // playerCtrl에 영향
                case ItemToFindType.Heal: // 체력 1 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.ChangeHP(1);
                        Debug.Log("체력 회복");
                        Destroy(transform.parent.gameObject);
                    }
                    else // 최대 체력이면 사용안됨
                    {
                        Debug.Log("이미 최대 체력");
                    }
                    break;

                case ItemToFindType.Money: // 돈 획득
                    playerCtrl.money += 1;
                    Debug.Log("돈 획득");
                    Destroy(transform.parent.gameObject);
                    break;
                case ItemToFindType.Gold: // 금괴 획득
                    playerCtrl.money += 3;
                    Debug.Log("금괴 획득");
                    Destroy(transform.parent.gameObject);
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
