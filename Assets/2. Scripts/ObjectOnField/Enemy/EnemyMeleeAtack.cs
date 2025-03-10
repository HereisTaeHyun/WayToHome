using UnityEngine;

public class EnemyMeleeAtack : MonoBehaviour
{
    private PlayerCtrl playerCtrl;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();
            playerCtrl.ChangeHP(-1);
        } 
    }
}
