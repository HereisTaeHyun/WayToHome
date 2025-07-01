using UnityEngine;

public class KillZone : MonoBehaviour
{
    // 진입 시 사망하는 곳
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerCtrl.player.ChangeHP(-9999);
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
}
