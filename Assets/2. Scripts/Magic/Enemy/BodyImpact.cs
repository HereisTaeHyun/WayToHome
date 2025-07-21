using UnityEngine;
using UnityEngine.Pool;

public class BodyImpact : MagicBase
{
    public MagicType magicType;

    protected override void Start()
    {
        base.Start();
        damage = -10.0f;
    }
    
    // Wall, Ground 등에 닿으면 풀 리턴
    protected override void OnTriggerStay2D(Collider2D other)
    {    
        if(GameManager.instance.readIsGameOver == false)
        {
            if(other.gameObject.CompareTag("Player"))
            {

                // 플레이어가 무적이 아니라면 공격
                if(PlayerCtrl.player.readIsInvincible != true)
                {
                    PlayerCtrl.player.ChangeHP(damage);
                }
            }
        }
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        originPool = pool;
        isPool = false;
    }

    public void ReturnAfterAnim()
    {
        ReturnToOriginPool();
    }

    // 풀로 되돌리기
    protected override void ReturnToOriginPool()
    {
        if (isPool == true)
        {
            return;
        }
        else
        {
            isPool = true;
            UtilityManager.utility.ReturnToPool(originPool, gameObject);
        }
    }
}
