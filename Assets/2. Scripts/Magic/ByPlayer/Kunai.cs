using UnityEngine;

public class Kunai : PlayerMagicBase
{
    private Vector2 moveDir;
    private Vector2 newVelocity;
    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
        damage = -1.0f;
    }

    private void FixedUpdate()
    {
        MoveMagic();
    }

    private void MoveMagic()
    {
        newVelocity.Set(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        rb2D.linearVelocity = newVelocity;
    }
}
