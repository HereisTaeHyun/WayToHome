using UnityEngine;

public class ItemToFind : MonoBehaviour
{
    // 필드에 배치되어 있고 가까이 가면 획득되는 아이템
    public enum ItemToFindType
    {
        HPrecovery,
        MaxJumpPlus,
        Money,
    }
    public ItemToFindType itemToFindType;
    PlayerCtrl playerCtrl;
    PlayerMove playerMove;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")) // 충돌 물체가 플레이어일 경우 컴포넌트를 받아와 아이템 사용
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();
            playerMove = other.GetComponent<PlayerMove>();

            switch(itemToFindType) // 아이템 사용
            {    
                // playerCtrl 제어
                case ItemToFindType.HPrecovery: // 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.currentHP += 1;
                        Debug.Log("체력 + 1");
                        Destroy(gameObject);
                    }
                    else // 최대 체력이면 사용안됨
                    {
                        Debug.Log("이미 최대 체력입니다.");
                    }
                    break;

                case ItemToFindType.Money: // 돈 획득
                    playerCtrl.money += 1;
                    Debug.Log("돈이다!");
                    Destroy(gameObject);
                    break;

                // playerMove 제어
                case ItemToFindType.MaxJumpPlus: // 점프 횟수 추가
                    playerMove.maxJump += 1;
                    Debug.Log("최대 점프 증가");
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
