using UnityEngine;

public class ShockWave : MagicBase
{
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private readonly int moveDirHash = Animator.StringToHash("MoveDir");

    protected override void Start()
    {
        base.Start();

        moveSpeed = 4.0f;
        damage = -1.0f;

        // 음직임 방향 설정
        moveDir = UtilityManager.utility.HorizontalDirSet(PlayerCtrl.player.transform.position - transform.position);
        anim.SetFloat(moveDirHash, moveDir.x);
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        // if(isLaunch == true)
        // {
        //     MoveMagic();
        // }
        MoveMagic();
    }

    private void MoveMagic()
    {
        if(GameManager.instance.readIsGameOver == false)
        {
            // 이동
            newVelocity.Set(moveDir.x * moveSpeed, rb2D.linearVelocity.y);
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
                }
            }
            // 
            else if(other.gameObject.CompareTag("Wall"))
            {
                ReturnToOriginPool();
            }
        }
    }
}
