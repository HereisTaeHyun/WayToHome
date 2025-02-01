using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    public enum ItemType
    {
        HPrecovery,
        MaxJumpPlus,
    }

    public ItemType itemType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = other.GetComponent<PlayerMove>();
            ItemUse(playerMove);
            Destroy(gameObject);
        }
    }

    private void ItemUse(PlayerMove playerMove)
    {
        switch(itemType)
        {
            case ItemType.HPrecovery:
                if(playerMove.currentHP < playerMove.MaxHP)
                {
                    playerMove.currentHP += 1;
                    Debug.Log("체력 회복 아이템과 접촉");
                }
                else
                {
                    Debug.Log("이미 최대 체력입니다");
                }
                break;
            case ItemType.MaxJumpPlus:
                playerMove.maxJump += 1;
                Debug.Log("점프 추가 아이템과 접촉");
                break;
        }
    }
}
