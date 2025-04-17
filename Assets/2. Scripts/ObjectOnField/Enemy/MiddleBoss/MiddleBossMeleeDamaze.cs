using UnityEngine;

public class MiddleBossMeleeDamaze : MonoBehaviour
{
    private PlayerCtrl playerCtrl;
    private MiddleBossCtrl middleBossCtrl;
    private Rigidbody2D playerRb;
    private static float PUSH_POWER = 10.0f;

    // enemyAttack의 공격력 읽어오기 위해 필요
    private void Start()
    {
        middleBossCtrl = GetComponentInParent<MiddleBossCtrl>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();
            playerCtrl.ChangeHP(middleBossCtrl.readDamage);

            // 밀어내기
            playerRb = other.GetComponent<Rigidbody2D>();
            Vector2 playerVector = other.transform.position - transform.position;
            playerVector.y = 0.0f;

            playerRb.linearVelocity = playerVector * PUSH_POWER;
            playerCtrl.GetDebuff(PlayerCtrl.DebuffType.Stun, 0.5f);
        }
    }
}
