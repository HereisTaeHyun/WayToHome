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
            ItemUse(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void ItemUse(GameObject Player)
    {
        switch(itemType)
        {
            case ItemType.HPrecovery:
                HPrecovery();
                break;
            case ItemType.MaxJumpPlus:
                MaxJumpPlus();
                break;
        }
    }

    private void HPrecovery()
    {
        Debug.Log("체력 회복 아이템과 접촉");
    }

    private void MaxJumpPlus()
    {
        Debug.Log("점프 추가 아이템과 접촉");
    }
}
