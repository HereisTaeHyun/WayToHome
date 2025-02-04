using Unity.VisualScripting;
using UnityEngine;

public class ItemToBuy : MonoBehaviour
{
    // 구입 아이템, 드랍 또는 필드 아이템보다 고성능이지만 money 차감 필요, 이후 UI로 가격 띄워줘야 함, 자판기 NPC 필요
    public enum ItemToBuyType
    {
        MaxHpPlus,
        AttackPlus,
    }

    public ItemToBuyType itemToBuyType;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && Input.GetButtonUp("Submit"))
        {
            PlayerMove playerMove = other.GetComponent<PlayerMove>();

            switch(itemToBuyType)
            {
                case ItemToBuyType.MaxHpPlus: // 최대 체력 증가
                    if(playerMove.money >= 3)
                    {
                        playerMove.money -= 3;
                        playerMove.MaxHP += 1;
                        Debug.Log("최대 체력 증가");
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.Log("돈 부족");
                    }
                    break;
                
                case ItemToBuyType.AttackPlus: // 공격력 증가
                    if(playerMove.money >= 2)
                    {
                        playerMove.money -= 2;
                        playerMove.attack += 1;
                        Debug.Log("공격력 증가");
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.Log("돈 부족");
                    }
                    break;
            }
        }
    }
}
