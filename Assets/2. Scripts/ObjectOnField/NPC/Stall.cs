using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stall : MonoBehaviour
{
    // 아이템을 파는 자판기, 한 대당 2회 사용 가능

    // 퍼블릭 변수

    // 프라이빗 변수
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private AudioClip moneySFX;
    [SerializeField] private AudioClip buyFailSFX;
    private Dictionary<GameObject, int> itemInformation = new Dictionary<GameObject, int>(); // 판매품, 가격 받는 딕셔너리
    private GameObject itemSpawnPoint;

    void Start()
    {
        itemSpawnPoint = transform.Find("ItemSpawnPoint").gameObject;


        text.text = "Welcome!";
    }

    // 플레이어 접촉 시 UI 활성화, 기본은 start에서 비활성화하기
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCtrl.player.canAttack = false;
            text.text = "Press E to Use!";

            if (PlayerCtrl.player.isSubmit)
            {
                PlayerCtrl.player.isSubmit = false;
                if (UIManager.uIManager.StallUI.activeSelf)
                {
                    UIManager.uIManager.CloseStallUI();
                }
                else
                {
                    UIManager.uIManager.OpenStallUI(this);
                }
            }
        }
    }

    // 플레이어 나가면 비활성화
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCtrl.player.canAttack = true;
            text.text = "Welcome!";

            UIManager.uIManager.CloseStallUI();
        }
    }

    // 버튼으로 입력 받은 아이템 구매 => 스탯 자체를 수정하는 방향으로 고민 중
    // public void BuyItem(GameObject buyingItem)
    // {
    //     if (itemInformation.ContainsKey(buyingItem))
    //     {
    //         int itemPrice = itemInformation[buyingItem];
    //         if (PlayerCtrl.player.money >= itemPrice)
    //         {
    //             PlayerCtrl.player.money -= itemPrice;
    //             UtilityManager.utility.PlaySFX(moneySFX);
    //             UtilityManager.utility.SetItemFromPool(itemSpawnPoint.transform, buyingItem);
    //         }
    //         else
    //         {
    //             UtilityManager.utility.PlaySFX(buyFailSFX);
    //         }
    //     }
    // }

    public void BuyItem(SellingStat sellingStat)
    {
        switch (sellingStat)
        {
            case SellingStat.Hp:
                PlayerCtrl.player.MaxHpPlus();
                break;
            case SellingStat.Mana:
                PlayerCtrl.player.MaxManaPlus();
                break;
            case SellingStat.Damage:
                PlayerCtrl.player.DamagePlus();
                break;
        }
    }
}
