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
    private static readonly Dictionary<SellingStat, int> ItemPrice = new Dictionary<SellingStat, int>
    {
        { SellingStat.Hp,     10 },
        { SellingStat.Mana,   10 },
        { SellingStat.Damage, 15 }
    };

    void Start()
    {
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

    public void BuyItem(SellingStat sellingStat)
    {
        int cost = ItemPrice[sellingStat];
        if (PlayerCtrl.player.money < cost)
        {
            UtilityManager.utility.PlaySFX(buyFailSFX);
            return;
        }
        switch (sellingStat)
            {
                case SellingStat.Hp:
                    PlayerCtrl.player.money -= 10;
                    PlayerCtrl.player.DisplayMoney();
                    PlayerCtrl.player.MaxHpPlus();
                    break;
                case SellingStat.Mana:
                    PlayerCtrl.player.money -= 10;
                    PlayerCtrl.player.DisplayMoney();
                    PlayerCtrl.player.MaxManaPlus();
                    break;
                case SellingStat.Damage:
                    PlayerCtrl.player.money -= 15;
                    PlayerCtrl.player.DisplayMoney();
                    PlayerCtrl.player.DamagePlus();
                    break;
            }
    }
}
