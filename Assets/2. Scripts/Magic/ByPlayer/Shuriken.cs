using UnityEngine;
using UnityEngine.Pool;

public class Shuriken : PlayerMagicBase
{
    public PlayerMagicType playerMagicType;
    private Vector2 moveDir;
    private float lifeSpan = 5.0f;

    protected override void Start()
    {
        base.Start();

        moveSpeed = 5.0f;
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
        if (rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
        }
        moveDir = UtilityManager.utility.AllDirSet(PlayerCtrl.player.aimPos);
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        lifeSpan = 5.0f;
        originPool = pool;
        isPool = false;

        rb2D.linearVelocity = gameObject.transform.right * 10.0f;
    }
}
