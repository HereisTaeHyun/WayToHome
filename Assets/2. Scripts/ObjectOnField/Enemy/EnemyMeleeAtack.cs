using UnityEngine;

public class EnemyMeleeAtack : MonoBehaviour
{
    private EnemyAttack enemyAttack;

    // enemyAttack의 공격력 읽어오기 위해 필요
    private void Start()
    {
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerCtrl.player.ChangeHP(enemyAttack.attackDamage);
        } 
    }
}
