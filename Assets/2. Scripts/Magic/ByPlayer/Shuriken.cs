using UnityEngine;
using UnityEngine.Pool;

public class Shuriken : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 5.0f;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 1.0f;
        damage = -2.0f;
    }

    private void FixedUpdate()
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0)
        {
            ReturnToOriginPool();
        }
    }

    public override void SetPool(ObjectPool<GameObject> pool)
    {
        lifeSpan = 5.0f;
        originPool = pool;
        isPool = false;
    }
}
