using UnityEngine;

public class MiddleBossMeleeDamaze : MonoBehaviour
{
    private PlayerCtrl playerCtrl;
    private MiddleBossCtrl middleBossCtrl;
    private Rigidbody2D playerRb;
    private static float PUSH_POWER = 5.0f;

    // enemyAttack의 공격력 읽어오기 위해 필요
    private void Start()
    {
        middleBossCtrl = GetComponentInParent<MiddleBossCtrl>();
        playerCtrl = middleBossCtrl.readTarget.GetComponent<PlayerCtrl>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && playerCtrl.readInvincible == false)
        {
            // 데미지 가해
            playerCtrl.ChangeHP(middleBossCtrl.readDamage);

            // 밀어내기
            playerRb = other.GetComponent<Rigidbody2D>();
            Vector2 playerVector = other.transform.position - transform.position;

            playerVector = (playerVector.x >= 0) ? new Vector2(1, 1) : new Vector2(-1, 1);
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(playerVector * PUSH_POWER, ForceMode2D.Impulse);

            // 공중에서 멈추는 현상 방지 위해 스턴
            playerCtrl.GetDebuff(PlayerCtrl.DebuffType.Stun, 1.0f);
        }
    }
}
