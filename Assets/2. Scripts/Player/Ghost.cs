using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayTime;

    public GameObject ghost;
    public bool makeGhost;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        ghostDelayTime = ghostDelay;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (makeGhost == true)
        {
            if (ghostDelayTime > 0)
            {
                ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                spriteRenderer.flipX = PlayerCtrl.player.playerMove.readMoveDir.x < 0;

                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                SpriteRenderer ghostSR = currentGhost.GetComponent<SpriteRenderer>();
                ghostSR.sprite = spriteRenderer.sprite;
                ghostSR.flipX = spriteRenderer.flipX;

                ghostDelayTime = ghostDelay;
                Destroy(currentGhost, 1.0f);
            }
        }
    }
}
