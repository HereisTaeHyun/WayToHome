using UnityEngine;

public class KillZone : MonoBehaviour
{
    // 진입 시 사망하는 곳
    // 플레이어 아닌 엔티티에 대한 적용은 아직 고민 중
    // 아마 한다면 CompareTag("Enemy") 후 EnemyDie나 Destroy 호출할듯
    private PlayerCtrl playerCtrl; 
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.gameObject.GetComponent<PlayerCtrl>();
            playerCtrl.ChangeHP(-9999);
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
