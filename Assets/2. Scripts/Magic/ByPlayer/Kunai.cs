using UnityEngine;
using UnityEngine.Pool;

public class Kunai : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 5.0f;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
        damage = -1.0f;
    }

    private void FixedUpdate()
    {
        MoveMagic();

        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0)
        {
            ReturnToOriginPool();
        }
    }

    private void MoveMagic()
    {
        newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        rb2D.linearVelocity = newVelocity;
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.aimPos);
        lifeSpan = 5.0f;
        originPool = pool;
        isPool = false;
    }
}
