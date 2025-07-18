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
                SpriteRenderer ghostSprite = currentGhost.GetComponent<SpriteRenderer>();
                ghostSprite.sprite = spriteRenderer.sprite;
                ghostSprite.flipX = spriteRenderer.flipX;

                ghostSprite.color = new Color(1, 1, 1, 0.5f);
                StartCoroutine(UtilityManager.utility.ChangeAlpha(ghostSprite, 0f, 0.3f));

                ghostDelayTime = ghostDelay;
                // Destroy(currentGhost, 1.0f);
            }
        }
    }
}
