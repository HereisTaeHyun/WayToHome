using Unity.VisualScripting;
using UnityEngine;

public class ItemToBuy : MonoBehaviour
{
    // 구입 아이템, 드랍 또는 필드 아이템보다 고성능이지만 money 차감 필요
    // 자판기 NPC에서 획득 가능

    // public 변수

    // private 변수
    [SerializeField] private ItemToBuyType itemToBuyType;
    private enum ItemToBuyType
    {
        MaxHpPlus,
        AttackPlus,
        PremiumHeal,
    }
    private PlayerCtrl playerCtrl;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 획득
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();

            switch(itemToBuyType)
            {
                case ItemToBuyType.MaxHpPlus: // 최대 체력 증가
                    playerCtrl.MaxHP += 1;
                    Debug.Log("최대 체력 증가");
                    Destroy(gameObject);
                    Destroy(transform.parent.gameObject);
                    break;
                
                case ItemToBuyType.AttackPlus: // 공격력 증가
                    playerCtrl.attack += 1;
                    Debug.Log("공격력 증가");
                    Destroy(gameObject);
                    Destroy(transform.parent.gameObject);
                    break;

                case ItemToBuyType.PremiumHeal: // 체력 2 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.ChangeHP(2);
                        Debug.Log("체력 회복");
                        Destroy(gameObject);
                        Destroy(transform.parent.gameObject);
                    }
                    else // 최대 체력이면 사용안됨
                    {
                        Debug.Log("이미 최대 체력");
                    }
                    break;
            }
        }
    }
}
