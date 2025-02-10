using UnityEngine;

public class KillZone : MonoBehaviour
{
    // 진입 시 사망하는 곳
    // 일단 Destroy로 구현 이후 PalyerCtrl이나 PlayerMove에 PlayerDie 메서드 추가하면 해당 메서드 실행으로 변경
    // 플레이어 아닌 엔티티에 대한 적용은 아직 고민 중, 아마 한다면 CompareTag("Enemy") 후 엔티티 스크립트 EnemyDie 호출?
    PlayerCtrl playerCtrl;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.gameObject.GetComponent<PlayerCtrl>();
            playerCtrl.ChangeHP(-9999);
        } 
    }
}
