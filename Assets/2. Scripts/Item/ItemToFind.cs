using UnityEngine;

public class ItemToFind : MonoBehaviour
{
    // 필드에 배치 or 드랍, 접촉하면 획득되는 아이템

    // public 변수

    // private 변수
    [SerializeField] private ItemToFindType itemToFindType;
    private enum ItemToFindType
    {
        Heal,
        Money,
        Gold,
    }
    private PlayerCtrl playerCtrl;
    private static float LIFESPAN = 60;
    public float remainLifespan;

    // 생성 후 60초 동안 필드에 존재
    private void OnEnable()
    {
        remainLifespan = LIFESPAN;
    }
    private void Update()
    {
        remainLifespan -= Time.deltaTime;
        if(remainLifespan <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
         // 충돌 물체가 플레이어일 경우 획득
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();

            switch(itemToFindType) // 아이템 사용
            {    
                // playerCtrl에 영향
                case ItemToFindType.Heal: // 체력 1 회복
                    if(playerCtrl.currentHP < playerCtrl.MaxHP)
                    {
                        playerCtrl.ChangeHP(1);
                        Debug.Log("체력 회복");
                        Destroy(transform.parent.gameObject);
                    }
                    else // 최대 체력이면 사용안됨
                    {
                        Debug.Log("이미 최대 체력");
                    }
                    break;

                case ItemToFindType.Money: // 돈 획득
                    playerCtrl.money += 1;
                    Debug.Log("돈 획득");
                    Destroy(transform.parent.gameObject);
                    break;
                case ItemToFindType.Gold: // 금괴 획득
                    playerCtrl.money += 3;
                    Debug.Log("금괴 획득");
                    Destroy(transform.parent.gameObject);
                    break;
            }
        }
    }
}
