using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class MagicShop : MonoBehaviour
{
    // 프라이빗 변수
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject[] sellingMagic;
    [SerializeField] private int[] magicPrice;
    [SerializeField] private AudioClip moneySFX;
    [SerializeField] private AudioClip buyFailSFX;
    private Dictionary<GameObject, int> magicInformation = new Dictionary<GameObject, int>();
    private Animator anim;
    private readonly int sellMagicHash = Animator.StringToHash("SellMagic");

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        UI.SetActive(false);

        // 판매 아이템 정보 딕셔너리 형성 sellingItem이 Key, itemPrices가 value
        for (int i = 0; i < sellingMagic.Length; i++)
        {
            magicInformation.Add(sellingMagic[i], magicPrice[i]);
        }
    }

    // 플레이어 접촉 시 UI 활성화, 기본은 start에서 비활성화하기
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            UI.SetActive(true);
            PlayerCtrl.player.canAttack = false;
        }
    }

    // 플레이어 나가면 비활성화
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            UI.SetActive(false);
            PlayerCtrl.player.canAttack = true;
        }
    }

    
    // 버튼으로 입력 받은 마법
    public void BuyMagic(GameObject buyingMagic)
    {
        // button으로 입력 받는 것은 key인 sellingMagic임
        // magicInformation에 buyingMagic이 있으면 magicPrice만큼 player.money 차감
        if(magicInformation.ContainsKey(buyingMagic))
        {
            // 이미 보유한 마법이거나 돈이 없으면 구입 불가능
            int magicPrice = magicInformation[buyingMagic];
            if (PlayerCtrl.player.playerAttack.UsingMagic.Contains(buyingMagic) || PlayerCtrl.player.money < magicPrice)
            {
                UtilityManager.utility.PlaySFX(buyFailSFX);
                return;
            }
            
            int targetIdx = Array.FindIndex(PlayerCtrl.player.playerAttack.UsingMagic, m => m == null);
            if (targetIdx != -1)
            {
                PlayerCtrl.player.playerAttack.UsingMagic[targetIdx] = buyingMagic;
                anim.SetTrigger(sellMagicHash);
            }
            else if (targetIdx == -1)
            {
                targetIdx = PlayerCtrl.player.playerAttack.selectedMagicIdx;
                PlayerCtrl.player.playerAttack.UsingMagic[targetIdx] = buyingMagic;
                anim.SetTrigger(sellMagicHash);
            }

            PlayerCtrl.player.money -= magicPrice;
            UtilityManager.utility.PlaySFX(moneySFX);
        }
    }
}
