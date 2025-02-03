using Unity.VisualScripting;
using UnityEngine;

public class ItemToBuy : MonoBehaviour
{
    // 구입 아이템, 드랍 또는 필드 아이템보다 고성능이지만 money 차감 필요, 이후 UI로 가격 띄워줘야 함, NPC 필요
    public enum ItemToBuyType
    {
        MaxHpPlus,
        AttackPlus,
    }

    public ItemToBuyType itemToBuyType;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            PlayerMove playerMove = other.GetComponent<PlayerMove>();

            switch(itemToBuyType)
            {
                case ItemToBuyType.MaxHpPlus: // 최대 체력 증가
                    playerMove.MaxHP += 1;
                    Debug.Log("최대 체력 증가");
                    Destroy(gameObject);
                    break;
                
                case ItemToBuyType.AttackPlus: // 공격력 증가
                    playerMove.attack += 1;
                    Debug.Log("공격력 증가");
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
