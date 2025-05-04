using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireMissile : MagicBase
{
    private Vector2 moveDir;
    private Vector2 newVelocity;
    [SerializeField] private float aimTime = 3.0f;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 10.0f;
        damage = -1.0f;

        // 이동 방향 세팅 및 조준 시작
        StartCoroutine(Aim());
    }
    protected override void FixedUpdate()
    {
        if(isLaunch == true)
        {
            MoveMagic();
        }
    }

    // aimTime 동안 플레이어 바라보다 발사되는 로직
    private IEnumerator Aim()
    {
        // 바라봄 축 설정
        while (aimTime >= 0)
        {
            moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.transform.position - transform.position);

            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            aimTime -= Time.deltaTime;
            yield return null;
        }

        isLaunch = true;
    }

    // 마법 이동
    private void MoveMagic()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 이동
            newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
            rb2D.linearVelocity = newVelocity;
        }
    }

    // Player, Wall, Ground 등에 닿으면 풀 리턴
    protected override void OnTriggerStay2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if(PlayerCtrl.player.readInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                    ReturnToOriginPool();
                }
            }
            // 
            else if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground"))
            {
                ReturnToOriginPool();
            }
        }
    }
}
