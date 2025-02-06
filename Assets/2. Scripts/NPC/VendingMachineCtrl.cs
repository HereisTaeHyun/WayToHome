using UnityEngine;

public class VendingMachineCtrl : MonoBehaviour
{
    public GameObject vendingText;
    public GameObject[] SellingItem;
    public Transform itemSpawnPoint;

    private bool playerUsing;
    private PlayerCtrl playerCtrl; // 소지금 체크에 필요
    void Start()
    {
        vendingText.SetActive(false);
    }
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어 접촉 시 UI 활성화
        if(other.CompareTag("Player"))
        {
            vendingText.SetActive(true);
            playerCtrl = other.GetComponent<PlayerCtrl>();
            playerUsing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어 나가면 비활성화
        if(other.CompareTag("Player"))
        {
            vendingText.SetActive(false);
            playerUsing = false;
        }
    }
}
