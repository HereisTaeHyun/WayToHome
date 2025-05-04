using UnityEngine;

public class FireCannon : MagicBase
{
    private Vector2 moveDir;
    private Vector2 newVelocity;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 1.0f;
        damage = -1.0f;

        moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.transform.position - transform.position);

        // 바라봄 축 설정
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    protected override void FixedUpdate()
    {
        // if(isLaunch == true)
        // {
        //     MoveMagic();
        // }
        MoveMagic();
        moveSpeed += Time.deltaTime * 2;
    }

    private void MoveMagic()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 이동
            newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
            rb2D.linearVelocity = newVelocity;
        }
    }

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
