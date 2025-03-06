using UnityEngine;

public class DamageZone : MonoBehaviour
{
    // 진입 시 HP에 데미지를 주는 공간
    private PlayerCtrl playerCtrl;
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.collider.GetComponent<PlayerCtrl>();
            playerCtrl.ChangeHP(-1);
        } 
    }
}
