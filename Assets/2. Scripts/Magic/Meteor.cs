using UnityEngine;

public class Meteor : MagicBase
{
    private Vector2 moveDir;
    private Vector2 newVelocity;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
        damage = -3.0f;
    }
}
