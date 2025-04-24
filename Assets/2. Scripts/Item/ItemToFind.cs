using System.Collections;
using UnityEngine;

public class ItemToFind : ItemBase
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

    // 생성 후 30초 동안 필드에 존재
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
         // 충돌 물체가 플레이어일 경우 획득
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();

            switch(itemToFindType) // 아이템 사용
            {    
                case ItemToFindType.Heal: // 체력 1 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.ChangeHP(1);
                        // Destroy(transform.parent.gameObject);
                        transform.parent.gameObject.SetActive(false);
                    }
                    break;

                case ItemToFindType.Money: // 돈 획득
                    playerCtrl.GetMoney(1);
                    // Destroy(transform.parent.gameObject);
                    transform.parent.gameObject.SetActive(false);
                    break;
                case ItemToFindType.Gold: // 금괴 획득
                    playerCtrl.GetMoney(3);
                    // Destroy(transform.parent.gameObject);
                    transform.parent.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
