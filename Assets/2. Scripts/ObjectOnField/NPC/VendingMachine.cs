using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour
{
    // 고급 아이템을 파는 자판기, 한 대당 2회 사용 가능

    // 퍼블릭 변수

    // 프라이빗 변수
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private GameObject vendingUI;
    [SerializeField] private GameObject menu;
    [SerializeField] private TextMeshProUGUI statement;
    [SerializeField] private GameObject[] SellingItems;
    [SerializeField] private int[] itemPrices;
    private Dictionary<GameObject, int> itemInformation = new Dictionary<GameObject, int>();
    private int useCount;
    private PlayerCtrl playerCtrl; // 소지금 체크에 필요
    void Start()
    {
        vendingUI.SetActive(false);
        for(int i = 0; i < SellingItems.Length; i++)
        {
            itemInformation.Add(SellingItems[i], itemPrices[i]);
        }
    }
    void Update()
    {
        if(useCount == 2)
        {
            statement.text = "Sold Out";
            menu.SetActive(false);
        }
    }

    // 플레이어 접촉 시 UI 활성화
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            vendingUI.SetActive(true);
            playerCtrl = other.GetComponent<PlayerCtrl>();
        }
    }

    // 플레이어 나가면 비활성화
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            vendingUI.SetActive(false);
        }
    }

    // 버튼으로 입력 받은 아이템 구매
    public void buyItem(GameObject buyingItem)
    {
        if(itemInformation.ContainsKey(buyingItem)) // itemInformation에 buyingItem이 있으면 itemPrice만큼 돈 차감 후 아이템 드랍
        {
            int itemPrice = itemInformation[buyingItem];
            if(playerCtrl.money >= itemPrice)
            {
                playerCtrl.money -= itemPrice;
                useCount += 1;
                Instantiate(buyingItem, itemSpawnPoint.position, itemSpawnPoint.rotation);
                Debug.Log($"잔액 : {playerCtrl.money}");
            }
            else
            {
                Debug.Log("돈 부족");
            }
        }
    }
}
