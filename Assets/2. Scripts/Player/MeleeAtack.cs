using UnityEngine;

public class MeleeAtack : MonoBehaviour
{
    // enemy가 melee attack 범위에 있으면 공격력 가해
    // melee는 기초 공격력을 따라감
    private PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<IDamageable>(out var target))
        {
            target.ChangeHP(playerAttack.attackDamage);
        } 
    }
}
