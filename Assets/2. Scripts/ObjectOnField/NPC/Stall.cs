using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stall : MonoBehaviour
{
    // 아이템을 파는 자판기, 한 대당 2회 사용 가능

    // 퍼블릭 변수

    // 프라이빗 변수
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private GameObject[] sellingItems;
    [SerializeField] private int[] itemPrices;
    [SerializeField] private AudioClip moneySFX;
    [SerializeField] private AudioClip buyFailSFX;
    private Dictionary<GameObject, int> itemInformation = new Dictionary<GameObject, int>(); // 판매품, 가격 받는 딕셔너리
    private GameObject itemSpawnPoint;

    void Start()
    {
        itemSpawnPoint = transform.Find("ItemSpawnPoint").gameObject;

        // 판매 아이템 정보 딕셔너리 형성 sellingItem이 Key, itemPrices가 value
        for (int i = 0; i < sellingItems.Length; i++)
        {
            itemInformation.Add(sellingItems[i], itemPrices[i]);
        }
        
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

    // 버튼으로 입력 받은 아이템 구매
    public void BuyItem(GameObject buyingItem)
    {
        // button으로 입력 받는 것은 key인 sellingItem임
        // itemInformation에 buyingItem이 있으면 itemPrice만큼 player.money 차감 및 자판기 사용 수 증가
        // 이후 아이템을 itemSpawnPoint에 생성
        if(itemInformation.ContainsKey(buyingItem))
        {
            int itemPrice = itemInformation[buyingItem];
            if(PlayerCtrl.player.money >= itemPrice)
            {
                PlayerCtrl.player.money -= itemPrice;
                UtilityManager.utility.PlaySFX(moneySFX);
                UtilityManager.utility.SetItemFromPool(itemSpawnPoint.transform, buyingItem);
            }
            else
            {
                UtilityManager.utility.PlaySFX(buyFailSFX);
            }
        }
    }
}
