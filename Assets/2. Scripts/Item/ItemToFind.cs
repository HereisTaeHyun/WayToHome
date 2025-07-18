using UnityEngine;

public enum ItemToFindType
    {
        Heal,
        Money,
        Gold,
    }

public class ItemToFind : ItemBase
{
    // 필드에 배치 or 드랍, 접촉하면 획득되는 아이템

    // public 변수
    [SerializeField] private ItemToFindType itemToFindType;
    

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
            switch(itemToFindType) // 아이템 사용
            {    
                case ItemToFindType.Heal: // 체력 1 회복
                    if(PlayerCtrl.player.currentHP < PlayerCtrl.player.maxHP)
                    {
                        PlayerCtrl.player.ChangeHP(10);
                        usingPool = ItemManager.itemManager.healPool;
                        UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
                    }
                    break;

                case ItemToFindType.Money: // 돈 획득
                    PlayerCtrl.player.GetMoney(1);
                    usingPool = ItemManager.itemManager.moneyPool;
                    UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
                    break;
                case ItemToFindType.Gold: // 금괴 획득
                    PlayerCtrl.player.GetMoney(3);
                    usingPool = ItemManager.itemManager.goldPool;
                    UtilityManager.utility.ReturnToPool(usingPool, transform.parent.gameObject);
                    break;
            }
        }
    }
}
