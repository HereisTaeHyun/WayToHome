using UnityEngine;

public class EnemyMeleeAtack : MonoBehaviour
{
    private PlayerCtrl playerCtrl;
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();
            playerCtrl.ChangeHP(-1);
        } 
    }
}
