using UnityEngine;
using UnityEngine.Pool;

public class FireVortex : MagicBase
{
    public MagicType magicType;
    private Collider2D attackCollider;

    protected override void Start()
    {
        base.Start();
        damage = -10.0f;

        attackCollider = GetComponent<Collider2D>();
    }
    
    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
    }

    // Player에 닿으면 공격
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.instance.readIsGameOver == false)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // 플레이어가 무적이 아니라면 공격
                if (PlayerCtrl.player.readIsInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                }
            }
        }
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }
    // 마법이 끝나고 리턴
    public void ReturnAfterAnim()
    {
        UtilityManager.utility.ReturnToPool(originPool, gameObject);
        isPool = true;
        rb2D.simulated = true;
    }
}
