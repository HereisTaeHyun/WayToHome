using UnityEngine;

public class MeleeAtack : MonoBehaviour
{
    // enemy가 melee attack 범위에 있으면 공격력 가해
    // melee는 기초 공격력을 따라감
    private EnemyCtrl enemyCtrl;
    private PlayerAttack playerAttack;
    public float meleeAtackDamage;

    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        meleeAtackDamage = playerAttack.readbaseAttackDamage;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            enemyCtrl = other.GetComponent<EnemyCtrl>();
            enemyCtrl.ChangeHP(meleeAtackDamage);
        } 
    }
}
