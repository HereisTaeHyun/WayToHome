using UnityEngine;

public class ItemToFind : MonoBehaviour
{
    // 필드에 배치되어 있고 가까이 가면 획득되는 아이템
    public enum ItemToFindType
    {
        MaxHpPlus,
        HPrecovery,
        MaxJumpPlus,
    }
    public ItemToFindType itemToFindType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")) // 충돌 물체가 플레이어일 경우 컴포넌트를 받아와 아이템 사용
        {
            PlayerMove playerMove = other.GetComponent<PlayerMove>();
            GetItem(playerMove);
            Destroy(gameObject);
        }
    }

    private void GetItem(PlayerMove playerMove)
    {
        switch(itemToFindType)
        {
            case ItemToFindType.MaxHpPlus:
                playerMove.MaxHP += 1;
                Debug.Log("최대 체력 증가");
                break;
            
            case ItemToFindType.HPrecovery:
                if(playerMove.currentHP < playerMove.MaxHP)
                {
                    playerMove.currentHP += 1;
                    Debug.Log("체력 + 1");
                }
                else
                {
                    Debug.Log("이미 최대 체력입니다.");
                }
                break;
            case ItemToFindType.MaxJumpPlus:
                playerMove.maxJump += 1;
                Debug.Log("최대 점프 증가");
                break;
        }
    }
}
