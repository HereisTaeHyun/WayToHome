using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineCtrl : MonoBehaviour
{
    // 고급 아이템을 파는 자판기, 한 대당 2회 사용 가능
    // HP+ : 5$, Attck+ : 3$, 이후 딕셔너리로 정리하는게 편할듯?

    // 퍼블릭 변수
    public GameObject vendingUI;
    public GameObject menu;
    public TextMeshProUGUI statement;
    public GameObject[] SellingItems;

    // 프라이빗 변수
    private int useCount;
    private PlayerCtrl playerCtrl; // 소지금 체크에 필요
    void Start()
    {
        vendingUI.SetActive(false);
    }
    void Update()
    {
        if(useCount == 2)
        {
            statement.text = "Sold Out";
            menu.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어 접촉 시 UI 활성화
        if(other.CompareTag("Player"))
        {
            vendingUI.SetActive(true);
            playerCtrl = other.GetComponent<PlayerCtrl>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어 나가면 비활성화
        if(other.CompareTag("Player"))
        {
            vendingUI.SetActive(false);
        }
    }

    public void buyItem(int SellingItemType)
    {
        if(SellingItemType == 0) // HP+ 아이템, 5원
        {
            if(playerCtrl.money >= 5)
            {
                playerCtrl.money -= 5;
                useCount += 1;
                Instantiate(SellingItems[SellingItemType], transform.position, transform.rotation);
                Debug.Log($"잔액 : {playerCtrl.money}");
            }
            else
            {
                Debug.Log("돈 부족");
            }
        }
        else if(SellingItemType == 1) // Attack+ 아이템, 3원
        {
            if(playerCtrl.money >= 3)
            {
                playerCtrl.money -= 3;
                useCount += 1;
                Instantiate(SellingItems[SellingItemType], transform.position, transform.rotation);
                Debug.Log($"잔액 : {playerCtrl.money}");
            }
            else
            {
                Debug.Log("돈 부족");
            }
        }
    }
}
