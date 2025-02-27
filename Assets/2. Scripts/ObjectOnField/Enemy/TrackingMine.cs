using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TrackingMine : MonoBehaviour
{
    // 인근을 스캐닝하여 가까이 가서 공격하는 타입의 적 엔티티

    // public 변수

    // private 변수
    private static float EXP_POWER = 12.0f;
    // damage는 EnemyCtrl 설정 값 이용
    private float damage;
    private EnemyCtrl enemyCtrl;

    // Player에 영향 미치는 부분
    private PlayerCtrl playerCtrl;
    private PlayerMove playerMove;
    private Rigidbody2D playerRb;

    private readonly int attackHash = Animator.StringToHash("Attack");

    void Start()
    {
        enemyCtrl = GetComponent<EnemyCtrl>();
        damage = enemyCtrl.readDamage;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other != null)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                playerCtrl = other.GetComponent<PlayerCtrl>();
                playerMove = other.GetComponent<PlayerMove>();
                playerRb = other.GetComponent<Rigidbody2D>();

                // 플레이어가 무적이 아니라면 공격
                if(playerCtrl.readInvincible != true)
                {
                    enemyCtrl.enemyAnim.SetTrigger(attackHash);
                    StartCoroutine(Attack(other));
                }
            }
        }
    }

    IEnumerator Attack(Collider2D target)
    {
        yield return new WaitForSeconds(0.1f);
        // 에셋 찾으면 파티클 Instantiate 후 destroy 추가 필요, 아직 에셋은 안찾았음

        if(target != null)
        {       
            // Player가 Mine의 왼쪽 or 오른쪽 계산 후 폭파력에 따라 밀려남
            Vector2 playerMineVector = target.transform.position - transform.position;
            if (playerMove.readIsGround)
            {
                // 땅에 있으면 수평으로 밀기
                playerMineVector = (playerMineVector.x >= 0) ? new Vector2(1, 0) : new Vector2(-1, 0);
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(playerMineVector * EXP_POWER, ForceMode2D.Impulse);
            }
            else if (playerMove.readIsJump)
            {
                // 점프 중이면 대각선 위로 밀기, 공중은 friction 없어서 폭파력 너무 커지기 줄여서 적용
                playerMineVector = (playerMineVector.x >= 0) ? new Vector2(1, 1) : new Vector2(-1, 1);
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(playerMineVector * (EXP_POWER / 2), ForceMode2D.Impulse);
            }

            // Player에게 데미지 가해 및 1.5초간 스턴
            playerCtrl.ChangeHP(damage);
            playerCtrl.GetDebuff(PlayerCtrl.DebuffType.Stun, 1.5f);
        }
    }
}
