using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

    public enum ItemToBuyType
    {
        MaxHpPlus,
        AttackPlus,
        PremiumHeal,
        MaxJumpPlus,
    }

public class ItemToBuy : ItemBase
{
    // 구입 아이템, 드랍 또는 필드 아이템보다 고성능이지만 money 차감 필요
    // 자판기 NPC에서 획득 가능

    // public 변수
    [SerializeField] private ItemToBuyType itemToBuyType;


    // 생성 후 120초 동안 필드에 존재
    protected override void Update()
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
            switch (itemToBuyType)
            {
                case ItemToBuyType.MaxHpPlus:
                    usingPool = ItemManager.itemManager.maxHpPlusPool;
                    break;

                case ItemToBuyType.AttackPlus:
                    usingPool = ItemManager.itemManager.attackPlusPool;
                    break;

                case ItemToBuyType.PremiumHeal:
                    usingPool = ItemManager.itemManager.premiumHealPool;
                    break;

                case ItemToBuyType.MaxJumpPlus:
                    usingPool = ItemManager.itemManager.maxJumpPlusPool;
                    break;
            }
            UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
        }
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 획득
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();

            switch(itemToBuyType)
            {
                case ItemToBuyType.MaxHpPlus: // 최대 체력 증가
                    playerCtrl.MaxHpPlus();
                    usingPool = ItemManager.itemManager.maxHpPlusPool;
                    UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
                    break;

                case ItemToBuyType.PremiumHeal: // 체력 2 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.ChangeHP(2);
                        usingPool = ItemManager.itemManager.premiumHealPool;
                        UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
                    }
                    break;
                
                case ItemToBuyType.AttackPlus: // 공격력 증가
                    playerCtrl.Attacklus();
                    usingPool = ItemManager.itemManager.attackPlusPool;
                    UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
                    break;
                
                // playerMove에 영향
                case ItemToBuyType.MaxJumpPlus: // 점프 횟수 추가
                    playerCtrl.MaxJumpPlus();
                    usingPool = ItemManager.itemManager.maxJumpPlusPool;
                    UtilityManager.utility.ReturnToPool(usingPool, gameObject);
                    break;
            }
        }
    }
}
