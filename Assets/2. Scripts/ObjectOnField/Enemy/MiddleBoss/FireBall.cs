using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    // 인근을 스캐닝하여 가까이 가서 공격하는 타입의 적 엔티티

    // public 변수

    // private 변수
    private static float EXP_POWER = 12.0f;
    // damage는 EnemyCtrl 설정 값 이용
    private float damage = 1.0f;

    // Player에 영향 미치는 부분
    private PlayerCtrl playerCtrl;

    void Update()
    {
        
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                playerCtrl = other.collider.GetComponent<PlayerCtrl>();

                // 플레이어가 무적이 아니라면 공격
                if(playerCtrl.readInvincible != true)
                {
                    playerCtrl.ChangeHP(damage);
                }
            }
        }
    }
}
