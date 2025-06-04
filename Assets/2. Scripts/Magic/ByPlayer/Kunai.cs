using UnityEngine;

public class Kunai : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private Vector2 moveDir;
    private Vector2 newVelocity;
    private float lifeSpan = 5.0f;
    protected override void Start()
    {
        base.Start();

        moveSpeed = 1.0f;
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
}
