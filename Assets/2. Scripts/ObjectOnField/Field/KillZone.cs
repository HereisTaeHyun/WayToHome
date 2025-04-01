using UnityEngine;

public class KillZone : MonoBehaviour
{
    // 진입 시 사망하는 곳
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
